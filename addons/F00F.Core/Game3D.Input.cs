using Godot;

namespace F00F;

public partial class Game3D
{
    static Game3D() => MyInput.Init();

    private class MyInput : F00F.MyInput
    {
        static MyInput() => Init<MyInput>();
        public static void Init() { }

#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value
        public static readonly StringName Quit;
        public static readonly StringName Debug;
#pragma warning restore CS0649 // Field is never assigned to, and will always have its default value

        private static class Defaults
        {
            public static readonly Key Quit = Key.End;
            public static readonly Key Debug = Key.Help;
        }

        private MyInput() { }
    }
}
