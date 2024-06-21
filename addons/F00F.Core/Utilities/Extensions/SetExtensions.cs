using System;
using System.Collections.Generic;
using Godot;

namespace F00F;

public static class SetX
{
    #region STATIC

    #region XTRAS

    public static void Set<TValue>(ref TValue field, TValue value, Action onSet, params Action[] xtras) => _Set<object, TValue>(null, ref field, value, onSet: Combine(onSet, xtras));
    public static void Set<TValue>(ref TValue field, TValue value, Action<TValue> onSet, params Action[] xtras) => _Set<object, TValue>(null, ref field, value, onValueSet: onSet, onSet: Combine(xtras));
    public static void Set<TValue>(ref TValue field, TValue value, Action<TValue, TValue> onSet, params Action[] xtras) => _Set<object, TValue>(null, ref field, value, onOldNewSet: onSet, onSet: Combine(xtras));

    public static void Set<TValue>(ref TValue field, TValue value, bool notify, Action onSet, params Action[] xtras) => _Set<object, TValue>(null, ref field, value, notify, onSet: Combine(onSet, xtras));
    public static void Set<TValue>(ref TValue field, TValue value, bool notify, Action<TValue> onSet, params Action[] xtras) => _Set<object, TValue>(null, ref field, value, notify, onValueSet: onSet, onSet: Combine(xtras));
    public static void Set<TValue>(ref TValue field, TValue value, bool notify, Action<TValue, TValue> onSet, params Action[] xtras) => _Set<object, TValue>(null, ref field, value, notify, onOldNewSet: onSet, onSet: Combine(xtras));

    public static void OnChanged<TValue>(ref TValue field, TValue value, Action onChanged, params Action[] xtras) => _Set<object, TValue>(null, ref field, value, onChanged: Combine(onChanged, xtras));
    public static void OnChanged<TValue>(ref TValue field, TValue value, Action<TValue> onChanged, params Action[] xtras) => _Set<object, TValue>(null, ref field, value, onValueChanged: onChanged, onChanged: Combine(xtras));
    public static void OnChanged<TValue>(ref TValue field, TValue value, Action<TValue, TValue> onChanged, params Action[] xtras) => _Set<object, TValue>(null, ref field, value, onOldNewChanged: onChanged, onChanged: Combine(xtras));
    public static void OnChanged<TValue>(ref TValue field, TValue value, Action onChanging, Action onChanged, params Action[] xtras) => _Set<object, TValue>(null, ref field, value, onChanging: onChanging, onChanged: Combine(onChanged, xtras));
    public static void OnChanged<TValue>(ref TValue field, TValue value, Action<TValue> onChanging, Action<TValue> onChanged, params Action[] xtras) => _Set<object, TValue>(null, ref field, value, onValueChanging: onChanging, onValueChanged: onChanged, onChanged: Combine(xtras));
    public static void OnChanged<TValue>(ref TValue field, TValue value, Action<TValue, TValue> onChanging, Action<TValue, TValue> onChanged, params Action[] xtras) => _Set<object, TValue>(null, ref field, value, onOldNewChanging: onChanging, onOldNewChanged: onChanged, onChanged: Combine(xtras));

    public static void OnChanged<TValue>(ref TValue field, TValue value, bool notify, Action onChanged, params Action[] xtras) => _Set<object, TValue>(null, ref field, value, notify, onChanged: Combine(onChanged, xtras));
    public static void OnChanged<TValue>(ref TValue field, TValue value, bool notify, Action<TValue> onChanged, params Action[] xtras) => _Set<object, TValue>(null, ref field, value, notify, onValueChanged: onChanged, onChanged: Combine(xtras));
    public static void OnChanged<TValue>(ref TValue field, TValue value, bool notify, Action<TValue, TValue> onChanged, params Action[] xtras) => _Set<object, TValue>(null, ref field, value, notify, onOldNewChanged: onChanged, onChanged: Combine(xtras));
    public static void OnChanged<TValue>(ref TValue field, TValue value, bool notify, Action onChanging, Action onChanged, params Action[] xtras) => _Set<object, TValue>(null, ref field, value, notify, onChanging: onChanging, onChanged: Combine(onChanged, xtras));
    public static void OnChanged<TValue>(ref TValue field, TValue value, bool notify, Action<TValue> onChanging, Action<TValue> onChanged, params Action[] xtras) => _Set<object, TValue>(null, ref field, value, notify, onValueChanging: onChanging, onValueChanged: onChanged, onChanged: Combine(xtras));
    public static void OnChanged<TValue>(ref TValue field, TValue value, bool notify, Action<TValue, TValue> onChanging, Action<TValue, TValue> onChanged, params Action[] xtras) => _Set<object, TValue>(null, ref field, value, notify, onOldNewChanging: onChanging, onOldNewChanged: onChanged, onChanged: Combine(xtras));

