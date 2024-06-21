using System;
using System.Diagnostics;
using Godot;

namespace F00F
{
    [Tool]
    public partial class GlbEdit : ModelView
    {
        private Button AcceptBtn => GetNode<Button>("%Accept");
        private Button ApplyBtn => GetNode<Button>("%Apply");
        private Button RevertBtn => GetNode<Button>("%Revert");
        private Button AbortBtn => GetNode<Button>("%Abort");

        private Button AutoApply => GetNode<Button>("%AutoApply");
        private Button ViewSource => GetNode<Button>("%ViewSource");

        private Node Source { get; set; }
        private GlbOptions Config { get; set; }
        private Action<Node> OnComplete { get; set; }

        protected override void ClearScene(out Node scene)
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

            if (!Config.IsConnected(Resource.SignalName.Changed, UpdateButtonStates))
                Config.Changed += UpdateButtonStates;
        }

        private void UpdateButtonStates()
        {
            const bool hasChanges = true;
            AcceptBtn.Enabled(!hasChanges);
            ApplyBtn.Enabled(hasChanges);
            RevertBtn.Enabled(hasChanges);

            Debug.Assert(Config.IsConnected(Resource.SignalName.Changed, UpdateButtonStates));
            Config.Changed -= UpdateButtonStates;
        }

        private void DisableButtonStates()
        {
            AcceptBtn.Enabled(false);
            ApplyBtn.Enabled(false);
            RevertBtn.Enabled(false);

            if (Config.IsConnected(Resource.SignalName.Changed, UpdateButtonStates))
                Config.Changed -= UpdateButtonStates;
        }

        public override void _Ready()
        {
            base._Ready();

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
}
