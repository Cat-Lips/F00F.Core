using System;
using System.Collections.Generic;
using Godot;

namespace F00F;

public interface IPlayerData
{
    event Action<string> NameChanged;
    event Action<Color> ColorChanged;
    event Action<string> AvatarChanged;

    event Action<string, string> StateChanged;

    event Action<float> ScoreChanged;
    event Action<int> RankChanged;

    string PlayerName { get; set; }
    Color PlayerColor { get; set; }
    string PlayerAvatar { get; set; }

    string this[string state] { get; set; }

    float CurrentScore { get; set; }
    int CurrentRank { get; }

    IEnumerable<string> State { get; }
}

internal class PlayerData : IPlayerData
{
    private readonly Dictionary<string, string> state = [];

    public event Action<string> NameChanged;
    public event Action<Color> ColorChanged;
    public event Action<string> AvatarChanged;

    public event Action<string, string> StateChanged;

    public event Action<float> ScoreChanged;
    public event Action<int> RankChanged;

    public string PlayerName { get; set => this.Set(ref field, value, NameChanged); }
    public Color PlayerColor { get; set => this.Set(ref field, value, ColorChanged); } = Colors.White;
    public string PlayerAvatar { get; set => this.Set(ref field, value, AvatarChanged); }

    public string this[string key]
    {
        get => state.TryGet(key);
        set => state.TrySet(key, value, StateChanged);
    }

    public float CurrentScore { get; set => this.Set(ref field, value, ScoreChanged); }
    public int CurrentRank { get; set => this.Set(ref field, value, RankChanged); }

    public IEnumerable<string> State => state.Keys;
}
