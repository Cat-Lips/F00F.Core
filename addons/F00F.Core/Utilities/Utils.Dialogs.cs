using System;
using Godot;

namespace F00F
{
    public static partial class Utils
    {
        public static void ShowError(Node parent, Error err, string msg)
            => ShowInfo(parent, $"{err}".Capitalize(), msg);

        public static void ShowInfo(Node parent, string title, string text)
            => ShowDialog<AcceptDialog>(parent, title ?? "Alert!", text);

        public static void ShowConfirm(Node parent, string title, string text, Action confirm, Action cancel = null)
            => ShowDialog<ConfirmationDialog>(parent, title ?? "Please Confirm...", text, confirm, cancel);

        private static void ShowDialog<T>(Node parent, string title, string text, Action confirm = null, Action cancel = null) where T : AcceptDialog, new()
        {
            var dlg = new T
            {
                Title = title,
                DialogText = text,
                AlwaysOnTop = true,
                DialogAutowrap = true,
            };

            dlg.Canceled += () => cancel?.Invoke();
            dlg.Confirmed += () => confirm?.Invoke();

            dlg.PopupExclusiveCenteredClamped(parent);

            dlg.GrabFocus(); // FIXME: Keep focus?
        }
    }
}
