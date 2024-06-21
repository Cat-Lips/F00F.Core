using System;
using Godot;

namespace F00F.NEW;

[Tool]
public partial class SessionFilter : GridContainer
{
    #region Private

    private const string EnumStr = "EnumStr";
    private readonly Config<SessionFilter> cfg = new(false);

    #region Filter

    private Button SessionHostBtn => field ??= (Button)GetNode("%SessionHostBtn");
    private Button SessionNameBtn => field ??= (Button)GetNode("%SessionNameBtn");
    private Button DistanceFilterBtn => field ??= (Button)GetNode("%DistanceFilterBtn");
    private Button MaxSearchResultsBtn => field ??= (Button)GetNode("%MaxSearchResultsBtn");
    private CheckButton OpenOnlyToggle => field ??= (CheckButton)GetNode("%OpenOnlyToggle");

    private StrEdit SessionHostEdit => field ??= (StrEdit)GetNode("%SessionHostEdit");
    private StrEdit SessionNameEdit => field ??= (StrEdit)GetNode("%SessionNameEdit");
    private StrEdit DistanceFilterEdit => field ??= (StrEdit)GetNode("%DistanceFilterEdit");
    private IntEdit MaxSearchResultsEdit => field ??= (IntEdit)GetNode("%MaxSearchResultsEdit");
    private DistanceFilterEnum DistanceFilterEnum => field ??= (DistanceFilterEnum)GetNode("%DistanceFilterEnum");

    private SunkLabel SessionHostInactive => field ??= (SunkLabel)GetNode("%SessionHostInactive");
    private SunkLabel SessionNameInactive => field ??= (SunkLabel)GetNode("%SessionNameInactive");
    private SunkLabel DistanceFilterInactive => field ??= (SunkLabel)GetNode("%DistanceFilterInactive");
    private SunkLabel MaxSearchResultsInactive => field ??= (SunkLabel)GetNode("%MaxSearchResultsInactive");

    private string SessionHost => SessionHostBtn.ButtonPressed ? SessionHostEdit.Text : null;
    private string SessionName => SessionNameBtn.ButtonPressed ? SessionNameEdit.Text : null;
    private Network.DistanceFilter? DistanceFilter => DistanceFilterBtn.ButtonPressed ? TryGetDistanceFilter() : null;
    private int? MaxSearchResults => MaxSearchResultsBtn.ButtonPressed ? MaxSearchResultsEdit.Text.AsIntN() : null;
    private bool OpenOnly => OpenOnlyToggle.ButtonPressed;

    private Network.DistanceFilter? TryGetDistanceFilter()
        => ((string)DistanceFilterEdit.GetMeta(EnumStr)).TryEnum<Network.DistanceFilter>();

    private Network.SearchFilter NewFilter()
    {
        return new(
            SessionHost,
            SessionName,
            DistanceFilter,
            MaxSearchResults,
            OpenOnly);
    }

    #endregion

    private Label Title => field ??= (Label)GetNode("%Title");
    private Button Refresh => field ??= (Button)GetNode("%Refresh");

    #region Orientation

    private const int VCols = 2;
    private const int HCols = 17;
    private Control[] HSeps => field ??= [
        (Control)GetNode("Sep2"),
        (Control)GetNode("Sep3"),
        (Control)GetNode("Sep4"),
        (Control)GetNode("Sep5"),
        (Control)GetNode("Sep6")];
    private Control VTitleSep => field ??= (Control)GetNode("VTitleSep");
    private Control HTitleSep => field ??= (Control)GetNode("HTitleSep");

    #endregion

    #endregion

    #region Export

    public event Action CanApplyChanged;
    public event Action ApplyTextChanged;
    public event Action TitleTextChanged;
    public event Action OrientationChanged;

    [Export] public bool CanApply { get; set => this.Set(ref field, value, CanApplyChanged); } = true;
    [Export] public string ApplyText { get; set => this.Set(ref field, value, ApplyTextChanged); } = "Search";
    [Export] public string TitleText { get; set => this.Set(ref field, value, TitleTextChanged); } = "Filter By";
    [Export] public Orientation Orientation { get; set => this.Set(ref field, value, OrientationChanged); } = Orientation.Horizontal;

    #endregion

    public event Action Apply;
    public Network.SearchFilter Filter { get; private set; } = new();

    #region Godot

    public sealed override void _Ready()
    {
        Load();
        InitUI();
        InitUX();
        InitUY();
        InitUZ();
    }

    #endregion

    #region Private

    #region Init