    #endregion

    public static void Set<TValue>(ref TValue field, TValue value, Action onSet = null) => _Set<object, TValue>(null, ref field, value, onSet: onSet);
    public static void Set<TValue>(ref TValue field, TValue value, Action<TValue> onSet) => _Set<object, TValue>(null, ref field, value, onValueSet: onSet);
    public static void Set<TValue>(ref TValue field, TValue value, Action<TValue, TValue> onSet) => _Set<object, TValue>(null, ref field, value, onOldNewSet: onSet);
    public static void OnPreSet<TValue>(ref TValue field, TValue value, Action preSet, Action postSet) => _Set<object, TValue>(null, ref field, value, onChanging: preSet, onSet: postSet);
    public static void OnPreSet<TValue>(ref TValue field, TValue value, Action<TValue> preSet, Action<TValue> postSet) => _Set<object, TValue>(null, ref field, value, onValueChanging: preSet, onValueSet: postSet);
    public static void OnPreSet<TValue>(ref TValue field, TValue value, Action<TValue, TValue> preSet, Action<TValue, TValue> postSet) => _Set<object, TValue>(null, ref field, value, onOldNewChanging: preSet, onOldNewSet: postSet);

    public static void Set<TValue>(ref TValue field, TValue value, bool notify, Action onSet = null) => _Set<object, TValue>(null, ref field, value, notify, onSet: onSet);
    public static void Set<TValue>(ref TValue field, TValue value, bool notify, Action<TValue> onSet) => _Set<object, TValue>(null, ref field, value, notify, onValueSet: onSet);
    public static void Set<TValue>(ref TValue field, TValue value, bool notify, Action<TValue, TValue> onSet) => _Set<object, TValue>(null, ref field, value, notify, onOldNewSet: onSet);
    public static void OnPreSet<TValue>(ref TValue field, TValue value, bool notify, Action preSet, Action postSet) => _Set<object, TValue>(null, ref field, value, notify, onChanging: preSet, onSet: postSet);
    public static void OnPreSet<TValue>(ref TValue field, TValue value, bool notify, Action<TValue> preSet, Action<TValue> postSet) => _Set<object, TValue>(null, ref field, value, notify, onValueChanging: preSet, onValueSet: postSet);
    public static void OnPreSet<TValue>(ref TValue field, TValue value, bool notify, Action<TValue, TValue> preSet, Action<TValue, TValue> postSet) => _Set<object, TValue>(null, ref field, value, notify, onOldNewChanging: preSet, onOldNewSet: postSet);

    public static void OnChanged<TValue>(ref TValue field, TValue value, Action onChanged = null) => _Set<object, TValue>(null, ref field, value, onChanged: onChanged);
    public static void OnChanged<TValue>(ref TValue field, TValue value, Action<TValue> onChanged) => _Set<object, TValue>(null, ref field, value, onValueChanged: onChanged);
    public static void OnChanged<TValue>(ref TValue field, TValue value, Action<TValue, TValue> onChanged) => _Set<object, TValue>(null, ref field, value, onOldNewChanged: onChanged);
    public static void OnChanged<TValue>(ref TValue field, TValue value, Action onChanging, Action onChanged) => _Set<object, TValue>(null, ref field, value, onChanging: onChanging, onChanged: onChanged);
    public static void OnChanged<TValue>(ref TValue field, TValue value, Action<TValue> onChanging, Action<TValue> onChanged) => _Set<object, TValue>(null, ref field, value, onValueChanging: onChanging, onValueChanged: onChanged);
    public static void OnChanged<TValue>(ref TValue field, TValue value, Action<TValue, TValue> onChanging, Action<TValue, TValue> onChanged) => _Set<object, TValue>(null, ref field, value, onOldNewChanging: onChanging, onOldNewChanged: onChanged);

