using Godot;

namespace F00F;

public partial class MainMenu2
{
    static MainMenu2() => MyInput.Init();
    public static void Init() { }

    private class MyInput : F00F.MyInput
    {
        static MyInput() => Init<MyInput>();
        public static void Init() { }

#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value
        public static readonly StringName Pause;
#pragma warning restore CS0649 // Field is never assigned to, and will always have its default value

        private static class Defaults
        {
            public static readonly Key[] Pause = [Key.Escape, Key.Pause];
        }

        private MyInput() { }
    }
}
