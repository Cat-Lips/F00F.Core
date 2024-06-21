using Godot;

namespace F00F
{
    [Tool, GlobalClass]
    public partial class CameraInput : Resource
    {
        protected internal virtual Vector3 GetMovement() => new(
            Input.GetAxis(MyInput.Left, MyInput.Right),
            Input.GetAxis(MyInput.Down, MyInput.Up),
            Input.GetAxis(MyInput.Forward, MyInput.Back));

        protected internal virtual bool Fast1()
            => Input.IsActionPressed(MyInput.Fast1);

        protected internal virtual bool Fast2()
            => Input.IsActionPressed(MyInput.Fast2);

        protected internal virtual bool Fast3()
            => Input.IsActionPressed(MyInput.Fast3);

        protected internal virtual bool SelectJustPressed()
            => Input.IsActionJustPressed(MyInput.Select);

        protected internal virtual bool ToggleSelectMode()
            => Input.IsActionJustPressed(MyInput.ToggleSelectMode);

        protected internal virtual bool ResetCameraPosition()
            => Input.IsActionJustPressed(MyInput.Reset);
    }
}
