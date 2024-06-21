using Godot;

namespace F00F;

// FIXME: I only want to register QuickHelp/QuickExit input if enabled in Config

public partial class GameInput
{
    static GameInput()
        => MyInput.Init();

    private class MyInput : InputConfig
    {
        public static void Init() { }
        static MyInput() => Init<MyInput>();

#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value
        public static readonly StringName QuickHelp;
        public static readonly StringName QuickExit;
#pragma warning restore CS0649 // Field is never assigned to, and will always have its default value

        private static class Default
        {
            public static readonly Key QuickHelp = Key.F1;
            public static readonly Key QuickExit = Key.End;
        }

        private MyInput() { }
    }
}
