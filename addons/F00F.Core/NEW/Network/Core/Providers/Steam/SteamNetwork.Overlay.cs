#if STEAM
using Steamworks;

namespace F00F.NEW;

internal partial class SteamNetwork
{
    #region Private

    private Callback<GameOverlayActivated_t> GameOverlayActivated;

    #endregion

    private void InitOverlay()
        => GameOverlayActivated = Callback<GameOverlayActivated_t>.Create(OnGameOverlayActivated);

    private void DnitOverlay()
    {
        GameOverlayActivated?.Dispose();
        GameOverlayActivated = null;
    }

    #region Private

    private bool TreePauseState;
    private void OnGameOverlayActivated(GameOverlayActivated_t t)
    {
        switch (t.m_bActive)
        {
            case 1:
                Log.Info("Steam Overlay Activated");
                TreePauseState = GetTree().Paused;
                GetTree().Paused = true;
                break;
            case 0:
                Log.Info($"Steam Overlay Deactivated");
                GetTree().Paused = TreePauseState;
                break;
        }
    }

    #endregion
}
#endif
