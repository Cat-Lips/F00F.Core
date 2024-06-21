using System;
using Godot;

namespace F00F;

public static class SetExtensions
{
    public static void Set<T, TValue>(this T src, ref TValue field, TValue value, params Action[] onChanged) => src._Set(ref field, value, onChanged: onChanged);
    public static void Set<T, TValue>(this T src, ref TValue field, TValue value, Action<TValue> onValueChanged, params Action[] onChanged) => src._Set(ref field, value, onValueChanged: onValueChanged, onChanged: onChanged);
    public static void Set<T, TValue>(this T src, ref TValue field, TValue value, Action<TValue, TValue> onOldNewChanged, params Action[] onChanged) => src._Set(ref field, value, onOldNewChanged: onOldNewChanged, onChanged: onChanged);

    public static void Set<T, TValue>(this T src, ref TValue field, TValue value, bool notify, params Action[] onChanged) => src._Set(ref field, value, notify, onChanged: onChanged);
    public static void Set<T, TValue>(this T src, ref TValue field, TValue value, bool notify, Action<TValue> onValueChanged, params Action[] onChanged) => src._Set(ref field, value, notify, onValueChanged: onValueChanged, onChanged: onChanged);
    public static void Set<T, TValue>(this T src, ref TValue field, TValue value, bool notify, Action<TValue, TValue> onOldNewChanged, params Action[] onChanged) => src._Set(ref field, value, notify, onOldNewChanged: onOldNewChanged, onChanged: onChanged);

    public static void _Set<T, TValue>(this T src, ref TValue field, TValue value, bool notify = false,
        Action[] onChanged = null, Action<TValue> onValueChanged = null, Action<TValue, TValue> onOldNewChanged = null)
    {
        if (Equals(field, value))
            return;

        var old = field;

        UnSet(field);
        field = value;

        OnSet();
        OnChanged();
        NotifyEditor();

        void OnSet()
        {
            if (value is Resource res)
                res.SafeConnect(Resource.SignalName.Changed, OnChanged);
            else if (value is Resource[] resArray)
                resArray.ForEach(x => x?.SafeConnect(Resource.SignalName.Changed, OnChanged));
        }

        void UnSet(in TValue field)
        {
            if (field is Resource res)
                res.SafeDisconnect(Resource.SignalName.Changed, OnChanged);
            else if (field is Resource[] resArray)
                resArray.ForEach(x => x?.SafeDisconnect(Resource.SignalName.Changed, OnChanged));
        }

        void OnChanged()
        {
            (src as Resource)?.EmitChanged();
            onChanged?.ForEach(x => x?.Invoke());
            onValueChanged?.Invoke(value);
            onOldNewChanged?.Invoke(old, value);
        }

        void NotifyEditor()
        {
            if (notify)
                (src as GodotObject)?.NotifyPropertyListChanged();
        }
    }
}