    public static void OnChanged<TValue>(ref TValue field, TValue value, bool notify, Action onChanged = null) => _Set<object, TValue>(null, ref field, value, notify, onChanged: onChanged);
    public static void OnChanged<TValue>(ref TValue field, TValue value, bool notify, Action<TValue> onChanged) => _Set<object, TValue>(null, ref field, value, notify, onValueChanged: onChanged);
    public static void OnChanged<TValue>(ref TValue field, TValue value, bool notify, Action<TValue, TValue> onChanged) => _Set<object, TValue>(null, ref field, value, notify, onOldNewChanged: onChanged);
    public static void OnChanged<TValue>(ref TValue field, TValue value, bool notify, Action onChanging, Action onChanged) => _Set<object, TValue>(null, ref field, value, notify, onChanging: onChanging, onChanged: onChanged);
    public static void OnChanged<TValue>(ref TValue field, TValue value, bool notify, Action<TValue> onChanging, Action<TValue> onChanged) => _Set<object, TValue>(null, ref field, value, notify, onValueChanging: onChanging, onValueChanged: onChanged);
    public static void OnChanged<TValue>(ref TValue field, TValue value, bool notify, Action<TValue, TValue> onChanging, Action<TValue, TValue> onChanged) => _Set<object, TValue>(null, ref field, value, notify, onOldNewChanging: onChanging, onOldNewChanged: onChanged);

    #endregion

    #region XTRAS

    public static void Set<T, TValue>(this T src, ref TValue field, TValue value, Action onSet, params Action[] xtras) where T : class => src._Set(ref field, value, onSet: Combine(onSet, xtras));
    public static void Set<T, TValue>(this T src, ref TValue field, TValue value, Action<TValue> onSet, params Action[] xtras) where T : class => src._Set(ref field, value, onValueSet: onSet, onSet: Combine(xtras));
    public static void Set<T, TValue>(this T src, ref TValue field, TValue value, Action<TValue, TValue> onSet, params Action[] xtras) where T : class => src._Set(ref field, value, onOldNewSet: onSet, onSet: Combine(xtras));

    public static void Set<T, TValue>(this T src, ref TValue field, TValue value, bool notify, Action onSet, params Action[] xtras) where T : class => src._Set(ref field, value, notify, onSet: Combine(onSet, xtras));
    public static void Set<T, TValue>(this T src, ref TValue field, TValue value, bool notify, Action<TValue> onSet, params Action[] xtras) where T : class => src._Set(ref field, value, notify, onValueSet: onSet, onSet: Combine(xtras));
    public static void Set<T, TValue>(this T src, ref TValue field, TValue value, bool notify, Action<TValue, TValue> onSet, params Action[] xtras) where T : class => src._Set(ref field, value, notify, onOldNewSet: onSet, onSet: Combine(xtras));

    public static void OnChanged<T, TValue>(this T src, ref TValue field, TValue value, Action onChanged, params Action[] xtras) where T : class => src._Set(ref field, value, onChanged: Combine(onChanged, xtras));
    public static void OnChanged<T, TValue>(this T src, ref TValue field, TValue value, Action<TValue> onChanged, params Action[] xtras) where T : class => src._Set(ref field, value, onValueChanged: onChanged, onChanged: Combine(xtras));
    public static void OnChanged<T, TValue>(this T src, ref TValue field, TValue value, Action<TValue, TValue> onChanged, params Action[] xtras) where T : class => src._Set(ref field, value, onOldNewChanged: onChanged, onChanged: Combine(xtras));
    public static void OnChanged<T, TValue>(this T src, ref TValue field, TValue value, Action onChanging, Action onChanged, params Action[] xtras) where T : class => src._Set(ref field, value, onChanging: onChanging, onChanged: Combine(onChanged, xtras));
    public static void OnChanged<T, TValue>(this T src, ref TValue field, TValue value, Action<TValue> onChanging, Action<TValue> onChanged, params Action[] xtras) where T : class => src._Set(ref field, value, onValueChanging: onChanging, onValueChanged: onChanged, onChanged: Combine(xtras));
    public static void OnChanged<T, TValue>(this T src, ref TValue field, TValue value, Action<TValue, TValue> onChanging, Action<TValue, TValue> onChanged, params Action[] xtras) where T : class => src._Set(ref field, value, onOldNewChanging: onChanging, onOldNewChanged: onChanged, onChanged: Combine(xtras));

