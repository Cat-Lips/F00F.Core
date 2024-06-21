using System;
using System.Threading.Tasks;
using Godot;
using CT = System.Threading.CancellationToken;
using CTS = System.Threading.CancellationTokenSource;

namespace F00F;

public static class AsyncExtensions
{
    public static void RunAsync(this GodotObject _, Action action)
        => Task.Run(action);

    public static CTS RunAsync(this GodotObject source, Action<CT> action)
    {
        var cs = new CTS();
        source.RunAsync(() => action(cs.Token));
        return cs;
    }

    public static CTS RunAsync<T>(this GodotObject source, Func<CT, T> AsyncAction, Action<T> ReadyAction)
    {
        return source.RunAsync(asyncToken =>
        {
            var result = AsyncAction(asyncToken);
            source.CallDeferred(asyncToken, OnSuccess, OnComplete);

            void OnSuccess()
                => ReadyAction(result);

            void OnComplete(bool success)
            {
                if (!success && result is Node node)
                    node.Free();
            }
        });
    }

    public static void RunAsync(this GodotObject source, ref CTS cts, Action<CT> action)
    {
        cts?.Cancel();
        cts = source.RunAsync(action);
    }

    public static void RunAsync<T>(this GodotObject source, ref CTS cts, Func<CT, T> AsyncAction, Action<T> ReadyAction)
    {
        cts?.Cancel();
        cts = source.RunAsync(AsyncAction, ReadyAction);
    }

    public static void RunAsync<TObj, TRes>(this GodotObject source, ref CTS cts, Func<CT, TRes, TObj> AsyncAction, Action<TObj> ReadyAction, TRes data, bool copy = false, bool deep = false)
        where TRes : Resource
    {
        cts?.Cancel();
        if (copy) data = data.Copy(deep);
        cts = source.RunAsync(ct => AsyncAction(ct, data), ReadyAction);
    }

    public static void CallDeferred(this GodotObject source, Action action)
    {
        Callable.From(() =>
        {
            if (GodotObject.IsInstanceValid(source))
                action();
        }).CallDeferred();
    }

    public static void CallDeferred(this GodotObject source, CT ct, Action action)
    {
        Callable.From(() =>
        {
            if (!ct.Cancelled() && GodotObject.IsInstanceValid(source))
                action();
        }).CallDeferred();
    }

    public static void CallDeferred(this GodotObject source, CT ct, Action action, Action<bool> OnComplete)
    {
        Callable.From(() =>
        {
            if (!ct.Cancelled() && GodotObject.IsInstanceValid(source))
            {
                action();
                OnComplete(true);
                return;
            }

            OnComplete(false);

        }).CallDeferred();
    }
}
