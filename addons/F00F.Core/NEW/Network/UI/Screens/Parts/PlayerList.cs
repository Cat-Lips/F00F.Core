namespace F00F.NEW;

public partial class PlayerList : DataGrid
{
    #region Private

    private const int Avatar = 0;
    private const int Player = 1;
    private const int Ping = 2;
    private const int Loc = 3;
    protected sealed override int ColumnCount => 4;

    protected sealed override void InitColumns()
        => ColumnTitlesVisible = false;

    //protected sealed override void InitColumns()
    //{
    //    SetColumnTitle(Avatar);
    //    SetColumnTitle(Player);
    //    SetColumnTitle(Ping);
    //    SetColumnTitle(Loc);
    //}

    #endregion

    #region Godot

    protected sealed override void OnReady()
    {
        OnTreeEntered();
        TreeEntered += OnTreeEntered;
        TreeExiting += OnTreeExiting;

        void OnTreeEntered()
        {
            InitRows();

            Network.Instance.PlayerInsert += InsertRow;
            Network.Instance.PlayerUpdate += UpdateRow;
            Network.Instance.PlayerRemove += RemoveRow;
            Network.Instance.PlayersReset += ClearRows;
        }

        void OnTreeExiting()
        {
            Network.Instance.PlayerInsert -= InsertRow;
            Network.Instance.PlayerUpdate -= UpdateRow;
            Network.Instance.PlayerRemove -= RemoveRow;
            Network.Instance.PlayersReset -= ClearRows;

            ClearRows();
        }

        void InitRows()
        {
            Network.Instance.CurrentPlayers?.ForEach(kvp =>
            {
                InsertRow(kvp.Key);
                UpdateRow(kvp.Value);
            });
        }

        void RemoveRow(ulong id, string _) => base.RemoveRow(id);
        void InsertRow(ulong id) => base.InsertRow(id);
        void UpdateRow(Network.PlayerProfile pp)
        {
            var row = Row(pp.PlayerId);
            row.SetIcon(Avatar, pp.PlayerAvatar);
            row.SetText(Player, pp.PlayerName);

            row.SetRangeConfig(Ping, 0, 1, .1);
            var ping = Network.Instance.Ping(pp);
            var col = IStatus.Color(ping);
            row.SetRange(Ping, ping);
            if (col.HasValue) row.SetCustomColor(Ping, col.Value);
            row.SetTooltipText(Ping, Utils.HumaniseMs(pp.PlayerPing));

            row.SetText(Loc, pp.PlayerLocation);
        }
    }

    #endregion

    #region Utils
    #endregion
}