    public static void OnChanged<T, TValue>(this T src, ref TValue field, TValue value, bool notify, Action onChanged, params Action[] xtras) where T : class => src._Set(ref field, value, notify, onChanged: Combine(onChanged, xtras));
    public static void OnChanged<T, TValue>(this T src, ref TValue field, TValue value, bool notify, Action<TValue> onChanged, params Action[] xtras) where T : class => src._Set(ref field, value, notify, onValueChanged: onChanged, onChanged: Combine(xtras));
    public static void OnChanged<T, TValue>(this T src, ref TValue field, TValue value, bool notify, Action<TValue, TValue> onChanged, params Action[] xtras) where T : class => src._Set(ref field, value, notify, onOldNewChanged: onChanged, onChanged: Combine(xtras));
    public static void OnChanged<T, TValue>(this T src, ref TValue field, TValue value, bool notify, Action onChanging, Action onChanged, params Action[] xtras) where T : class => src._Set(ref field, value, notify, onChanging: onChanging, onChanged: Combine(onChanged, xtras));
    public static void OnChanged<T, TValue>(this T src, ref TValue field, TValue value, bool notify, Action<TValue> onChanging, Action<TValue> onChanged, params Action[] xtras) where T : class => src._Set(ref field, value, notify, onValueChanging: onChanging, onValueChanged: onChanged, onChanged: Combine(xtras));
    public static void OnChanged<T, TValue>(this T src, ref TValue field, TValue value, bool notify, Action<TValue, TValue> onChanging, Action<TValue, TValue> onChanged, params Action[] xtras) where T : class => src._Set(ref field, value, notify, onOldNewChanging: onChanging, onOldNewChanged: onChanged, onChanged: Combine(xtras));

    private static Action Combine(Action[] xtras)
        => Combine(null, xtras);

    private static Action Combine(Action main, Action[] xtras)
    {
        foreach (var x in xtras)
            main += x;
        return main;
    }

    #endregion

    #region MAIN

    public static void Set<T, TValue>(this T src, ref TValue field, TValue value, Action onSet = null) where T : class => src._Set(ref field, value, onSet: onSet);
    public static void Set<T, TValue>(this T src, ref TValue field, TValue value, Action<TValue> onSet) where T : class => src._Set(ref field, value, onValueSet: onSet);
    public static void Set<T, TValue>(this T src, ref TValue field, TValue value, Action<TValue, TValue> onSet) where T : class => src._Set(ref field, value, onOldNewSet: onSet);
    public static void OnPreSet<T, TValue>(this T src, ref TValue field, TValue value, Action preSet, Action postSet) where T : class => src._Set(ref field, value, onChanging: preSet, onSet: postSet);
    public static void OnPreSet<T, TValue>(this T src, ref TValue field, TValue value, Action<TValue> preSet, Action<TValue> postSet) where T : class => src._Set(ref field, value, onValueChanging: preSet, onValueSet: postSet);
    public static void OnPreSet<T, TValue>(this T src, ref TValue field, TValue value, Action<TValue, TValue> preSet, Action<TValue, TValue> postSet) where T : class => src._Set(ref field, value, onOldNewChanging: preSet, onOldNewSet: postSet);

    public static void Set<T, TValue>(this T src, ref TValue field, TValue value, bool notify, Action onSet = null) where T : class => src._Set(ref field, value, notify, onSet: onSet);
    public static void Set<T, TValue>(this T src, ref TValue field, TValue value, bool notify, Action<TValue> onSet) where T : class => src._Set(ref field, value, notify, onValueSet: onSet);
    public static void Set<T, TValue>(this T src, ref TValue field, TValue value, bool notify, Action<TValue, TValue> onSet) where T : class => src._Set(ref field, value, notify, onOldNewSet: onSet);
    public static void OnPreSet<T, TValue>(this T src, ref TValue field, TValue value, bool notify, Action preSet, Action postSet) where T : class => src._Set(ref field, value, notify, onChanging: preSet, onSet: postSet);
    public static void OnPreSet<T, TValue>(this T src, ref TValue field, TValue value, bool notify, Action<TValue> preSet, Action<TValue> postSet) where T : class => src._Set(ref field, value, notify, onValueChanging: preSet, onValueSet: postSet);
    public static void OnPreSet<T, TValue>(this T src, ref TValue field, TValue value, bool notify, Action<TValue, TValue> preSet, Action<TValue, TValue> postSet) where T : class => src._Set(ref field, value, notify, onOldNewChanging: preSet, onOldNewSet: postSet);

    public static void OnChanged<T, TValue>(this T src, ref TValue field, TValue value, Action onChanged = null) where T : class => src._Set(ref field, value, onChanged: onChanged);
    public static void OnChanged<T, TValue>(this T src, ref TValue field, TValue value, Action<TValue> onChanged) where T : class => src._Set(ref field, value, onValueChanged: onChanged);
    public static void OnChanged<T, TValue>(this T src, ref TValue field, TValue value, Action<TValue, TValue> onChanged) where T : class => src._Set(ref field, value, onOldNewChanged: onChanged);
    public static void OnChanged<T, TValue>(this T src, ref TValue field, TValue value, Action onChanging, Action onChanged) where T : class => src._Set(ref field, value, onChanging: onChanging, onChanged: onChanged);
    public static void OnChanged<T, TValue>(this T src, ref TValue field, TValue value, Action<TValue> onChanging, Action<TValue> onChanged) where T : class => src._Set(ref field, value, onValueChanging: onChanging, onValueChanged: onChanged);
    public static void OnChanged<T, TValue>(this T src, ref TValue field, TValue value, Action<TValue, TValue> onChanging, Action<TValue, TValue> onChanged) where T : class => src._Set(ref field, value, onOldNewChanging: onChanging, onOldNewChanged: onChanged);

