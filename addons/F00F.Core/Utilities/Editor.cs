#if TOOLS
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Godot;
using Godot.Collections;

namespace F00F;

public static partial class Editor
{
    #region Validate Property

    // Usage:
    //public sealed override void _ValidateProperty(Dictionary property)
    //{
    //    if (this.IsEditedSceneRoot())
    //    {
    //        if (Editor.EditorOnly(property, PropertyName.Config)) return;
    //    }
    //}

    public static bool SetNone(Dictionary source, StringName property)
        => SetUsage(source, property, PropertyUsageFlags.None);

    public static bool SetDefault(Dictionary source, StringName property)
        => SetUsage(source, property, PropertyUsageFlags.Default); // Storage, Editor

    public static bool SetReadOnly(Dictionary source, StringName property, bool @if = true) => @if
        ? SetUsage(source, property, source.Usage() | PropertyUsageFlags.ReadOnly)
        : SetUsage(source, property, source.Usage() & ~PropertyUsageFlags.ReadOnly);

    public static bool SetDisplayOnly(Dictionary source, StringName property, bool @if = true) => @if
        ? SetUsage(source, property, source.Usage() & ~PropertyUsageFlags.Storage)
        : SetUsage(source, property, source.Usage() | PropertyUsageFlags.Storage);

    public static bool Show(Dictionary source, StringName property, bool @if = true) => @if
        ? SetUsage(source, property, source.Usage() | PropertyUsageFlags.Editor)
        : SetUsage(source, property, source.Usage() & ~PropertyUsageFlags.Editor);

    public static bool Hide(Dictionary source, StringName property, bool @if = true)
        => Show(source, property, !@if);

    private static bool SetUsage(Dictionary source, StringName property, params PropertyUsageFlags[] usage)
    {
        if (source["name"].AsStringName() == property)
        {
            source["usage"] = (int)usage.Aggregate((x, y) => x | y);
            return true;
        }

        return false;
    }

    private static PropertyUsageFlags Usage(this Dictionary source)
        => source["usage"].As<PropertyUsageFlags>();

    public static bool SetEnumHint<T>(Dictionary source, StringName property, params IEnumerable<T> enums) where T : struct, Enum
    {
        if (source["name"].AsStringName() == property)
        {
            source["hint"] = (int)PropertyHint.Enum;
            source["hint_string"] = enums.IsNullOrEmpty() ? default : string.Join(",", enums.Select(x => $"{x}:{Convert.ToInt32(x)}"));
            return true;
        }

        return false;
    }

    public static bool SetEnumHint(Dictionary source, StringName property, params IEnumerable<string> enums)
    {
        if (source["name"].AsStringName() == property)
        {
            source["hint"] = (int)PropertyHint.Enum;
            source["hint_string"] = enums.IsNullOrEmpty() ? default : string.Join(",", enums);
            return true;
        }

        return false;
    }

    #endregion

    #region Load & Save

    //#if TOOLS
    //using Godot;
    //using Godot.Collections;

    //namespace F00F;

    //public partial class <NodeClassName>
    //{
    //    protected void OnEditorSave()
    //        => Editor.DoPreSaveResetField(this, PropertyName.Config);

    //    protected void DoPreSaveReset()
    //    {
    //        Editor.DoPreSaveReset(Mesh, MeshInstance3D.PropertyName.Mesh);
    //        Editor.DoPreSaveReset(Shape, CollisionShape3D.PropertyName.Shape);
    //    }

    //    #region EnableEditorSave

    //    [Export] public bool EnableEditorSave { get; set; } = true;

    //    public sealed override void _ValidateProperty(Dictionary property)
    //    {
    //        if (this.IsEditedSceneRoot())
    //            if (Editor.Hide(property, PropertyName.EnableEditorSave)) return;
    //    }

    //    public sealed override void _Notification(int what)
    //    {
    //        if (Editor.OnPreSave(what))
    //        {
    //            if (!EnableEditorSave || this.IsEditedSceneRoot())
    //                OnEditorSave();
    //            DoPreSaveReset();
    //            return;
    //        }

    //        if (Editor.OnPostSave(what))
    //            Editor.DoPostSaveRestore();
    //    }

    //    #endregion
    //}
    //#endif

    #region PreSave

    public static bool OnPreSave(int what)
    {
        if (what is (int)Node.NotificationEditorPreSave)
        {
            IsSaving = true;
            return true;
        }

        return false;
    }

    public static void DoPreSaveReset(GodotObject source, StringName property, Variant @default = default)
    {
        var value = source.Get(property);
        source.Set(property, @default);
        AddPostSaveAction(() => source.Set(property, value));
    }

    public static void DoPreSaveReset(GodotObject[] source, StringName property, Variant @default = default)
    {
        foreach (var item in source)
            DoPreSaveReset(item, property, @default);
    }

    public static void DoPreSaveResetMeta(GodotObject source, params StringName[] tags)
    {
        foreach (var tag in tags)
        {
            if (source.HasMeta(tag))
            {
                var value = source.GetMeta(tag);
                source.SetMeta(tag, default);
                AddPostSaveAction(() => source.SetMeta(tag, value));
            }
        }
    }

    public static void DoPreSaveResetField<T>(T source, StringName property) where T : GodotObject
    {
        var field = GetBackingField(source, property);

        var value = field.GetValue(source);
        field.SetValue(source, default);
        AddPostSaveAction(() => field.SetValue(source, value));
    }

    public static void DoPreSaveResetOwner(this Node source)
        => source.ForEachChild(x => DoPreSaveReset(x, Node.PropertyName.Owner));

    public static void DoPreSaveResetOwner(this Node source, Func<Node, bool> where)
        => source.ForEachChild(where, x => DoPreSaveReset(x, Node.PropertyName.Owner));

    #endregion

    #region PostSave

    public static bool OnPostSave(int what)
    {
        if (what is (int)Node.NotificationEditorPostSave)
        {
            IsSaving = false;
            return true;
        }

        return false;
    }

    public static void DoPostSaveRestore()
        => ApplyPostSaveActions();

    #endregion

    #region Private

    private const BindingFlags BackingFieldFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;
    private static FieldInfo GetBackingField<T>(T _, StringName property) where T : GodotObject
    {
        var name = $"<{property}>k__BackingField";
        return typeof(T).GetField(name, BackingFieldFlags)
            ?? throw new InvalidOperationException($"Could not find backing field for {property} ({name})");
    }

    private static readonly Stack<Action> PostSaveActions = new();
    private static void AddPostSaveAction(Action action)
        => PostSaveActions.Push(action);

    private static void ApplyPostSaveActions()
    {
        while (PostSaveActions.TryPop(out var action))
            action();
    }

    #endregion

    #endregion

    #region Other

    public static bool OnReady(int what)
        => what is (int)Node.NotificationReady;

    #endregion
}
#endif
