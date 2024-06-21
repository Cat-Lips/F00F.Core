using System;
using System.Threading;
using System.Threading.Tasks;
using Godot;

namespace F00F;

public partial class AutoAction(Action action) : GodotObject
{
    public AutoAction() : this(null) { }

    public event Action Action = action;
    public bool Triggered { get; private set; }
#if TOOLS
    private CancellationTokenSource cts;
    public void Run()
    {
        if (Editor.IsEditor)
        {
            Triggered = true;
            cts?.Cancel(); cts = new();
            Task.Delay(100, cts.Token)
                .ContinueWith(_ => CallDeferred(),
                    TaskContinuationOptions.NotOnCanceled);
        }
        else
        {
            if (Triggered) return;
            Triggered = true;
            CallDeferred();
        }

        void CallDeferred()
        {
            this.CallDeferred(() =>
            {
                if (Triggered)
                    RunNow();
            });
        }
    }
#else
    public void Run()
    {
        if (Triggered) return;
        Triggered = true;

        this.CallDeferred(() =>
        {
            if (Triggered)
                RunNow();
        });
    }
#endif
    public void RunNow()
    {
        Triggered = false;
        Action?.Invoke();
    }
}
