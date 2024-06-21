#if TOOLS
using Godot.Collections;

namespace F00F
{
    public partial class Camera
    {
        public override void _ValidateProperty(Dictionary property)
        {
            if (this.IsSceneRoot())
            {
                if (Editor.SetDisplayOnly(property, PropertyName.Input)) return;
                if (Editor.SetDisplayOnly(property, PropertyName.Config)) return;
                if (Editor.SetDisplayOnly(property, PropertyName.Target)) return;
                if (Editor.SetDisplayOnly(property, PropertyName.SelectMode)) return;
            }
        }
    }
}
#endif
