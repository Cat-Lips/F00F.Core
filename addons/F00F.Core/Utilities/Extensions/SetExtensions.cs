using System;
using Godot;

namespace F00F;

public static class SetExtensions
{
    public static void Set<T, TValue>(this T src, ref TValue field, TValue value, params Action[] onSet) => src._Set(ref field, value, onSet: onSet);
    public static void Set<T, TValue>(this T src, ref TValue field, TValue value, Action<TValue> onValueSet, params Action[] onSet) => src._Set(ref field, value, onValueSet: onValueSet, onSet: onSet);
    public static void Set<T, TValue>(this T src, ref TValue field, TValue value, Action<TValue, TValue> onOldNewSet, params Action[] onSet) => src._Set(ref field, value, onOldNewSet: onOldNewSet, onSet: onSet);

    public static void Set<T, TValue>(this T src, ref TValue field, TValue value, bool notify, params Action[] onSet) => src._Set(ref field, value, notify, onSet: onSet);
    public static void Set<T, TValue>(this T src, ref TValue field, TValue value, bool notify, Action<TValue> onValueSet, params Action[] onSet) => src._Set(ref field, value, notify, onValueSet: onValueSet, onSet: onSet);
    public static void Set<T, TValue>(this T src, ref TValue field, TValue value, bool notify, Action<TValue, TValue> onOldNewSet, params Action[] onSet) => src._Set(ref field, value, notify, onOldNewSet: onOldNewSet, onSet: onSet);

    public static void OnChanged<T, TValue>(this T src, ref TValue field, TValue value, params Action[] onChanged) => src._Set(ref field, value, NotifyOnChanged: true, onSet: onChanged);

    public static void _Set<T, TValue>(this T src, ref TValue field, TValue value, bool notify = false, bool NotifyOnChanged = false,
        Action[] onSet = null, Action<TValue> onValueSet = null, Action<TValue, TValue> onOldNewSet = null)
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
            if (!NotifyOnChanged)
                onSet?.ForEach(x => x?.Invoke());
            onValueSet?.Invoke(value);
            onOldNewSet?.Invoke(old, value);

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

            if (NotifyOnChanged)
                onSet?.ForEach(x => x?.Invoke());
        }

        void NotifyEditor()
        {
            if (notify)
                (src as GodotObject)?.NotifyPropertyListChanged();
        }
    }
}
