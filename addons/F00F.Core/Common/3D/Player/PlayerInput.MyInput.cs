using Godot;

namespace F00F;

public partial class PlayerInput
{
    public static void Init() { }
    static PlayerInput() => MyInput.Init();

    private class MyInput : F00F.MyInput
    {
        public static void Init() { }
        static MyInput() => Init<MyInput>();

#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value
        public static readonly StringName Forward;
        public static readonly StringName Back;
        public static readonly StringName Left;
        public static readonly StringName Right;
#pragma warning restore CS0649 // Field is never assigned to, and will always have its default value

        private static class Defaults
        {
            public static readonly Key[] Forward = [Key.W, Key.Up];
            public static readonly Key[] Back = [Key.S, Key.Down];
            public static readonly Key[] Left = [Key.A, Key.Left];
            public static readonly Key[] Right = [Key.D, Key.Right];
        }

        private MyInput() { }
    }
}
