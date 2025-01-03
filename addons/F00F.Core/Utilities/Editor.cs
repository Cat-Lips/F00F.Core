﻿#if TOOLS
using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Godot.Collections;

namespace F00F;

public static partial class Editor
{
    public static bool IsSceneRoot(this Node source)
        => source.GetParent() is null || source == EditorInterface.Singleton.GetEditedSceneRoot();

    public static bool IsEditedSceneRoot(this Node source)
        => source == EditorInterface.Singleton.GetEditedSceneRoot();

    public static bool IsSceneRoot<T>(this T source) where T : Node
        => ((Node)source).IsSceneRoot() && source.GetType() == typeof(T);

    #region public override void _ValidateProperty(Dictionary property)

    public static bool SetDefault(Dictionary source, StringName property)
        => SetUsage(source, property, PropertyUsageFlags.Default); // Storage, Editor

    public static bool SetReadOnly(Dictionary source, StringName property)
        => SetUsage(source, property, PropertyUsageFlags.Default, PropertyUsageFlags.ReadOnly);

    public static bool SetDisplayOnly(Dictionary source, StringName property)
        => SetUsage(source, property, PropertyUsageFlags.NoInstanceState, PropertyUsageFlags.Editor);

    public static bool Show(Dictionary source, StringName property, bool @if)
    {
        return @if
            ? SetUsage(source, property, source.Usage() | PropertyUsageFlags.Editor)
            : SetUsage(source, property, source.Usage() & ~PropertyUsageFlags.Editor);
    }

    public static bool Hide(Dictionary source, StringName property, bool @if)
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

    #endregion

    #region public override void _Notification(int what)

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