    public static void OnChanged<T, TValue>(this T src, ref TValue field, TValue value, bool notify, Action onChanged = null) where T : class => src._Set(ref field, value, notify, onChanged: onChanged);
    public static void OnChanged<T, TValue>(this T src, ref TValue field, TValue value, bool notify, Action<TValue> onChanged) where T : class => src._Set(ref field, value, notify, onValueChanged: onChanged);
    public static void OnChanged<T, TValue>(this T src, ref TValue field, TValue value, bool notify, Action<TValue, TValue> onChanged) where T : class => src._Set(ref field, value, notify, onOldNewChanged: onChanged);
    public static void OnChanged<T, TValue>(this T src, ref TValue field, TValue value, bool notify, Action onChanging, Action onChanged) where T : class => src._Set(ref field, value, notify, onChanging: onChanging, onChanged: onChanged);
    public static void OnChanged<T, TValue>(this T src, ref TValue field, TValue value, bool notify, Action<TValue> onChanging, Action<TValue> onChanged) where T : class => src._Set(ref field, value, notify, onValueChanging: onChanging, onValueChanged: onChanged);
    public static void OnChanged<T, TValue>(this T src, ref TValue field, TValue value, bool notify, Action<TValue, TValue> onChanging, Action<TValue, TValue> onChanged) where T : class => src._Set(ref field, value, notify, onOldNewChanging: onChanging, onOldNewChanged: onChanged);

    #endregion

    #region Core

    private static void _Set<T, TValue>(
        this T src, ref TValue field,
        TValue value, bool notify = false,
        Action onChanging = null, Action onChanged = null,
        Action<TValue> onValueChanging = null, Action<TValue> onValueChanged = null,
        Action<TValue, TValue> onOldNewChanging = null, Action<TValue, TValue> onOldNewChanged = null,
        Action onSet = null, Action<TValue> onValueSet = null, Action<TValue, TValue> onOldNewSet = null) where T : class
    {
        if (Equal(ref field, ref value))
            return;

        var old = field;

        if (IsRefType())
            OnRefTypeChanging();
        OnValueChanging();

        field = value;

        if (IsRefType())
            OnRefTypeChanged();
        OnValueChanged();

        OnValueSet();
        EmitChanged();
        NotifyEditor();

        void OnValueSet()
        {
            onOldNewSet?.Invoke(old, value);
            onValueSet?.Invoke(value);
            onSet?.Invoke();
        }

        void OnValueChanged()
        {
            onOldNewChanged?.Invoke(old, value);
            onValueChanged?.Invoke(value);
            onChanged?.Invoke();
        }

        void OnValueChanging()
        {
            onOldNewChanging?.Invoke(old, value);
            onValueChanging?.Invoke(old);
            onChanging?.Invoke();
        }

        void EmitChanged()
        {
            if (src is Resource res)
                res.EmitChanged();
        }

        void NotifyEditor()
        {
            if (notify && src is GodotObject go)
                go.NotifyPropertyListChanged();
        }

        static bool IsRefType()
            => !typeof(TValue).IsValueType;

        static bool Equal(ref TValue field, ref TValue value)
            => EqualityComparer<TValue>.Default.Equals(field, value);

        void OnRefTypeChanging()
        {
            if (old is Resource res)
                res.SafeDisconnect(Resource.SignalName.Changed, OnChildChanged);
            else if (old is Resource[] resArray)
                resArray.ForEach(x => x?.SafeDisconnect(Resource.SignalName.Changed, OnChildChanged));
        }

        void OnRefTypeChanged()
        {
            if (value is Resource res)
                res.SafeConnect(Resource.SignalName.Changed, OnChildChanged);
            else if (value is Resource[] resArray)
                resArray.ForEach(x => x?.SafeConnect(Resource.SignalName.Changed, OnChildChanged));
        }

        void OnChildChanged()
        {
            EmitChanged();
            OnValueChanged();
        }
    }

    #endregion
}
