using System;
using System.Runtime.CompilerServices;

namespace F00F;

public interface IStatus
{
    static IStatus Instance { get; protected set; }

    protected void OnPush(Status status, string msg, string tag = null, Action Resolve = null);

    void Push(Status status, string msg, string tag = null, Action Resolve = null, [CallerFilePath] string filePath = null, [CallerMemberName] string memberName = null)
    {
        LogMsg(status, msg);
        OnPush(status, msg, tag, Resolve);

        void LogMsg(Status status, string msg)
        {
            switch (status)
            {
                case Status.Error:
                    Log.Error(msg, filePath, memberName);
                    break;
                case Status.Warn:
                    Log.Warn(msg, filePath, memberName);
                    break;
                default:
                    Log.Debug(msg, filePath, memberName);
                    break;
            }
        }
    }

    void Clear(Action OnComplete = null);
}
