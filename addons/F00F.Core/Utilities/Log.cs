using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using Godot;
using Path = System.IO.Path;

namespace F00F
{
    public static class Log
    {
        public static bool EnableTimestamp { get; set; } = true;
        public static bool EnableRuntime { get; set; } = true;
        public static bool EnableThreadId { get; set; } = false;
        public static bool EnableFileName { get; set; } = true;
        public static bool EnableMemberName { get; set; } = false;

        [Conditional("DEBUG")]
        public static void Debug(object msg = null, [CallerFilePath] string filePath = null, [CallerMemberName] string memberName = null) => GD.Print(Format(filePath, memberName, msg));
        public static void Info(object msg = null, [CallerFilePath] string filePath = null, [CallerMemberName] string memberName = null) => GD.Print(Format(filePath, memberName, msg));
        public static void Warn(object msg = null, [CallerFilePath] string filePath = null, [CallerMemberName] string memberName = null) => GD.PushWarning(Format(filePath, memberName, msg));
        public static void Error(object msg = null, [CallerFilePath] string filePath = null, [CallerMemberName] string memberName = null) => GD.PushError(Format(filePath, memberName, msg));

        #region Private

        private static string Format(string filePath, string memberName, object msg)
            => $"{Timestamp()}{Runtime()}{ThreadId()}{FileName(filePath)}{MemberName(memberName)}{msg}";

        private static string Timestamp() => EnableTimestamp ? DateTime.Now.ToString("[HH:mm:ss.fff] ") : null;
        private static string Runtime() => EnableRuntime ? $"[{TimeSpan.FromMilliseconds(Time.GetTicksMsec()).Format()}] " : null;
        private static string ThreadId() => EnableThreadId ? $"[THRD{System.Environment.CurrentManagedThreadId}] " : null;
        private static string FileName(string x) => EnableFileName ? $"[{Path.GetFileNameWithoutExtension(x)}] " : null;
        private static string MemberName(string x) => EnableMemberName && x is not null ? $"[{x}] " : null;

        private static string Format(this TimeSpan value, string noTimeStr = "0ms")
        {
            var timeStr = value.ToString("d'.'hh':'mm':'ss'.'fff").TrimStart('0', ':', '.');
            return timeStr == "" ? noTimeStr
                 : !timeStr.Contains('.') ? $"{timeStr}ms"
                 : timeStr;
        }

        #endregion

        #region Extensions

        [Conditional("DEBUG")]
        public static void If(bool log, object msg = null, [CallerFilePath] string filePath = null, [CallerMemberName] string memberName = null)
        {
            if (log) GD.Print(Format(filePath, memberName, msg));
        }

