using System;
using System.Diagnostics;
using Godot;

namespace F00F;

public static partial class Editor
{
    public static readonly bool IsEditor = Engine.IsEditorHint();
    public static bool IsSaving { get; private set; }

    //// Usage (for any property that might be reset on save):
    ////  this.Set(ref field, Editor.New(value));
    //public static T New<T>(T value) where T : new()
    //    => IsSaving ? value : value ?? new();

#if TOOLS
    public static bool IsEditedSceneRoot<T>(this T source, bool includeInherited = false) where T : Node
    {
        if (source is null) return false;
        var root = GetEditedSceneRoot();
        return source == root && (includeInherited || root.GetType() == typeof(T));

        static Node GetEditedSceneRoot()
            => IsEditor ? EditorInterface.Singleton.GetEditedSceneRoot() : null;
    }
#else
    public static bool IsEditedSceneRoot(this Node source, bool includeInherited = false)
        => false;
#endif

    // Usage:
    //  Editor.Disable(this);
    //  if (Editor.IsEditor) return;
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

    [Conditional("TOOLS")]
    public static void TrackChanges(Resource source, Action OnChanged)
    {
        if (IsEditor)
            source.Changed += OnChanged;
    }

    [Conditional("TOOLS")]
    public static void TrackChanges(CustomResource source, Action OnChanged)
    {
        if (IsEditor)
            source.Changed += OnChanged;
    }
}
