using Godot;
using static F00F.NEW.Network;

namespace F00F.NEW;

public partial class SessionFilter : Container
{
    #region Private

    private readonly Config<SessionFilter> cfg = new(false);

    private Button SessionNameBtn => field ??= (Button)GetNode(nameof(SessionNameBtn));
    private Button SessionOwnerBtn => field ??= (Button)GetNode(nameof(SessionOwnerBtn));
    private Button DistanceFilterBtn => field ??= (Button)GetNode(nameof(DistanceFilterBtn));
    private Button MaxSearchResultsBtn => field ??= (Button)GetNode(nameof(MaxSearchResultsBtn));

    private StrEdit SessionNameEdit => field ??= (StrEdit)GetNode(nameof(SessionNameEdit));
    private StrEdit SessionOwnerEdit => field ??= (StrEdit)GetNode(nameof(SessionOwnerEdit));
    private StrEdit DistanceFilterEdit => field ??= (StrEdit)GetNode(nameof(DistanceFilterEdit));
    private IntEdit MaxSearchResultsEdit => field ??= (IntEdit)GetNode(nameof(MaxSearchResultsEdit));
    private DistanceFilterSelect DistanceFilterSelect => field ??= (DistanceFilterSelect)GetNode(nameof(DistanceFilterSelect));

    private CheckButton OpenOnly => field ??= (CheckButton)GetNode(nameof(OpenOnly));

    #endregion

    public SearchFilter Get()
    {
        Store(out var filter);
        Save(); return filter;

        void Store(out SearchFilter filter)
        {
            filter = new(
                SessionName: SessionNameBtn.ButtonPressed ? SessionNameEdit.Text : null,
                SessionOwner: SessionOwnerBtn.ButtonPressed ? SessionOwnerEdit.Text : null,
                DistanceFilter: DistanceFilterBtn.ButtonPressed ? DistanceFilterEdit.Text.AsEnum<DistanceFilter>() : null,
                MaxSearchResults: MaxSearchResultsBtn.ButtonPressed ? MaxSearchResultsEdit.Value : null,
                OpenOnly: OpenOnly.ButtonPressed);
        }

        void Save()
        {
            cfg.Clear();
            if (filter.SessionName.NotNullOrEmpty()) cfg.Set("SessionName", filter.SessionName);
            if (filter.SessionOwner.NotNullOrEmpty()) cfg.Set("SessionOwner", filter.SessionOwner);
            if (filter.DistanceFilter.HasValue) cfg.Set("DistanceFilter", filter.DistanceFilter.Value.AsStr());
            if (filter.MaxSearchResults.HasValue) cfg.Set("MaxSearchResults", filter.MaxSearchResults.Value);
            if (filter.OpenOnly) cfg.Set("OpenOnly", filter.SessionName);
            cfg.Save();
        }
    }

    #region Godot

    public sealed override void _Ready()
    {
        Load(out var dflt);
        InitToggles();
        InitControls();

        void Load(out SearchFilter dflt)
        {
            dflt = new(
                SessionName: cfg.Has("SessionName") ? cfg.Get<string>("SessionName") : null,
                SessionOwner: cfg.Has("SessionOwner") ? cfg.Get<string>("SessionOwner") : null,
                DistanceFilter: cfg.Has("DistanceFilter") ? cfg.Get<string>("DistanceFilter").AsEnum<DistanceFilter>() : null,
                MaxSearchResults: cfg.Has("MaxSearchResults") ? cfg.Get<int>("MaxSearchResults") : null,
                OpenOnly: cfg.Has("OpenOnly") && cfg.Get<bool>("OpenOnly"));
        }

        void InitToggles()
        {
            SessionNameBtn.Toggled += OnSessionNameToggled;
            SessionOwnerBtn.Toggled += OnSessionOwnerToggled;
            DistanceFilterBtn.Toggled += OnDistanceFilterToggled;
            MaxSearchResultsBtn.Toggled += OnMaxSearchResultsToggled;
            OnSessionNameToggled(SessionNameBtn.ButtonPressed);
            OnSessionOwnerToggled(SessionOwnerBtn.ButtonPressed);
            OnDistanceFilterToggled(DistanceFilterBtn.ButtonPressed);
            OnMaxSearchResultsToggled(MaxSearchResultsBtn.ButtonPressed);

            void OnSessionNameToggled(bool on)
            {
                SessionNameEdit.Editable = on;
                SessionNameEdit.Text = on ? dflt.SessionName : null;
            }

            void OnSessionOwnerToggled(bool on)
            {
                SessionOwnerEdit.Editable = on;
                SessionOwnerEdit.Text = on ? dflt.SessionOwner : null;
            }

            void OnDistanceFilterToggled(bool on)
            {
                DistanceFilterEdit.Editable = on;
                DistanceFilterEdit.Text = on ? dflt.DistanceFilter?.Str() : null;
            }

            void OnMaxSearchResultsToggled(bool on)
            {
                MaxSearchResultsEdit.Editable = on;
                MaxSearchResultsEdit.Text = on ? dflt.MaxSearchResults?.ToString() : null;
            }
        }

        void InitControls()
        {
            DistanceFilterEdit.EditingToggled += OnDistanceFilterEditing;
            DistanceFilterSelect.FocusExited += OnDistanceFilterSelectFocusExited;

            void OnDistanceFilterEditing(bool editing)
            {
                if (editing) DistanceFilterSelect.Visible = true;
            }

            void OnDistanceFilterSelectFocusExited()
            {
                DistanceFilterSelect.Visible = false;
                DistanceFilterEdit.Text = DistanceFilterSelect.Selected.Str();
            }
        }
    }

    #endregion
}
