using System.Diagnostics;
using Godot;

// Usage:
//  Editor.Disable(this);
//  if (Editor.IsEditor) return;

namespace F00F
{
    public static partial class Editor
    {
        public static bool IsEditor => Engine.IsEditorHint();
        public static bool IsEditedSceneRoot<T>(this T source) where T : Node
            => source == EditorInterface.Singleton.GetEditedSceneRoot();

        [Conditional("TOOLS")]
        public static void Disable(Node source, bool process = true, bool physics = true, bool input = true)
        {
            if (IsEditor)
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