        [Conditional("DEBUG")]
        public static void Print(
            object value0 = null,
            object value1 = null,
            object value2 = null,
            object value3 = null,
            object value4 = null,
            object value5 = null,
            object value6 = null,
            object value7 = null,
            object value8 = null,
            object value9 = null,
            [CallerArgumentExpression(nameof(value0))] string key0 = null,
            [CallerArgumentExpression(nameof(value1))] string key1 = null,
            [CallerArgumentExpression(nameof(value2))] string key2 = null,
            [CallerArgumentExpression(nameof(value3))] string key3 = null,
            [CallerArgumentExpression(nameof(value4))] string key4 = null,
            [CallerArgumentExpression(nameof(value5))] string key5 = null,
            [CallerArgumentExpression(nameof(value6))] string key6 = null,
            [CallerArgumentExpression(nameof(value7))] string key7 = null,
            [CallerArgumentExpression(nameof(value8))] string key8 = null,
            [CallerArgumentExpression(nameof(value9))] string key9 = null)
        {
            if (EmptyLog()) return;

            PrintTitle();
            PrintParts();

            bool EmptyLog()
                => key0 is null;

            void PrintTitle()
            {
                if (LogHasTitle(out var title))
                {
                    if (LogHasNoParts())
                        GD.Print(title);
                    else if (LogHasSinglePart(out var part))
                        GD.Print($"{title} [{part}]");
                    else
                        GD.Print($"{title}:");
                }

                bool LogHasTitle(out string title)
                {
                    if (key0.StartsWith('"') || key0.StartsWith('$'))
                    {
                        title = (string)value0;
                        key0 = null;
                        return true;
                    };

                    title = null;
                    return false;
                }

                bool LogHasNoParts()
                    => key1 is null;

                bool LogHasSinglePart(out string part)
                {
                    if (key2 is null)
                    {
                        part = $"{key1}: {value1}";
                        key1 = null;
                        return true;
                    };

                    part = null;
                    return false;
                }
            }

            void PrintParts()
            {
                Parts().ForEach(GD.Print);

                IEnumerable<string> Parts()
                {
                    var pad = Keys().MaxOrDefault(x => x.Length);
                    if (key0 is not null) yield return $" - {key0}:{Pad(key0)} {value0}";
                    if (key1 is not null) yield return $" - {key1}:{Pad(key1)} {value1}";
                    if (key2 is not null) yield return $" - {key2}:{Pad(key2)} {value2}";
                    if (key3 is not null) yield return $" - {key3}:{Pad(key3)} {value3}";
                    if (key4 is not null) yield return $" - {key4}:{Pad(key4)} {value4}";
                    if (key5 is not null) yield return $" - {key5}:{Pad(key5)} {value5}";
                    if (key6 is not null) yield return $" - {key6}:{Pad(key6)} {value6}";
                    if (key7 is not null) yield return $" - {key7}:{Pad(key7)} {value7}";
                    if (key8 is not null) yield return $" - {key8}:{Pad(key8)} {value8}";
                    if (key9 is not null) yield return $" - {key9}:{Pad(key9)} {value9}";

                    IEnumerable<string> Keys()
                    {
                        if (key0 is not null) yield return key0;
                        if (key1 is not null) yield return key1;
                        if (key2 is not null) yield return key2;
                        if (key3 is not null) yield return key3;
                        if (key4 is not null) yield return key4;
                        if (key5 is not null) yield return key5;
                        if (key6 is not null) yield return key6;
                        if (key7 is not null) yield return key7;
                        if (key8 is not null) yield return key8;
                        if (key9 is not null) yield return key9;
                    }

                    string Pad(string key)
                        => "".PadLeft(pad - key.Length);
                }
            }
        }

        public static string Str(
            object value0 = null,
            object value1 = null,
            object value2 = null,
            object value3 = null,
            object value4 = null,
            object value5 = null,
            object value6 = null,
            object value7 = null,
            object value8 = null,
            object value9 = null,
            [CallerArgumentExpression(nameof(value0))] string key0 = null,
            [CallerArgumentExpression(nameof(value1))] string key1 = null,
            [CallerArgumentExpression(nameof(value2))] string key2 = null,
            [CallerArgumentExpression(nameof(value3))] string key3 = null,
            [CallerArgumentExpression(nameof(value4))] string key4 = null,
            [CallerArgumentExpression(nameof(value5))] string key5 = null,
            [CallerArgumentExpression(nameof(value6))] string key6 = null,
            [CallerArgumentExpression(nameof(value7))] string key7 = null,
            [CallerArgumentExpression(nameof(value8))] string key8 = null,
            [CallerArgumentExpression(nameof(value9))] string key9 = null)
        {
            return string.Join(", ", Parts());

            IEnumerable<string> Parts()
            {
                if (key0 is not null) yield return $"{key0}: {value0}";
                if (key1 is not null) yield return $"{key1}: {value1}";
                if (key2 is not null) yield return $"{key2}: {value2}";
                if (key3 is not null) yield return $"{key3}: {value3}";
                if (key4 is not null) yield return $"{key4}: {value4}";
                if (key5 is not null) yield return $"{key5}: {value5}";
                if (key6 is not null) yield return $"{key6}: {value6}";
                if (key7 is not null) yield return $"{key7}: {value7}";
                if (key8 is not null) yield return $"{key8}: {value8}";
                if (key9 is not null) yield return $"{key9}: {value9}";
            }
        }

        #region Private

        private static bool IsNullOrEmpty<T>(this IEnumerable<T> source)
            => source is null || !source.Any();

        //private static T MaxOrDefault<T>(this IEnumerable<T> source, T defaultValue = default)
        //    => source.IsNullOrEmpty() ? defaultValue : source.Max();

        private static TResult MaxOrDefault<T, TResult>(this IEnumerable<T> source, Func<T, TResult> selector, TResult defaultValue = default)
            => source.IsNullOrEmpty() ? defaultValue : source.Max(selector);

        #endregion

        #endregion
    }
}
