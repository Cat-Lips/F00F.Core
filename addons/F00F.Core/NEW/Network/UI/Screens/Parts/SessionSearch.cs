using System;
using Godot;

namespace F00F.NEW;

[Tool]
public partial class SessionSearch : Container
{
    #region Private

    private readonly DeltaTimer PollTimer = new();
    private readonly DeltaTimer TextTimer = new();
    private readonly WidthGroup WidthGroup = new();
    private readonly HeightGroup HeightGroup = new();

    private Container Header => field ??= (Container)GetNode("%Header");
    private JoinGame JoinGame => field ??= (JoinGame)GetNode("%JoinGame");
    private StatusBar StatusBar => field ??= (StatusBar)GetNode("%StatusBar");
    private SunkLabel InfoLabel => field ??= (SunkLabel)GetNode("%InfoLabel");
    private SessionList SessionList => field ??= (SessionList)GetNode("%SessionList");
    private SessionFilter SessionFilter => field ??= (SessionFilter)GetNode("%SessionFilter");

    #endregion

    #region Export

    public event Action PollIntervalChanged;
    public event Action TextIntervalChanged;

    [Export] public int PollInterval { get; set => this.Set(ref field, value.ClampMin(0), PollIntervalChanged); } = Const.OneSecond;
    [Export] public int TextInterval { get; set => this.Set(ref field, value.ClampMin(0), TextIntervalChanged); } = Const.QtrSecond;

    #endregion

    public ulong EndTime { get; private set; }
    public ulong StartTime { get; private set; }
    public ulong SearchTime => EndTime - StartTime;
    public ulong TimeSinceLastResult => GetTime() - EndTime;
    public ulong TimeSinceLastSearch => GetTime() - StartTime;
    public ulong TimeToNextSearch => Millis(PollTimer.TimeRemaining);

    public event Action SearchingChanged;
    public bool Searching { get; private set => this.Set(ref field, value, SearchingChanged); }
    public bool NotSearching => !Searching;

    #region Godot

    public sealed override void _Ready()
    {
        InitLayout();

        Editor.Disable(this);
        if (Editor.IsEditor) return;

        InitTimers();
        InitSearch();
        InitSelect();
        InitStatus();
        StartSearch();
    }

    public sealed override void _Process(double _delta)
    {
        var delta = (float)_delta;

        if (NotSearching && PollTimer.Ready(delta))
            StartSearch();

        if (TextTimer.Ready(delta))
            SetInfoText();
    }

    #endregion

    #region Private

    private void StartSearch()
    {
        SetStartTime();
        Searching = true;
        SetInfoText();
        StartSearch();

        void SetStartTime()
            => StartTime = GetTime();

        void StartSearch()
            => SessionList.Search(SessionFilter.Filter, EndSearch);
    }

    private void EndSearch()
    {
        SetEndTime();
        Searching = false;
        SetInfoText();
        SetToolTipText();
        ResetPollTimer();

        void SetEndTime()
            => EndTime = GetTime();

        void SetToolTipText()
        {
            InfoLabel.TooltipText = GetToolTipText();

            string GetToolTipText()
                => $"Last Search: {SearchTime.HumaniseTime()}";
        }

        void ResetPollTimer()
            => PollTimer.Reset();
    }

    private void SetInfoText()
    {
        InfoLabel.Text = GetInfoText();

        string GetInfoText() => Searching
            ? $"Searching... {TimeSinceLastSearch.HumaniseTime()}"
            : $"Waiting... {PollTimer.TimeRemaining.HumaniseTime()}";
    }

    #region Init

    private void InitLayout()
    {
        WidthGroup.Init(Header, old: null, @new: WidthGroup);
        WidthGroup.Init(SessionList, old: null, @new: WidthGroup);
        HeightGroup.Init(Header, old: null, @new: HeightGroup);
        HeightGroup.Init(SessionList, old: null, @new: HeightGroup);
    }

    private void InitTimers()
    {
        SetPollInterval();
        SetTextInterval();
        PollIntervalChanged += SetPollInterval;
        TextIntervalChanged += SetTextInterval;

        void SetPollInterval() => PollTimer.Interval = PollInterval;
        void SetTextInterval() => TextTimer.Interval = TextInterval;
    }

    private void InitSearch()
    {
        SessionFilter.Apply += StartSearch;
        SearchingChanged += OnSearchingChanged;

        void OnSearchingChanged()
            => SessionFilter.CanApply = NotSearching;
    }

    private void InitSelect()
    {
        SessionList.SelectionChanged += OnSelectionChanged;
        SessionList.SelectionActivated += OnSelectionActivated;

        void OnSelectionChanged()
            => JoinGame.SessionId = SessionList.SelectedId ?? default;

        void OnSelectionActivated()
            => JoinGame.TryJoin();
    }

    private void InitStatus()
    {
        OnTreeEntered();
        TreeEntered += OnTreeEntered;
        TreeExiting += OnTreeExiting;

        void OnTreeEntered()
            => Network.Instance.CombinedStatus += StatusBar.SetStatus;

        void OnTreeExiting()
            => Network.Instance.CombinedStatus -= StatusBar.SetStatus;
    }

    #endregion

    #region Utils

    private static ulong GetTime()
        => Time.GetTicksMsec();

    private static ulong Millis(float s)
        => (ulong)TimeSpan.FromSeconds(s).TotalMilliseconds;

    #endregion

    #endregion
}
