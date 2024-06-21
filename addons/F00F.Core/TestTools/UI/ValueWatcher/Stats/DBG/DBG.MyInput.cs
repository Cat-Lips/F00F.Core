using Godot;

namespace F00F;

public partial class DBG
{
    public static void Init() { }
    static DBG() => MyInput.Init();

    private class MyInput : F00F.MyInput
    {
        public static void Init() { }
        static MyInput() => Init<MyInput>();

#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value
        public static readonly StringName Show;
#pragma warning restore CS0649 // Field is never assigned to, and will always have its default value

        private static class Defaults
        {
            public static readonly Key Show = Key.F12;
        }

        private MyInput() { }
    }
}
