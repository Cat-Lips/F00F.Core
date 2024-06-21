using System;
using Godot;

namespace F00F
{
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

        public static void SafeConnect(this GodotObject source, StringName signal, Action action) { var callable = Callable.From(action); if (!source.IsConnected(signal, callable)) source.Connect(signal, callable); }
        public static void SafeConnect<T>(this GodotObject source, StringName signal, Action<T> action) { var callable = Callable.From(action); if (!source.IsConnected(signal, callable)) source.Connect(signal, callable); }
        public static void SafeConnect<T1, T2>(this GodotObject source, StringName signal, Action<T1, T2> action) { var callable = Callable.From(action); if (!source.IsConnected(signal, callable)) source.Connect(signal, callable); }

        public static void SafeDisconnect(this GodotObject source, StringName signal, Action action) { var callable = Callable.From(action); if (source.IsConnected(signal, callable)) source.Disconnect(signal, callable); }
        public static void SafeDisconnect<T>(this GodotObject source, StringName signal, Action<T> action) { var callable = Callable.From(action); if (source.IsConnected(signal, callable)) source.Disconnect(signal, callable); }
        public static void SafeDisconnect<T1, T2>(this GodotObject source, StringName signal, Action<T1, T2> action) { var callable = Callable.From(action); if (source.IsConnected(signal, callable)) source.Disconnect(signal, callable); }

        public static void SafeReconnect(this GodotObject source, StringName signal, Action action) { var callable = Callable.From(action); if (source.IsConnected(signal, callable)) source.Disconnect(signal, callable); source.Connect(signal, callable); }
        public static void SafeReconnect<T>(this GodotObject source, StringName signal, Action<T> action) { var callable = Callable.From(action); if (source.IsConnected(signal, callable)) source.Disconnect(signal, callable); source.Connect(signal, callable); }
        public static void SafeReconnect<T1, T2>(this GodotObject source, StringName signal, Action<T1, T2> action) { var callable = Callable.From(action); if (source.IsConnected(signal, callable)) source.Disconnect(signal, callable); source.Connect(signal, callable); }

        public static void SafeInit(this Node source, Action OnEnter, Action OnExit, Func<bool> Init = null)
        {
            source.SafeDisconnect(Node.SignalName.TreeEntered, OnEnter);
            source.SafeDisconnect(Node.SignalName.TreeExiting, OnExit);

            if (Init?.Invoke() ?? true)
            {
                source.Connect(Node.SignalName.TreeEntered, OnEnter);
                source.Connect(Node.SignalName.TreeExiting, OnExit);

                if (source.IsInsideTree())
                    OnEnter();
            }
        }

        public static void OnReady(this Node source, Action action)
        {
            if (source.IsNodeReady()) action();
            else source.SafeReconnect(Node.SignalName.Ready, action);
        }
    }
}
