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
        var handled = this.Handle(e.IsActionPressed(MyInput.Quit), QuitGame);
#if TOOLS
        handled |= e.CtrlPressed && this.Handle(e.IsActionPressed(MyInput.Debug), ToggleDebug);
#endif
        return handled;

        void QuitGame()
            => GetTree().Quit();
#if TOOLS
        void ToggleDebug()
        {
            var enable = GetViewport().DebugDraw is Viewport.DebugDrawEnum.Disabled;
            GetViewport().DebugDraw = enable ? Viewport.DebugDrawEnum.Wireframe : Viewport.DebugDrawEnum.Disabled;
            DebugDraw.Enabled = enable;
        }
#endif
    }
}
