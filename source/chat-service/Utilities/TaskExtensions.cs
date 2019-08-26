using System;
using System.Threading;
using System.Threading.Tasks;

namespace LemVic.Services.Chat.Utilities
{
    public static class TaskExtensions
    {
        public static Task AwaitCancellation(this CancellationToken token)
        {
            var completionSource = new TaskCompletionSource<bool>();
            token.Register(() => completionSource.SetResult(true));
            return completionSource.Task;
        }
    }
}
