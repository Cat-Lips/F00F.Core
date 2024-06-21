using Godot;

namespace F00F.NEW;

public partial class Test3D
{
    static Test3D()
        => MyInput.Init();

    private class MyInput : InputConfig
    {
        public static void Init() { }
        static MyInput() => Init<MyInput>("DEBUG");

#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value
        public static readonly StringName ToggleVSync;
        public static readonly StringName ToggleValues;
#pragma warning restore CS0649 // Field is never assigned to, and will always have its default value

        private static class Default
        {
            public static readonly Key ToggleVSync = Key.F11;
            public static readonly Key ToggleValues = Key.F12;
        }

        private MyInput() { }
    }
}
