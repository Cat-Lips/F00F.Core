using System.Collections.Generic;
using System.Linq;
using Godot;

namespace F00F;

[Tool]
public partial class PlayerList : DataView
{
    private readonly Dictionary<int, (PlayerData Data, Control[] Row)> myPlayers = [];
    private string[] xtras = [];

    [ExportGroup("Display")]
    [Export] public int DisplayPrecision { get; set; }
    [Export] public bool DisplayAscending { get; set; }

    public IPlayerData this[int pid] => myPlayers[pid].Data;

    public PlayerList()
        => Visible = Editor.IsEditor;

    public void Initialise(params string[] xtras)
        => Rows.AddTitle(Grid, this.xtras = xtras ?? []);

    internal void AddPlayer(int pid, PlayerData data)
    {
        myPlayers.Add(pid, (data, Rows.Add(Grid, pid, data, xtras, DisplayPrecision)));

        RankPlayers();
        data.ScoreChanged += x => RankPlayers();

        if (myPlayers.Count is 1)
            Visible = true;
    }

    internal void RemovePlayer(int pid)
    {
        if (myPlayers.Remove(pid, out var x))
        {
            Rows.Remove(Grid, x.Row);
            RankPlayers();
        }

        if (myPlayers.Count is 0)
            Visible = false;
    }

    public void SetDisplayColor(Node player, /*in */Color color)
        => myPlayers[player.PeerId()].Row.ForEach(x => (x as Label)?.SetFontColor(color));

    #region Private

    private AutoAction _RankPlayers;
    private void RankPlayers()
    {
        (_RankPlayers ??= new(RankRows)).Run();

        void RankRows()
        {
            var order = DisplayAscending
                ? myPlayers.Values.OrderBy(x => x.Data.CurrentScore)
                : myPlayers.Values.OrderByDescending(x => x.Data.CurrentScore);

            order.ForEach((x, i) =>
            {
                x.Data.CurrentRank = i + 1;
                Rows.Move(Grid, x.Row, i + 2);
            });
        }
    }

    #endregion

    private static class Rows
    {
        public static void AddTitle(GridContainer grid, string[] xtras)
        {
            var columns = CreateTitle(xtras);
            grid.Columns = columns.Length;
            columns.ForEach(x => grid.AddChild(x));
            grid.AddRowSeparator();
        }

        public static Control[] Add(GridContainer grid, int pid, PlayerData data, string[] xtras, int precision)
        {
            var controls = CreateRow(pid, data, xtras, $"N{precision}");
            controls.ForEach(x => grid.AddChild(x));
            return controls;
        }

        public static void Remove(GridContainer grid, Control[] controls)
            => controls.ForEach(x => grid.RemoveChild(x, free: true));

        public static void Move(GridContainer grid, Control[] controls, int idx)
        {
            idx *= grid.Columns;
            controls.ForEach(x => grid.MoveChild(x, idx++));
        }

        private static Control[] CreateTitle(string[] xtras)
        {
            return Labels().InterspersedWith(() => UI.NewSep(v: true)).ToArray();

            IEnumerable<Control> Labels()
            {
                var lblRank = UI.NewLabel("Rank", "Rank", alignment: HorizontalAlignment.Center);
                var lblName = UI.NewLabel("Player", "Player", alignment: HorizontalAlignment.Center);
                var lblScore = UI.NewLabel("Score", "Score", alignment: HorizontalAlignment.Center);

                return GetXtras().Append(lblScore).Prepend(lblName).Prepend(lblRank);

                IEnumerable<Control> GetXtras()
                    => xtras.Select(x => UI.NewLabel(x, x, alignment: HorizontalAlignment.Center));
            }
        }

        private static Control[] CreateRow(int pid, PlayerData data, string[] xtras, string format)
        {
            return Labels().InterspersedWith(() => UI.NewSep(v: true)).ToArray();

            IEnumerable<Control> Labels()
            {
                var key = $"{pid}";

                var lblRank = UI.NewLabel($"{key}_Rank", $"{data.CurrentRank}", alignment: HorizontalAlignment.Right);
                var lblName = UI.NewLabel($"{key}_Name", data.PlayerName, alignment: HorizontalAlignment.Right);
                var lblScore = UI.NewLabel($"{key}_Score", $"{data.CurrentScore.ToString(format)}", alignment: HorizontalAlignment.Right);

                data.RankChanged += x => lblRank.Text = $"{x}";
                data.NameChanged += x => lblName.Text = x;
                data.ScoreChanged += x => lblScore.Text = $"{x.ToString(format)}";

                lblRank.SetFontColor(data.PlayerColor);
                lblName.SetFontColor(data.PlayerColor);
                lblScore.SetFontColor(data.PlayerColor);

                data.ColorChanged += x =>
                {
                    lblRank.SetFontColor(x);
                    lblName.SetFontColor(x);
                    lblScore.SetFontColor(x);
                };

                return GetXtras().Append(lblScore).Prepend(lblName).Prepend(lblRank);

                IEnumerable<Control> GetXtras()
                {
                    var lblLookup = xtras.ToDictionary(x => x, column =>
                    {
                        var lblState = UI.NewLabel($"{key}_{column}", data[column], alignment: HorizontalAlignment.Right);
                        lblState.SetFontColor(data.PlayerColor);
                        return lblState;
                    });

                    data.StateChanged += (column, value) => lblLookup.TryGet(column)?.SetText(value);
                    data.ColorChanged += x => lblLookup.Values.ForEach(lblState => lblState.SetFontColor(x));

                    return lblLookup.Values;
                }
            }
        }
    }
}
