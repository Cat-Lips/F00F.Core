using Godot;

namespace F00F;

public partial class Game3Dold
{
    public static void Init() { }
    static Game3Dold() => MyInput.Init();

    private class MyInput : F00F.MyInput
    {
        public static void Init() { }
        static MyInput() => Init<MyInput>();

#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value
        public static readonly StringName Quit;
#if TOOLS
        public static readonly StringName Debug;
#endif
#pragma warning restore CS0649 // Field is never assigned to, and will always have its default value

        private static class Defaults
        {
            public static readonly Key Quit = Key.End;
#if TOOLS
            public static readonly Key Debug = Key.F12; // +Ctrl
#endif
        }

        private MyInput() { }
    }
}
