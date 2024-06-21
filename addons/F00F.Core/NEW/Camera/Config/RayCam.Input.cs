using System;
using Godot;

namespace F00F;

public partial class RayCam
{
    static RayCam()
        => MyInput.Init();

    private class MyInput : InputConfig
    {
        public static void Init() { }
        static MyInput() => Init<MyInput>("SelectMode");

#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value
        public static readonly StringName Toggle;
        public static readonly StringName Select;
#pragma warning restore CS0649 // Field is never assigned to, and will always have its default value

        private static class Default
        {
            public static readonly Enum Toggle = Key.Escape;
            public static readonly Enum Select = MouseButton.Left;
        }

        private MyInput() { }
    }
}
