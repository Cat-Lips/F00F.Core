using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace F00F;

public interface IPlayerData
{
    event Action RankChanged;

    string DisplayName { get; set; }
    Color DisplayColor { get; set; }
    float CurrentScore { get; set; }
    int CurrentRank { get; }

    string this[string xtras] { get; set; }
}

[Tool]
public partial class PlayerList : DataView
{
    private readonly Dictionary<int, PlayerData> myPlayers = [];
    private string[] xtras = [];

    public PlayerList()
        => Visible = Editor.IsEditor;

    public void Initialise(params string[] xtras)
        => PlayerData.AddTitle(Grid, this.xtras = xtras ?? []);

    public IPlayerData AddPlayer(Node player)
    {
        var id = player.AuthId();
        var data = new PlayerData(id, xtras);
        data.ScoreChanged += RankRows;
        myPlayers.Add(id, data);
        data.AddRow(Grid);
        Visible = true;
        RankRows();
        return data;
    }

    public void RemovePlayer(Node player)
    {
        var id = player.AuthId();

        if (myPlayers.Remove(id, out var data))
        {
            data.RemoveRow(Grid);
            RankRows();
        }

        Visible = myPlayers.Count is not 0;
    }

    #region Private

    private AutoAction _RankRows;
    private void RankRows()
    {
        (_RankRows ??= new(RankRows)).Run();

        void RankRows()
        {
            myPlayers.Values
                .OrderBy(x => x.CurrentScore)
                .ForEach((x, i) =>
                {
                    x.CurrentRank = i;
                    x.MoveRow(Grid, i + 2);
                });
        }
    }

    #endregion

    private class PlayerData : IPlayerData
    {
        public event Action NameChanged;
        public event Action ColorChanged;
        public event Action ScoreChanged;
        public event Action RankChanged;

        public string DisplayName { get; set => this.Set(ref field, value, NameChanged); }
        public Color DisplayColor { get; set => this.Set(ref field, value, ColorChanged); }
        public float CurrentScore { get; set => this.Set(ref field, value, ScoreChanged); }
        public int CurrentRank { get; set => this.Set(ref field, value, RankChanged); }

        #region State

        private readonly Dictionary<string, string> state = [];

        public event Action<string> StateChanged;

        public string this[string name]
        {
            get => state.TryGet(name);
            set { state[name] = value; StateChanged?.Invoke(name); }
        }

        #endregion

        #region Row

        private Control[] Row { get; }

        public PlayerData(int id, string[] xtras)
            => Row = CreateRow(id, xtras);

        public void AddRow(GridContainer grid)
            => Row.ForEach(x => grid.AddChild(x));

        public void RemoveRow(GridContainer grid)
            => Row.ForEach(x => grid.RemoveChild(x, free: true));

        public void MoveRow(GridContainer grid, int idx)
        {
            idx *= grid.Columns;
            Row.ForEach(x => grid.MoveChild(x, idx++));
        }

        public static void AddTitle(GridContainer grid, string[] xtras)
        {
            var columns = CreateTitle(xtras);
            grid.Columns = columns.Length;
            columns.ForEach(x => grid.AddChild(x, forceReadableName: true));
            grid.AddRowSeparator();
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

        private Control[] CreateRow(int id, string[] xtras)
        {
            return Labels().InterspersedWith(() => UI.NewSep(v: true)).ToArray();

            IEnumerable<Control> Labels()
            {
                var key = $"{id}";

                var lblRank = UI.NewLabel($"{key}_Rank", $"{CurrentRank}", alignment: HorizontalAlignment.Right);
                var lblName = UI.NewLabel($"{key}_Name", DisplayName, alignment: HorizontalAlignment.Right);
                var lblScore = UI.NewLabel($"{key}_Score", $"{CurrentScore}", alignment: HorizontalAlignment.Right);
                lblRank.SetFontColor(DisplayColor);
                lblName.SetFontColor(DisplayColor);
                lblScore.SetFontColor(DisplayColor);

                NameChanged += () => lblName.Text = DisplayName;
                ColorChanged += () => lblRank.SetFontColor(DisplayColor);
                ColorChanged += () => lblName.SetFontColor(DisplayColor);
                ColorChanged += () => lblScore.SetFontColor(DisplayColor);
                ScoreChanged += () => lblScore.Text = $"{CurrentScore}";

                return GetXtras().Append(lblScore).Prepend(lblName).Prepend(lblRank);

                IEnumerable<Control> GetXtras()
                {
                    var lblLookup = xtras.ToDictionary(x => x, column =>
                    {
                        var lblState = UI.NewLabel($"{key}_{column}", this[column], alignment: HorizontalAlignment.Right);
                        lblState.SetFontColor(DisplayColor);
                        return lblState;
                    });

                    StateChanged += column => lblLookup.TryGet(column)?.SetText(this[column]);
                    ColorChanged += () => lblLookup.Values.ForEach(lblState => lblState.SetFontColor(DisplayColor));

                    return lblLookup.Values;
                }
            }
        }

        #endregion
    }
}
