using System;
using System.Threading.Tasks;
using Godot;

namespace F00F;

public partial class AutoAction(Action action = null) : GodotObject
{
    public event Action Action = action;
    public bool Triggered { get; private set; }

    public virtual void Run()
    {
        if (Action is null) return;
        if (Triggered) return;
        Triggered = true;

        if (Editor.IsEditor)
            Task.Delay(100).ContinueWith(_ => CallDeferred());
        else CallDeferred();

        void CallDeferred()
        {
            this.CallDeferred(() =>
            {
                if (Triggered)
                    RunNow();
            });
        }
    }

    public virtual void RunNow()
    {
        Triggered = false;
        Action?.Invoke();
    }
}
