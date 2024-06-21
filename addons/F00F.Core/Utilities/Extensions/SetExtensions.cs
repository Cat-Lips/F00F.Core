using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Godot;

namespace F00F;

public static class SetExtensions
{
    public static void Set<T, TValue>(this T src, ref TValue field, TValue value, params Action[] onSet) where T : class => src._Set(ref field, value, onSet: onSet);
    public static void Set<T, TValue>(this T src, ref TValue field, TValue value, Action<TValue> onValueSet, params Action[] onSet) where T : class => src._Set(ref field, value, onValueSet: onValueSet, onSet: onSet);
    public static void Set<T, TValue>(this T src, ref TValue field, TValue value, Action<TValue, TValue> onOldNewSet, params Action[] onSet) where T : class => src._Set(ref field, value, onOldNewSet: onOldNewSet, onSet: onSet);

    public static void Set<T, TValue>(this T src, ref TValue field, TValue value, bool notify, params Action[] onSet) where T : class => src._Set(ref field, value, notify, onSet: onSet);
    public static void Set<T, TValue>(this T src, ref TValue field, TValue value, bool notify, Action<TValue> onValueSet, params Action[] onSet) where T : class => src._Set(ref field, value, notify, onValueSet: onValueSet, onSet: onSet);
    public static void Set<T, TValue>(this T src, ref TValue field, TValue value, bool notify, Action<TValue, TValue> onOldNewSet, params Action[] onSet) where T : class => src._Set(ref field, value, notify, onOldNewSet: onOldNewSet, onSet: onSet);

    public static void OnChanged<T, TValue>(this T src, ref TValue field, TValue value, params Action[] onChanged) where T : class => src._Set(ref field, value, NotifyOnChanged: true, onSet: onChanged);

    public static void _Set<T, TValue>(this T src, ref TValue field, TValue value, bool notify = false, bool NotifyOnChanged = false,
        Action[] onSet = null, Action<TValue> onValueSet = null, Action<TValue, TValue> onOldNewSet = null) where T : class
    {
        if (EqualityComparer<TValue>.Default.Equals(field, value))
            return;

        var old = field;

        UnSet(ref field);
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

            if (IsRes(ref value, out var res))
                res.SafeConnect(Resource.SignalName.Changed, OnChanged);
            else if (IsResArray(ref value, out var resArray))
                resArray.ForEach(x => x?.SafeConnect(Resource.SignalName.Changed, OnChanged));
        }

        void UnSet(ref TValue field)
        {
            if (IsRes(ref field, out var res))
                res.SafeDisconnect(Resource.SignalName.Changed, OnChanged);
            else if (IsResArray(ref field, out var resArray))
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

        static bool IsRes(ref TValue v, out Resource res)
        {
            if (typeof(Resource).IsAssignableFrom(typeof(TValue)))
            {
                res = Unsafe.As<TValue, Resource>(ref v);
                return true;
            }
            else
            {
                res = null;
                return false;
            }
        }

        static bool IsResArray(ref TValue v, out Resource[] resArray)
        {
            var t = typeof(TValue);
            if (t.IsArray && typeof(Resource).IsAssignableFrom(t.GetElementType()))
            {
                resArray = Unsafe.As<TValue, Resource[]>(ref v);
                return true;
            }
            else
            {
                resArray = null;
                return false;
            }
        }
    }
}
