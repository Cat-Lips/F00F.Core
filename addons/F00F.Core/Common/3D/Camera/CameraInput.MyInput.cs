using System;
using Godot;

namespace F00F
{
    public partial class CameraInput
    {
        private class MyInput : F00F.MyInput
        {
#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value
            public static readonly StringName Forward;
            public static readonly StringName Back;
            public static readonly StringName Left;
            public static readonly StringName Right;
            public static readonly StringName Up;
            public static readonly StringName Down;
            public static readonly StringName ZoomIn;
            public static readonly StringName ZoomOut;

            public static readonly StringName Fast1;
            public static readonly StringName Fast2;
            public static readonly StringName Fast3;

            public static readonly StringName Reset;
            public static readonly StringName Select;
            public static readonly StringName ToggleSelectMode;

            public static readonly StringName RotateAroundTarget;
#pragma warning restore CS0649 // Field is never assigned to, and will always have its default value
            private static class Defaults
            {
                public static readonly Key[] Forward = [Key.W, Key.Up];
                public static readonly Key[] Back = [Key.S, Key.Down];
                public static readonly Key[] Left = [Key.A, Key.Left];
                public static readonly Key[] Right = [Key.D, Key.Right];
                public static readonly Key Up = Key.R;
                public static readonly Key Down = Key.F;
                public static readonly Enum[] ZoomIn = [MouseButton.WheelUp, Key.Plus];
                public static readonly Enum[] ZoomOut = [MouseButton.WheelDown, Key.Minus];

                public static readonly Key Fast1 = Key.Alt;
                public static readonly Key Fast2 = Key.Ctrl;
                public static readonly Key Fast3 = Key.Shift;

                public static readonly Key Reset = Key.Home;
                public static readonly Enum[] Select = [MouseButton.Left, Key.Enter, Key.KpEnter];
                public static readonly Key ToggleSelectMode = Key.Escape;

                public static readonly Key[] RotateAroundTarget = [Key.Alt, Key.Ctrl];
            }

            static MyInput() => Init<MyInput>();
            private MyInput() { }
        }
    }
}
