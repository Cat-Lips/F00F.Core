using System;
using System.Runtime.CompilerServices;
using Godot;

namespace F00F;

public static class ErrorExtensions
{
    public static bool Ok(this Error err) => err is Error.Ok;
    public static bool NotOk(this Error err) => err is not Error.Ok;

    public static Error Ok(this Error err, Action ok) { if (err.Ok()) ok(); return err; }
    public static Error NotOk(this Error err, Action notOk) { if (err.NotOk()) notOk(); return err; }

    public static bool Err(this Error err, string title, string msg, out string error, [CallerArgumentExpression(nameof(err))] string expr = null, [CallerFilePath] string file = null, [CallerMemberName] string member = null, [CallerLineNumber] int line = default)
    {
        if (err.Ok()) { error = null; return true; }
        error = Print(err, title, msg, expr, file, member, line);
        return false;
    }

    public static bool Err(this Error err, string title, string msg, Action<string> OnError, [CallerArgumentExpression(nameof(err))] string expr = null, [CallerFilePath] string file = null, [CallerMemberName] string member = null, [CallerLineNumber] int line = default)
    {
        if (err.Ok()) return true;
        OnError(Print(err, title, msg, expr, file, member, line));
        return false;
    }

    public static bool Err(this Error err, string title, string msg, [CallerArgumentExpression(nameof(err))] string expr = null, [CallerFilePath] string file = null, [CallerMemberName] string member = null, [CallerLineNumber] int line = default)
    {
        if (err.Ok()) return true;
        Print(err, title, msg, expr, file, member, line);
        return false;
    }

    public static void Throw(this Error err, string title, string msg, [CallerArgumentExpression(nameof(err))] string expr = null, [CallerFilePath] string file = null, [CallerMemberName] string member = null, [CallerLineNumber] int line = default)
    {
        if (err.Ok()) return;
        throw new Exception(Print(err, title, msg, expr, file, member, line));
    }

    private static string Print(Error err, string title, string msg, [CallerArgumentExpression(nameof(err))] string expr = null, [CallerFilePath] string file = null, [CallerMemberName] string member = null, [CallerLineNumber] int line = default)
    {
        var message = FormatMessage(err, title, msg);
        var location = FormatLocation(expr, file, member, line);
        GD.PrintErr(message, '\n', location);
        return message;

        static string FormatMessage(Error err, string title, string msg)
            => $"{title.Capitalise()} [{err}]: {msg}";

        static string FormatLocation(string expr, string file, string member, int line)
            => $" - in {file.GetFileBaseName()}.{member} (line {line}): {expr}";
    }

    public static bool Err(this Error err, string msg, out string error, [CallerArgumentExpression(nameof(err))] string expr = null, [CallerFilePath] string file = null, [CallerMemberName] string member = null, [CallerLineNumber] int line = default)
        => err.Err(title: member, msg, out error, expr, file, member, line);

    public static bool Err(this Error err, string msg, Action<string> OnError, [CallerArgumentExpression(nameof(err))] string expr = null, [CallerFilePath] string file = null, [CallerMemberName] string member = null, [CallerLineNumber] int line = default)
        => err.Err(title: member, msg, OnError, expr, file, member, line);

    public static bool Err(this Error err, string msg, [CallerArgumentExpression(nameof(err))] string expr = null, [CallerFilePath] string file = null, [CallerMemberName] string member = null, [CallerLineNumber] int line = default)
        => err.Err(title: member, msg, expr, file, member, line);

    public static void Throw(this Error err, string msg, [CallerArgumentExpression(nameof(err))] string expr = null, [CallerFilePath] string file = null, [CallerMemberName] string member = null, [CallerLineNumber] int line = default)
        => err.Throw(title: member, msg, expr, file, member, line);
}
