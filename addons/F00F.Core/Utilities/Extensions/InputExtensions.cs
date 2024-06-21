using System;
using Godot;

namespace F00F
{
    public static class InputExtensions
    {
        public static bool IsMouseOver(this Control source)
            => source.GetRect().HasPoint(source.GetLocalMousePosition());

        public static bool IsMouseOver(this Popup source)
            => source.Visible && new Rect2(default, source.Size).HasPoint(source.GetMousePosition());

        public static bool MouseRotate(this Node3D source, InputEvent e, float sensitivity, float pitchLimit)
        {
            if (e is not InputEventMouseMotion motion) return false;

            var (x, y) = -motion.Relative * sensitivity;

            var rotation = source.Rotation;

            rotation.X = ClampPitch(rotation.X += y);
            rotation.Y += x;
            rotation.Z = 0;

            source.Rotation = rotation;
            return true;

            float ClampPitch(float x)
                => Mathf.Clamp(x, -pitchLimit, pitchLimit);
        }

        public static bool MouseRotateAround(this Node3D source, InputEvent e, Vector3 target, float sensitivity, float pitchLimit)
        {
            if (e is not InputEventMouseMotion motion) return false;

            var (x, y) = -motion.Relative * sensitivity;

            var transform = source.Transform;

            var hRot = new Quaternion(transform.Basis.Y, x);
            var vRot = new Quaternion(transform.Basis.X, y);
            var rotation = hRot * vRot;

            var curDirection = transform.Origin - target;
            var newDirection = rotation * curDirection;

            source.Transform = new Transform3D(new Basis(rotation), target + newDirection);
            return true;
        }

        public static bool Handle(this Node source, bool handle, Action action = null)
        {
            if (handle)
            {
                action?.Invoke();
                source.SetInputAsHandled();
                return true;
            }

            return false;
        }

        private static void SetInputAsHandled(this Node source)
            => source.GetViewport().SetInputAsHandled();
    }
}
