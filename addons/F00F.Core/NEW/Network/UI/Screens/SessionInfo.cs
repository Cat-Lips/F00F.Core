using Godot;

namespace F00F.NEW;

[Tool]
public partial class SessionInfo : RootView
{
    #region Godot

    public SessionInfo()
    {
        if (Editor.NotEditor)
            Ready += OnReady;

        static void OnReady()
        {

        }
    }

    #endregion
}
