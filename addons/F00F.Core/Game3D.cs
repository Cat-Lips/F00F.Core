using Godot;

namespace F00F;

public partial class Game3D : Node
{
    protected Camera Camera => GetNode<Camera>("Camera");

    protected virtual void OnReady() { }
    public sealed override void _Ready()
    {
        Editor.Disable(this);
        if (Editor.IsEditor) return;

        InitDebugOptions();

        OnReady();

        void InitDebugOptions()
            => DebugDraw.Enabled = GetTree().DebugCollisionsHint;
    }

    public sealed override void _UnhandledKeyInput(InputEvent e)
        => OnUnhandledKeyInput((InputEventKey)e);

    protected virtual bool OnUnhandledKeyInput(InputEventKey e)
    {
        return this.Handle(Input.IsActionJustPressed(MyInput.Quit), Quit)
            || this.Handle(Input.IsActionJustPressed(MyInput.Debug), ToggleDebugOptions);

        void Quit()
            => GetTree().Quit();

        void ToggleDebugOptions()
        {
            var debug = DebugDraw.Enabled;
            DebugDraw.Enabled = !debug;
            GetTree().DebugCollisionsHint = !debug;
        }
    }
}
