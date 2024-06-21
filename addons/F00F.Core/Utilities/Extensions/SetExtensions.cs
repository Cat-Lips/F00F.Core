using System;
using Godot;

namespace F00F;

public static class SetExtensions
{
    public static void Set<T, TValue>(this T src, ref TValue field, TValue value, params Action[] onSet) => src._Set(ref field, value, onSet: onSet);
    public static void Set<T, TValue>(this T src, ref TValue field, TValue value, Action<TValue> onSetValue, params Action[] onSet) => src._Set(ref field, value, onSetValue: onSetValue, onSet: onSet);
    public static void Set<T, TValue>(this T src, ref TValue field, TValue value, Action<TValue, TValue> onSetOldNew, params Action[] onSet) => src._Set(ref field, value, onSetOldNew: onSetOldNew, onSet: onSet);

    public static void Set<T, TValue>(this T src, ref TValue field, TValue value, bool notify, params Action[] onSet) => src._Set(ref field, value, notify, onSet: onSet);
    public static void Set<T, TValue>(this T src, ref TValue field, TValue value, bool notify, Action<TValue> onSetValue, params Action[] onSet) => src._Set(ref field, value, notify, onSetValue: onSetValue, onSet: onSet);
    public static void Set<T, TValue>(this T src, ref TValue field, TValue value, bool notify, Action<TValue, TValue> onSetOldNew, params Action[] onSet) => src._Set(ref field, value, notify, onSetOldNew: onSetOldNew, onSet: onSet);

    private static void _Set<T, TValue>(this T src, ref TValue field, TValue value, bool notify = false,
        Action[] onSet = null, Action<TValue> onSetValue = null, Action<TValue, TValue> onSetOldNew = null)
    {
        if (Equals(field, value))
            return;

        var old = field;
        field = value;

        OnSet();
        OnChanged();
        NotifyEditor();

        void OnSet()
        {
            onSet?.ForEach(x => x?.Invoke());
            onSetValue?.Invoke(value);
            onSetOldNew?.Invoke(old, value);

            if (value is Resource res)
                res.SafeConnect(Resource.SignalName.Changed, OnChanged);
            else if (value is Resource[] resArray)
                resArray.ForEach(x => x?.SafeConnect(Resource.SignalName.Changed, OnChanged));
        }

        void OnChanged()
            => (src as Resource)?.EmitSignal(Resource.SignalName.Changed);

        void NotifyEditor()
        {
            if (notify)
                (src as GodotObject)?.NotifyPropertyListChanged();
        }
    }
}