    private void InitUI()
    {
        OnCanApplyChanged();
        OnApplyTextChanged();
        OnTitleTextChanged();
        OnOrientationChanged();

        CanApplyChanged += OnCanApplyChanged;
        ApplyTextChanged += OnApplyTextChanged;
        TitleTextChanged += OnTitleTextChanged;
        OrientationChanged += OnOrientationChanged;

        void OnCanApplyChanged() => Refresh.Enabled(CanApply);
        void OnApplyTextChanged() => Refresh.TooltipText = ApplyText;
        void OnTitleTextChanged() => Title.Text = TitleText;
        void OnOrientationChanged()
        {
            VTitleSep.Visible = Orientation is Orientation.Vertical;
            HTitleSep.Visible = Orientation is Orientation.Horizontal;
            HSeps.ForEach(x => x.Visible = Orientation is Orientation.Horizontal);
            Columns = Orientation is Orientation.Vertical ? VCols :
                      Orientation is Orientation.Horizontal ? HCols :
                      throw new NotImplementedException($"{Orientation}");
        }
    }

    private void InitUX()
    {
        SessionHostBtn.ButtonPressed = Filter.SessionHost.NotNullOrEmpty();
        SessionNameBtn.ButtonPressed = Filter.SessionName.NotNullOrEmpty();
        DistanceFilterBtn.ButtonPressed = Filter.DistanceFilter is not null;
        MaxSearchResultsBtn.ButtonPressed = Filter.MaxSearchResults is not null;
        OpenOnlyToggle.ButtonPressed = Filter.OpenOnly;

        OnSessionHostToggled(SessionHostBtn.ButtonPressed);
        OnSessionNameToggled(SessionNameBtn.ButtonPressed);
        OnDistanceFilterToggled(DistanceFilterBtn.ButtonPressed);
        OnMaxSearchResultsToggled(MaxSearchResultsBtn.ButtonPressed);

        SessionHostBtn.Toggled += OnSessionHostToggled;
        SessionNameBtn.Toggled += OnSessionNameToggled;
        DistanceFilterBtn.Toggled += OnDistanceFilterToggled;
        MaxSearchResultsBtn.Toggled += OnMaxSearchResultsToggled;

        void OnSessionHostToggled(bool on)
        {
            SessionHostEdit.Editable = on;
            SessionHostInactive.Visible = !on;
            if (on) SessionHostEdit.GrabFocus();
        }

        void OnSessionNameToggled(bool on)
        {
            SessionNameEdit.Editable = on;
            SessionNameInactive.Visible = !on;
            if (on) SessionNameEdit.GrabFocus();
        }

        void OnDistanceFilterToggled(bool on)
        {
            DistanceFilterEdit.Editable = on;
            DistanceFilterInactive.Visible = !on;
            if (on) DistanceFilterEdit.GrabFocus();
        }

        void OnMaxSearchResultsToggled(bool on)
        {
            MaxSearchResultsEdit.Editable = on;
            MaxSearchResultsInactive.Visible = !on;
            if (on) MaxSearchResultsEdit.GrabFocus();
        }
    }

    private void InitUY()
    {
        DistanceFilterEdit.EditingToggled += OnDistanceFilterEditing;
        DistanceFilterEnum.FocusExited += OnDistanceFilterFocusExited;

        void OnDistanceFilterEditing(bool editing)
        {
            if (editing)
            {
                DistanceFilterEnum.Visible = true;
                DistanceFilterEnum.Selected = TryGetDistanceFilter() ?? default;
            }
        }

        void OnDistanceFilterFocusExited()
        {
            DistanceFilterEnum.Visible = false;
            DistanceFilterEdit.Text = DistanceFilterEnum.Selected.Capitalise();
            DistanceFilterEdit.SetMeta(EnumStr, DistanceFilterEnum.Selected.AsStr());
        }
    }

    private void InitUZ()
    {
        Refresh.Pressed += OnRefreshPressed;

        void OnRefreshPressed()
        {
            Filter = NewFilter();
            Apply?.Invoke();
            Save();
        }
    }

    #endregion

    #region Load/Save

    private void Load()
    {
        Filter = new(
            cfg.Get(Filter.SessionHost),
            cfg.Get(Filter.SessionName),
            cfg.GetEnum(Filter.DistanceFilter),
            cfg.Get(Filter.MaxSearchResults),
            cfg.Get(Filter.OpenOnly));
    }

    private void Save()
    {
        cfg.Clear();
        cfg.Set(Filter.SessionHost);
        cfg.Set(Filter.SessionName);
        cfg.SetEnum(Filter.DistanceFilter);
        cfg.Set(Filter.MaxSearchResults);
        cfg.Set(Filter.OpenOnly);
        cfg.Save();
    }

    #endregion

    #endregion
}
