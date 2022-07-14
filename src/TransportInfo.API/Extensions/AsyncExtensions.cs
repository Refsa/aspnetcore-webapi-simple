namespace TransportInfo.Extensions;

public static class AsyncExtensions
{
    public static T ResolveBlocking<T>(this Task<T> task, CancellationToken cancellationToken = default)
    {
        task.Wait(cancellationToken);
        return task.Result;
    }
}