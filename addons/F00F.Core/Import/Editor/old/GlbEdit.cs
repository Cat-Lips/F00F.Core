using System;
using Godot;

namespace F00F;

[Tool]
public partial class GlbEdit : ModelView
{
    private Button AcceptBtn => field ??= (Button)GetNode("%Accept");
    private Button ApplyBtn => field ??= (Button)GetNode("%Apply");
    private Button RevertBtn => field ??= (Button)GetNode("%Revert");
    private Button AbortBtn => field ??= (Button)GetNode("%Abort");

    private Button AutoApply => field ??= (Button)GetNode("%AutoApply");
    private Button ViewSource => field ??= (Button)GetNode("%ViewSource");

    private Node Source { get; set; }
    private GlbOptions Config { get; set; }
    private Action<Node> OnComplete { get; set; }

    protected sealed override void ClearScene(out Node scene)
    {
        DisableButtonStates();
        base.ClearScene(out scene);
        Config.Dispose(); Config = null;
        Source.QueueFree(); Source = null;
    }

    public void EditScene(Node source, Action<Node> done)
    {
        if (Scene is not null)
            ClearScene();

        Config = new();
        Source = source;
        OnComplete = done;

        UpdateEditScene();

        if (AutoApply.ButtonPressed) Config.Changed += UpdateEditScene;
        else Config.Changed -= UpdateEditScene;

    }

    private void UpdateEditScene()
    {
        if (ViewSource.ButtonPressed)
        {
            ViewScene(Source, null, GlbOptions.EditControls(out var SetData), SetData);
            DisableButtonStates();
        }
        else
        {
            var editScene = GLB.ApplyPhysics(Source, Config);
            ViewScene(editScene, Config, GlbOptions.EditControls(out var SetData), SetData);
            ResetButtonStates();
        }
    }

    private void ResetButtonStates()
    {
        const bool hasChanges = false;
        AcceptBtn.Enabled(!hasChanges);
        ApplyBtn.Enabled(hasChanges);
        RevertBtn.Enabled(hasChanges);

        Config.SafeConnect(Resource.SignalName.Changed, UpdateButtonStates);
    }

    private void UpdateButtonStates()
    {
        const bool hasChanges = true;
        AcceptBtn.Enabled(!hasChanges);
        ApplyBtn.Enabled(hasChanges);
        RevertBtn.Enabled(hasChanges);

        Config.SafeDisconnect(Resource.SignalName.Changed, UpdateButtonStates);
    }

    private void DisableButtonStates()
    {
        AcceptBtn.Enabled(false);
        ApplyBtn.Enabled(false);
        RevertBtn.Enabled(false);

        Config.SafeDisconnect(Resource.SignalName.Changed, UpdateButtonStates);
    }

    protected sealed override void OnReady1()
    {
        AcceptBtn.Pressed += OnAccept;
        ApplyBtn.Pressed += OnApply;
        RevertBtn.Pressed += OnRevert;
        AbortBtn.Pressed += OnAbort;

        this.AutoApply.Toggled += AutoApply;
        this.ViewSource.Toggled += ViewSource;

        void OnAccept()
        {
            ClearScene(out var scene);
            OnComplete(scene);
        }

        void OnApply()
            => UpdateEditScene();

        void OnRevert()
            => EditScene(Source, OnComplete);

        void OnAbort()
        {
            ClearScene();
            OnComplete(null);
        }

        void AutoApply(bool on)
        {
            if (on) Config.Changed += UpdateEditScene;
            else Config.Changed -= UpdateEditScene;
        }

        void ViewSource(bool _)
            => UpdateEditScene();
    }
}
