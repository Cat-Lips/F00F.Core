using Godot;

namespace F00F;

[Tool]
public partial class MyInput : Node
{
    public static MyInput Instance { get; private set; }

    #region Private

    public MyInput()
    {
        Instance ??= this;
        ProcessMode = ProcessModeEnum.Always;
        MouseVisibilityChanged += OnMouseVisibilityChanged;

        void OnMouseVisibilityChanged()
        {
            if (!MouseVisible)
                GetViewport().GuiReleaseFocus();
        }
    }

    #endregion
}
