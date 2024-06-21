using System;
using Godot;

namespace F00F;

public static class ConnectExtensions
{
    private static void Connect(this GodotObject source, StringName signal, Action action) => source.Connect(signal, Callable.From(action));
    private static void Connect<T>(this GodotObject source, StringName signal, Action<T> action) => source.Connect(signal, Callable.From(action));
    private static void Connect<T1, T2>(this GodotObject source, StringName signal, Action<T1, T2> action) => source.Connect(signal, Callable.From(action));

    private static void Disconnect(this GodotObject source, StringName signal, Action action) => source.Disconnect(signal, Callable.From(action));
    private static void Disconnect<T>(this GodotObject source, StringName signal, Action<T> action) => source.Disconnect(signal, Callable.From(action));
    private static void Disconnect<T1, T2>(this GodotObject source, StringName signal, Action<T1, T2> action) => source.Disconnect(signal, Callable.From(action));

    private static bool IsConnected(this GodotObject source, StringName signal, Action action) => source.IsConnected(signal, Callable.From(action));
    private static bool IsConnected<T>(this GodotObject source, StringName signal, Action<T> action) => source.IsConnected(signal, Callable.From(action));
    private static bool IsConnected<T1, T2>(this GodotObject source, StringName signal, Action<T1, T2> action) => source.IsConnected(signal, Callable.From(action));

    public static void SafeConnect(this GodotObject source, StringName signal, Action action) { source.SafeDisconnect(signal, action); source.Connect(signal, action); }
    public static void SafeConnect<T>(this GodotObject source, StringName signal, Action<T> action) { source.SafeDisconnect(signal, action); source.Connect(signal, action); }
    public static void SafeConnect<T1, T2>(this GodotObject source, StringName signal, Action<T1, T2> action) { source.SafeDisconnect(signal, action); source.Connect(signal, action); }

    public static void SafeDisconnect(this GodotObject source, StringName signal, Action action) { if (source.IsConnected(signal, action)) source.Disconnect(signal, action); }
    public static void SafeDisconnect<T>(this GodotObject source, StringName signal, Action<T> action) { if (source.IsConnected(signal, action)) source.Disconnect(signal, action); }
    public static void SafeDisconnect<T1, T2>(this GodotObject source, StringName signal, Action<T1, T2> action) { if (source.IsConnected(signal, action)) source.Disconnect(signal, action); }

    public static bool SafeInit(this CustomResource source, GodotObject root, Action Init)
    {
        if (source is null) return false;
        source.Changed -= _Init;
        source.Changed += _Init;
        _Init(); return true;

        void _Init()
        {
            if (!GodotObject.IsInstanceValid(root))
                source.Changed -= _Init;
            else Init();
        }
    }

    public static bool SafeInit(this Node source, Action OnEnterTree, Action OnExitTree = null)
    {
        if (source is null) return false;
        source.SafeConnect(Node.SignalName.TreeExiting, OnExitTree);
        source.SafeConnect(Node.SignalName.TreeEntered, OnEnterTree);
        if (source.IsInsideTree()) OnEnterTree();
        return true;
    }

    public static void OnReady(this Node source, Action action)
    {
        if (source.NotReady())
        {
            source.SafeConnect(Node.SignalName.Ready, action);
            source.SafeConnect(Node.SignalName.Ready, SafeDisconnect);
        }
        else
        {
            action();
            SafeDisconnect();
        }

        void SafeDisconnect()
        {
            source.SafeDisconnect(Node.SignalName.Ready, action);
            source.SafeDisconnect(Node.SignalName.Ready, SafeDisconnect);
        }
    }

    public static SignalAwaiter ToSignal(this GodotObject source, StringName signal)
         => source.ToSignal(source, signal);

    public static SignalAwaiter AwaitChanged(this Resource source)
         => source.ToSignal(source, Resource.SignalName.Changed);
}
