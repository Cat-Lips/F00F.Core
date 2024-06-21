using System;
using Godot;

namespace F00F;

using StatusMsg = Action<Status, string>;

public interface IStatus
{
    #region Instance

    static IStatus Instance { get; protected set; }

    void Push(Status status, string msg, string tag = null, Action Resolve = null);

    void Clear(Action OnComplete = null);

    #endregion

    #region Helpers

    static void Subscribe(StatusMsg action) => Subscribe(null, action);
    static void Subscribe(string tag, StatusMsg action) => StatusExtensions.Subscribe(tag, action);

    static Color? Color(float ratio) =>
        ratio < .25f ? Status.Error.Color() :
        ratio < .50f ? Status.Warn.Color() :
        ratio < .75f ? Status.Info.Color() : Status.Success.Color();

    #endregion
}
