using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Godot;

namespace F00F
{
    public static class SetExtensions
    {
        public static void Set<T, TValue>(this T src, ref TValue field, TValue value, params Action[] onSet) => src._Set(ref field, value, onSet: onSet);
        public static void Set<T, TValue>(this T src, ref TValue field, TValue value, Action<TValue> onSetValue, params Action[] onSet) => src._Set(ref field, value, onSetValue: onSetValue, onSet: onSet);
        public static void Set<T, TValue>(this T src, ref TValue field, TValue value, Action<string> onSetName, [CallerMemberName] string caller = null) => src._Set(ref field, value, onSetName: onSetName, caller: caller);
        public static void Set<T, TValue>(this T src, ref TValue field, TValue value, Action<string> onSetName, Action onSet, [CallerMemberName] string caller = null) => src._Set(ref field, value, onSetName: onSetName, onSet: [onSet], caller: caller);
        public static void Set<T, TValue>(this T src, ref TValue field, TValue value, Action<string> onSetName, Action onSet1, Action onSet2, [CallerMemberName] string caller = null) => src._Set(ref field, value, onSetName: onSetName, onSet: [onSet1, onSet2], caller: caller);
        public static void Set<T, TValue>(this T src, ref TValue field, TValue value, Action<string> onSetName, Action onSet1, Action onSet2, Action onSet3, [CallerMemberName] string caller = null) => src._Set(ref field, value, onSetName: onSetName, onSet: [onSet1, onSet2, onSet3], caller: caller);
        public static void Set<T, TValue>(this T src, ref TValue field, TValue value, Action<string> onSetName, Action<TValue> onSetValue, [CallerMemberName] string caller = null) => src._Set(ref field, value, onSetName: onSetName, onSetValue: onSetValue, caller: caller);
        public static void Set<T, TValue>(this T src, ref TValue field, TValue value, Action<string> onSetName, Action<TValue> onSetValue, Action onSet, [CallerMemberName] string caller = null) => src._Set(ref field, value, onSetName: onSetName, onSetValue: onSetValue, onSet: [onSet], caller: caller);
        public static void Set<T, TValue>(this T src, ref TValue field, TValue value, Action<string> onSetName, Action<TValue> onSetValue, Action onSet1, Action onSet2, [CallerMemberName] string caller = null) => src._Set(ref field, value, onSetName: onSetName, onSetValue: onSetValue, onSet: [onSet1, onSet2], caller: caller);
        public static void Set<T, TValue>(this T src, ref TValue field, TValue value, Action<string> onSetName, Action<TValue> onSetValue, Action onSet1, Action onSet2, Action onSet3, [CallerMemberName] string caller = null) => src._Set(ref field, value, onSetName: onSetName, onSetValue: onSetValue, onSet: [onSet1, onSet2, onSet3], caller: caller);

        public static void Set<T, TValue>(this T src, ref TValue field, TValue value, bool notify, params Action[] onSet) => src._Set(ref field, value, notify, onSet: onSet);
        public static void Set<T, TValue>(this T src, ref TValue field, TValue value, bool notify, Action<TValue> onSetValue, params Action[] onSet) => src._Set(ref field, value, notify, onSetValue: onSetValue, onSet: onSet);
        public static void Set<T, TValue>(this T src, ref TValue field, TValue value, bool notify, Action<string> onSetName, [CallerMemberName] string caller = null) => src._Set(ref field, value, notify, onSetName: onSetName, caller: caller);
        public static void Set<T, TValue>(this T src, ref TValue field, TValue value, bool notify, Action<string> onSetName, Action onSet, [CallerMemberName] string caller = null) => src._Set(ref field, value, notify, onSetName: onSetName, onSet: [onSet], caller: caller);
        public static void Set<T, TValue>(this T src, ref TValue field, TValue value, bool notify, Action<string> onSetName, Action onSet1, Action onSet2, [CallerMemberName] string caller = null) => src._Set(ref field, value, notify, onSetName: onSetName, onSet: [onSet1, onSet2], caller: caller);
        public static void Set<T, TValue>(this T src, ref TValue field, TValue value, bool notify, Action<string> onSetName, Action onSet1, Action onSet2, Action onSet3, [CallerMemberName] string caller = null) => src._Set(ref field, value, notify, onSetName: onSetName, onSet: [onSet1, onSet2, onSet3], caller: caller);
        public static void Set<T, TValue>(this T src, ref TValue field, TValue value, bool notify, Action<string> onSetName, Action<TValue> onSetValue, [CallerMemberName] string caller = null) => src._Set(ref field, value, notify, onSetName: onSetName, onSetValue: onSetValue, caller: caller);
        public static void Set<T, TValue>(this T src, ref TValue field, TValue value, bool notify, Action<string> onSetName, Action<TValue> onSetValue, Action onSet, [CallerMemberName] string caller = null) => src._Set(ref field, value, notify, onSetName: onSetName, onSetValue: onSetValue, onSet: [onSet], caller: caller);
        public static void Set<T, TValue>(this T src, ref TValue field, TValue value, bool notify, Action<string> onSetName, Action<TValue> onSetValue, Action onSet1, Action onSet2, [CallerMemberName] string caller = null) => src._Set(ref field, value, notify, onSetName: onSetName, onSetValue: onSetValue, onSet: [onSet1, onSet2], caller: caller);
        public static void Set<T, TValue>(this T src, ref TValue field, TValue value, bool notify, Action<string> onSetName, Action<TValue> onSetValue, Action onSet1, Action onSet2, Action onSet3, [CallerMemberName] string caller = null) => src._Set(ref field, value, notify, onSetName: onSetName, onSetValue: onSetValue, onSet: [onSet1, onSet2, onSet3], caller: caller);

        private static void _Set<T, TValue>(this T src, ref TValue field, TValue value, bool notify = false, string caller = null,
            Action[] onSet = null, Action<string> onSetName = null, Action<TValue> onSetValue = null)
        {
            if (!Equals(field, value))
            {
                field = value;

                OnSet();
                OnChanged();
                NotifyEditor();
            }

            void OnSet()
            {
                onSet?.ForEach(x => x?.Invoke());
                onSetName?.Invoke(caller);
                onSetValue?.Invoke(value);
                Debug.Assert(onSetName is null == caller is null);
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
}
