using System;
using Godot;

namespace F00F;

public partial class AutoAction(Action action = null) : GodotObject
{
    //private static readonly int Delay = Editor.IsEditor ? 100 : 0;

    public event Action Action = action;
    public bool Triggered { get; private set; }

    public virtual void Run()
    {
        if (Action is null) return;
        if (Triggered) return;
        Triggered = true;

        //Task.Delay(Delay).ContinueWith(_ =>
        //{
        this.CallDeferred(() =>
        {
            if (Triggered)
                RunNow();
        });
        //});
    }

    public virtual void RunNow()
    {
        Triggered = false;
        Action?.Invoke();
    }
}
