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
    #region Property Settings

    // Usage:
    //public sealed override void _ValidateProperty(Dictionary property)
    //{
    //    if (this.IsEditedSceneRoot())
    //    {
    //        if (Editor.SetDisplayOnly(property, PropertyName.Config)) return;
    //        if (Editor.SetDisplayOnly(property, PropertyName.Target)) return;
    //        if (Editor.SetDisplayOnly(property, PropertyName.SelectMode)) return;
    //    }
    //}

    public static bool SetDefault(Dictionary source, StringName property)
        => SetUsage(source, property, PropertyUsageFlags.Default); // Storage, Editor

    public static bool SetReadOnly(Dictionary source, StringName property)
        => SetUsage(source, property, PropertyUsageFlags.Default, PropertyUsageFlags.ReadOnly);

    public static bool SetDisplayOnly(Dictionary source, StringName property)
        => SetUsage(source, property, PropertyUsageFlags.NoInstanceState, PropertyUsageFlags.Editor);

    public static bool Show(Dictionary source, StringName property, bool @if = true)
    {
        return @if
            ? SetUsage(source, property, source.Usage() | PropertyUsageFlags.Editor)
            : SetUsage(source, property, source.Usage() & ~PropertyUsageFlags.Editor);
    }

    public static bool Hide(Dictionary source, StringName property, bool @if = true)
    {
        return @if
            ? SetUsage(source, property, source.Usage() & ~PropertyUsageFlags.Editor)
            : SetUsage(source, property, source.Usage() | PropertyUsageFlags.Editor);
    }

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

    public static bool SetEnumHint<T>(Dictionary source, StringName property, params T[] enums) where T : struct, Enum
    {
        if (source["name"].AsStringName() == property)
        {
            source["hint"] = (int)PropertyHint.Enum;
            source["hint_string"] = enums.IsNullOrEmpty() ? default : string.Join(",", enums.Select(x => $"{x}:{(int)(object)x}"));
            return true;
        }

        return false;
    }

    #endregion

    #region Load & Save

    // Usage:
    //protected virtual void OnEditorSave() { }
    //public sealed override void _Notification(int what)
    //{
    //    if (Editor.OnPreSave(what))
    //    {
    //        if (this.IsEditedSceneRoot())
    //            Editor.DoPreSaveReset(Camera, Camera.PropertyName.Config);
    //        OnEditorSave();
    //        return;
    //    }

    //    if (Editor.OnPostSave(what))
    //        Editor.DoPostSaveRestore();
    //}

    private static readonly Stack<Action> PostSaveActions = new();

    public static bool OnReady(int what)
        => IsEditor && (long)what is Node.NotificationReady;

    public static bool OnPreSave(int what)
    {
        if ((long)what is Node.NotificationEditorPreSave)
        {
            IsSaving = true;
            return true;
        }

        return false;
    }

    public static bool OnPostSave(int what)
    {
        if ((long)what is Node.NotificationEditorPostSave)
        {
            IsSaving = false;
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

    public static void DoPreSaveReset<T>(T value, Action<T> set, T @default = default)
    {
        set(@default);
        AddPostSaveAction(() => set(value));
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
            var value = source.GetMeta(tag);
            source.SetMeta(tag, default);
            AddPostSaveAction(() => source.SetMeta(tag, value));
        }
    }

    private const BindingFlags BackingFieldFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;
    public static void DoPreSaveResetField<T>(T source, StringName property) where T : GodotObject
    {
        var name = $"<{property}>k__BackingField";
        var field = typeof(T).GetField(name, BackingFieldFlags)
            ?? throw new InvalidOperationException($"Could not find backing field for {property} ({name})");
        //Debug.Assert(field is not null, $"Could not find backing field for {property} ({name})");

        var value = field.GetValue(source);
        field.SetValue(source, default);
        AddPostSaveAction(() => field.SetValue(source, value));
    }

    public static void AddPostSaveAction(Action action)
        => PostSaveActions.Push(action);

    public static void DoPostSaveRestore()
        => ApplyPostSaveActions();

    public static void ApplyPostSaveActions()
    {
        while (PostSaveActions.TryPop(out var restore))
            restore();
    }

    #endregion
}
#endif
