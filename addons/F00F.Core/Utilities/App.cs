﻿using System;
using System.ComponentModel;
using Godot;

namespace F00F;

public static class App
{
    public static readonly string Name = (string)ProjectSettings.GetSetting("application/config/name");
    public static readonly uint Hash = (uint)GD.Hash(Name);
    public static int Id { get; internal set; } = -1;

    #region Life Cycle

    public static event Action<CancelEventArgs> Exit;

    public static void Quit(Node root)
        => root.GetMainWindow().PropagateNotification(NotificationWMCloseRequest);

    internal static void InitQuit(Node root)
    {
        root.GetTree().QuitOnGoBack = true;
        root.GetTree().AutoAcceptQuit = false;
    }

    private const int NotificationWMCloseRequest = (int)Node.NotificationWMCloseRequest;
    private const int NotificationWMGoBackRequest = (int)Node.NotificationWMGoBackRequest;
    internal static void NotifyQuit(Node root, int what)
    {
        if (!Editor.IsEditor)
        {
            switch (what)
            {
                case NotificationWMCloseRequest:
                case NotificationWMGoBackRequest:
                    TryQuit();
                    break;
            }

            void TryQuit()
            {
                var e = new CancelEventArgs();

                Exit?.Invoke(e);
                if (e.Cancel) return;
                root.GetTree().Quit();
            }
        }
    }

    #endregion

    #region Utilities

    public static string AddId(string str) => Id is -1 ? str : str.AddSuffix($".{Id}");
    public static string RemoveId(string str) => Id is -1 ? str : str.TrimSuffix($".{Id}");

    #endregion
}
