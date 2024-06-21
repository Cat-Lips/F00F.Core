using Godot;

namespace F00F
{
    public partial class Test3D
    {
        private class MyInput : F00F.MyInput
        {
            static MyInput() => Init<MyInput>();
            private MyInput() { }

#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value
            public static readonly StringName ToggleArena;
            public static readonly StringName ToggleTarget;
#pragma warning restore CS0649 // Field is never assigned to, and will always have its default value

            private static class Defaults
            {
                public static readonly Key ToggleArena = Key.F12;
                public static readonly Key ToggleTarget = Key.F11;
            }
        }
    }
}
