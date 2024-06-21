using System.Diagnostics;
using Godot;

namespace F00F
{
    public static partial class Editor
    {
        [Conditional("TOOLS")]
        public static void Disable(Node source, bool process = true, bool physics = true, bool input = true)
        {
            if (Engine.IsEditorHint())
            {
                if (process) source.SetProcess(false);
                if (input) source.SetProcessInput(false);
                if (physics) source.SetPhysicsProcess(false);
                if (input) source.SetProcessUnhandledInput(false);
                if (input) source.SetProcessUnhandledKeyInput(false);
            }
        }
    }
}
