using System;
using System.ComponentModel;
using Godot;

namespace F00F;

public static class App
{
    public static readonly string Name = (string)ProjectSettings.GetSetting("application/config/name");
    public static readonly uint Hash = (uint)GD.Hash(Name);
    public static int Id { get; internal set; } = -1;

    public static string AddId(string str) => Id is -1 ? str : str.AddSuffix($".{Id}");
    public static string RemoveId(string str) => Id is -1 ? str : str.TrimSuffix($".{Id}");

    public static event Action<CancelEventArgs> Quit;

    internal static void InitQuit(this Node source)
    {
        source.GetTree().QuitOnGoBack = true;
        source.GetTree().AutoAcceptQuit = false;
    }

    internal static void NotifyQuit(this Node source, int what)
    {
        switch ((long)what)
        {
            case Node.NotificationWMCloseRequest:
            case Node.NotificationWMGoBackRequest:
                TryQuit();
                break;
        }

        void TryQuit()
        {
            var e = new CancelEventArgs();

            Quit?.Invoke(e);
            if (e.Cancel) return;
            source.GetTree().Quit();
        }
    }
}
