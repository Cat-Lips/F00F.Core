using System;
using Godot;

namespace F00F.NEW;

using static Network;

[Tool]
public partial class SessionList : DataGrid
{
    #region Private

    private const int Host = 0;
    private const int Game = 1;
    private const int Players = 2;
    protected override int ColumnCount => 3;

    protected static Network Network => Network.Instance;

    #endregion

    public void Search(Action Done, SearchFilter Filter = null) => Search(Filter, Done);
    public void Search(SearchFilter Filter = null, Action Done = null)
    {
        Network.Search(Filter, OnSearchComplete);

        void OnSearchComplete(SessionState[] ss)
        {
            ResetRows(ss, InitRow);
            Done?.Invoke();

            void InitRow(SessionState ss)
            {
                var row = NewRow(ss.SessionId);
                row.SetText(Host, ss.SessionHost);
                row.SetText(Game, ss.SessionName);
                row.SetText(Players, PlayersText());

                string PlayersText()
                    => $"{ss.MemberCount}/{ss.MemberLimit}";
            }
        }
    }

    #region Private

    protected sealed override void InitColumns()
    {
        SetColumnTitle(Host);
        SetColumnTitle(Game);
        SetColumnTitle(Players);
    }

    #endregion
}
