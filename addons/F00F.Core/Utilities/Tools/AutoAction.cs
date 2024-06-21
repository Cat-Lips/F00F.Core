using System;
using Godot;

namespace F00F;

public partial class AutoAction(Action action = null) : GodotObject
{
    public event Action Action = action;
    public bool Triggered { get; private set; }

    public virtual void Run()
    {
        if (Triggered) return;
        if (Action is null) return;

        Triggered = true;
        this.CallDeferred(() =>
        {
            if (Triggered)
                RunNow();
        });
    }

    public virtual void RunNow()
    {
        Triggered = false;
        Action?.Invoke();
    }
}
