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

    // old
    public static bool SafeInit(this CustomResource source, GodotObject root, Action OnChanged)
    {
        if (source is null) return false;
        source.Changed -= _OnChanged;
        source.Changed += _OnChanged;
        _OnChanged(); return true;

        void _OnChanged()
        {
            if (!GodotObject.IsInstanceValid(root))
                source.Changed -= _OnChanged;
            else OnChanged();
        }
    }

    public static void SafeInit(this Node source, Action OnEnterTree, Action OnExitTree = null)
    {
        //Debug.Assert(!source.IsConnected(Node.SignalName.TreeEntered, OnEnterTree));
        //Debug.Assert(!source.IsConnected(Node.SignalName.TreeEntered, OnExitTree));
        source.SafeConnect(Node.SignalName.TreeEntered, OnEnterTree);
        source.SafeConnect(Node.SignalName.TreeExiting, OnExitTree);
        //Debug.Assert(source.IsConnected(Node.SignalName.TreeEntered, OnEnterTree));
        //Debug.Assert(source.IsConnected(Node.SignalName.TreeEntered, OnExitTree));
        if (source.IsInTree()) OnEnterTree();
    }

    public static void SafeInit<T>(this Node source, T old, T @new, Action<T> OnEnterTree, Action<T> OnExitTree) where T : class
    {
        if (old.NotNull())
        {
            OnExitTree(old);
            //Debug.Assert(source.IsConnected(Node.SignalName.TreeEntered, () => OnEnterTree(@old)));
            //Debug.Assert(source.IsConnected(Node.SignalName.TreeExiting, () => OnExitTree(@old)));
            source.SafeDisconnect(Node.SignalName.TreeEntered, () => OnEnterTree(@old));
            source.SafeDisconnect(Node.SignalName.TreeExiting, () => OnExitTree(@old));
            //Debug.Assert(!source.IsConnected(Node.SignalName.TreeEntered, () => OnEnterTree(@old)));
            //Debug.Assert(!source.IsConnected(Node.SignalName.TreeExiting, () => OnExitTree(@old)));
        }

        if (@new.NotNull())
        {
            //Debug.Assert(!source.IsConnected(Node.SignalName.TreeEntered, () => OnEnterTree(@new)));
            //Debug.Assert(!source.IsConnected(Node.SignalName.TreeExiting, () => OnExitTree(@new)));
            source.SafeConnect(Node.SignalName.TreeEntered, () => OnEnterTree(@new));
            source.SafeConnect(Node.SignalName.TreeExiting, () => OnExitTree(@new));
            //Debug.Assert(source.IsConnected(Node.SignalName.TreeEntered, () => OnEnterTree(@new)));
            //Debug.Assert(source.IsConnected(Node.SignalName.TreeExiting, () => OnExitTree(@new)));
            if (source.IsInTree()) OnEnterTree(@new);
        }
    }

    public static void SafeInit<T>(this Node source, T old, T @new, Action OnChanged) where T : CustomResource
    {
        source.SafeInit(old, @new, OnEnterTree, OnExitTree);

        void OnEnterTree(T cfg)
        {
            OnChanged();
            cfg.Changed += OnChanged;
        }

        void OnExitTree(T cfg)
            => cfg.Changed -= OnChanged;
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

    public static void IfReady(this Node source, params Action[] action)
    {
        if (source.IsReady())
            action.ForEach(x => x());
    }

    public static SignalAwaiter ToSignal(this GodotObject source, StringName signal)
         => source.ToSignal(source, signal);

    public static SignalAwaiter AwaitChanged(this Resource source)
         => source.ToSignal(source, Resource.SignalName.Changed);
}
