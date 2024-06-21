using System;
using Godot;

namespace F00F;

public partial class FollowCam
{
    static FollowCam()
        => MyInput.Init();

    private class MyInput : InputConfig
    {
        public static void Init() { }
        static MyInput() => Init<MyInput>();

#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value
        //public static readonly StringName ZoomIn;
        //public static readonly StringName ZoomOut;

        public static readonly StringName OrbitLock;
        public static readonly StringName LookBehind;
#pragma warning restore CS0649 // Field is never assigned to, and will always have its default value

        private static class Default
        {
            //public static readonly Enum[] ZoomIn = [MouseButton.WheelUp, Key.KpAdd];
            //public static readonly Enum[] ZoomOut = [MouseButton.WheelDown, Key.KpSubtract];

            public static readonly Enum OrbitLock = MouseButton.Right;
            public static readonly Enum LookBehind = Key.Backspace;
        }

        private MyInput() { }
    }
}
