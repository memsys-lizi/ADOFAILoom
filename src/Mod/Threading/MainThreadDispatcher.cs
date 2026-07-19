using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace ADOFAILoom.Threading
{
    internal sealed class MainThreadDispatcher
    {
        private readonly ConcurrentQueue<IMainThreadRequest> requests =
            new ConcurrentQueue<IMainThreadRequest>();

        public async Task<T> InvokeAsync<T>(
            Func<T> action,
            TimeSpan timeout,
            CancellationToken cancellationToken)
        {
            if (timeout <= TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException(nameof(timeout));
            }

            using (var timeoutSource = new CancellationTokenSource(timeout))
            using (var linkedSource = CancellationTokenSource.CreateLinkedTokenSource(
                       cancellationToken,
                       timeoutSource.Token))
            {
                var request = new MainThreadRequest<T>(action, linkedSource.Token);
                requests.Enqueue(request);

                try
                {
                    return await request.Task.ConfigureAwait(false);
                }
                catch (OperationCanceledException) when (
                    timeoutSource.IsCancellationRequested && !cancellationToken.IsCancellationRequested)
                {
                    throw new TimeoutException(
                        $"The Unity main-thread operation exceeded {timeout.TotalSeconds:0.###} seconds.");
                }
            }
        }

        public void ProcessPending()
        {
            while (requests.TryDequeue(out IMainThreadRequest request))
            {
                request.Execute();
            }
        }

        public void CancelPending()
        {
            while (requests.TryDequeue(out IMainThreadRequest request))
            {
                request.Cancel();
                request.Dispose();
            }
        }

        private interface IMainThreadRequest : IDisposable
        {
            void Execute();

            void Cancel();
        }

        private sealed class MainThreadRequest<T> : IMainThreadRequest
        {
            private readonly Func<T> action;
            private readonly TaskCompletionSource<T> completion =
                new TaskCompletionSource<T>(TaskCreationOptions.RunContinuationsAsynchronously);
            private readonly CancellationTokenRegistration cancellationRegistration;

            public MainThreadRequest(Func<T> action, CancellationToken cancellationToken)
            {
                this.action = action ?? throw new ArgumentNullException(nameof(action));
                cancellationRegistration = cancellationToken.Register(Cancel);
            }

            public Task<T> Task => completion.Task;

            public void Execute()
            {
                if (completion.Task.IsCompleted)
                {
                    Dispose();
                    return;
                }

                try
                {
                    completion.TrySetResult(action());
                }
                catch (Exception exception)
                {
                    completion.TrySetException(exception);
                }
                finally
                {
                    Dispose();
                }
            }

            public void Cancel()
            {
                completion.TrySetCanceled();
            }

            public void Dispose()
            {
                cancellationRegistration.Dispose();
            }
        }
    }
}
