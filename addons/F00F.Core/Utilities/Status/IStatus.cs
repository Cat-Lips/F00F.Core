using System.Runtime.CompilerServices;

namespace F00F;

public interface IStatus
{
    static IStatus Instance { get; protected set; }

    protected void OnPush(Status status, string msg, string tag = null);

    void Push(Status status, string msg, string tag = null, [CallerFilePath] string filePath = null, [CallerMemberName] string memberName = null)
    {
        LogMsg(status, msg);
        OnPush(status, msg, tag);

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
}
