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
        public static readonly StringName MoveForward;
        public static readonly StringName MoveBack;
        public static readonly StringName StrafeLeft;
        public static readonly StringName StrafeRight;

        public static readonly StringName Run;
        public static readonly StringName Jump;
        public static readonly StringName Shoot;
        public static readonly StringName Interact;
#pragma warning restore CS0649 // Field is never assigned to, and will always have its default value

        private static class Defaults
        {
            public static readonly Key[] MoveForward = [Key.W, Key.Up];
            public static readonly Key[] MoveBack = [Key.S, Key.Down];
            public static readonly Key[] StrafeLeft = [Key.A, Key.Left];
            public static readonly Key[] StrafeRight = [Key.D, Key.Right];

            public static readonly Key Run = Key.Shift;
            public static readonly Key Jump = Key.Space;
            public static readonly Key Shoot = Key.Ctrl;
            public static readonly Key Interact = Key.E;
        }

        private MyInput() { }
    }
}
