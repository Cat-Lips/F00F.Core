using System;
using System.Threading;
using System.Threading.Tasks;
using Godot;

namespace F00F;

#if TOOLS
public partial class AutoAction(Action action, int ms = 100) : GodotObject
#else
public partial class AutoAction(Action action, int ms = 0) : GodotObject
#endif
{
    public AutoAction() : this(null) { }

    public event Action Action = action;
    public bool IsTriggered { get; private set; }
    public bool IsExecuting { get; private set; }
#if TOOLS
    private CancellationTokenSource cts;
    public void Run()
    {
        if (Editor.IsEditor)
        {
            IsTriggered = true;
            cts?.Cancel(); cts = new();
            Task.Delay(ms, cts.Token)
                .ContinueWith(_ => CallDeferred(),
                    TaskContinuationOptions.NotOnCanceled);
        }
        else
        {
            if (IsTriggered) return;
            IsTriggered = true;
            CallDeferred();
        }

        void CallDeferred()
        {
            this.CallDeferred(() =>
            {
                if (IsTriggered)
                    RunNow();
            });
        }
    }
#else
    public void Run()
    {
        if (IsTriggered) return;
        IsTriggered = true;

        this.CallDeferred(() =>
        {
            if (IsTriggered)
                RunNow();
        });
    }
#endif
    public void RunNow()
    {
        IsTriggered = false;
        IsExecuting = true;
        Action?.Invoke();
        IsExecuting = false;
    }
}
