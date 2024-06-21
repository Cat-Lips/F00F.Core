using System;
using Godot;

namespace F00F;

public partial class ValueWatcher
{
    static ValueWatcher()
        => MyInput.Init();

    private class MyInput : InputConfig
    {
        public static void Init() { }
        static MyInput() => Init<MyInput>("DEBUG");

#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value
        public static readonly StringName Show;
#pragma warning restore CS0649 // Field is never assigned to, and will always have its default value

        private static class Default
        {
            public static readonly Enum Show = Godot.Key.F12;
        }

        private MyInput() { }
    }

    public sealed override void _UnhandledKeyInput(InputEvent e)
    {
        if (this.Handle(e.IsActionPressed(MyInput.Show), () => Visible = !Visible)) return;
    }
}
