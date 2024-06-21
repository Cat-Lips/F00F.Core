using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Godot;

namespace F00F;

public partial class ProgressRoot : DataView
{
    public event Action ProgressChanged;
    public float Progress { get; private set => this.Set(ref field, value, ProgressChanged); }

    public event Action IsCompleteChanged;
    public bool IsComplete { get; private set => this.Set(ref field, value, IsCompleteChanged); }

    public event Action IsCancelledChanged;
    public bool IsCancelled { get; private set => this.Set(ref field, value, IsCancelledChanged); }

    public event Action HasErrorsChanged;
    public bool HasErrors { get; private set => this.Set(ref field, value, HasErrorsChanged); }

    public event Action CloseClicked;
    public event Action CancelClicked;

    private readonly Dictionary<string, float> myProgress = [];
    public void Add(string name, out SetProgress SetProgress)
    {
        CreateControls(out var progress);
        myProgress.Add(name, default);
        SetProgress = OnSetProgress;

        void CreateControls(out UI.IStatusBar progress)
            => Add(UI.NewStatusBar(name, out progress));

        void OnSetProgress(Status status, string msg, float? bytesOrPercent)
        {
            myProgress[name] = progress.Set(status, msg, bytesOrPercent);

            Progress = myProgress.Values.Average();
            Debug.Assert(Progress is >= 0 and <= 1);
            HasErrors |= status is Status.Error;
            IsComplete = Progress >= 1;
        }
    }

    protected sealed override void OnReady()
    {
        InitButtons();

        void InitButtons()
        {
            var btnClose = (Button)GetNode("%Close");
            var btnCancel = (Button)GetNode("%Cancel");

            SetButtonState();
            IsCompleteChanged += SetButtonState;
            IsCancelledChanged += SetButtonState;

            btnClose.Pressed += OnClosePressed;
            btnCancel.Pressed += OnCancelPressed;

            void SetButtonState()
            {
                btnClose.Enabled(IsComplete || IsCancelled);
                btnCancel.Enabled(!IsComplete && !IsCancelled);
            }

            void OnClosePressed()
            {
                Debug.Assert(IsComplete || IsCancelled);
                Debug.Assert(!btnClose.Disabled);
                btnClose.Disabled = true;
                CloseClicked?.Invoke();
            }

            void OnCancelPressed()
            {
                Debug.Assert(!IsComplete && !IsCancelled);
                Debug.Assert(!btnCancel.Disabled);
                btnCancel.Disabled = true;
                CancelClicked?.Invoke();
                IsCancelled = true;
            }
        }
    }
}
