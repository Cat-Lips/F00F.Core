using System;
using Godot;

namespace F00F;

public partial class Camera
{
    public event Action<Node3D> Select;
    public event Action SelectModeChanged;

    public bool SelectMode { get; set => this.Set(ref field, value, SelectModeChanged); }

    #region Private

    private bool _doRayCast;
    private Node3D _curTarget;
    private void InitSelectMode()
    {
        _doRayCast = false;
        _curTarget = Target;

        SelectMode = MyInput.Instance.MouseVisible = false;
        SelectModeChanged += () => MyInput.Instance.MouseVisible = SelectMode;
        MyInput.Instance.MouseVisibilityChanged += () => SelectMode = MyInput.Instance.MouseVisible;

        Select += SetFollowTarget;
        TargetChanged += OnTargetChanged;
        SelectModeChanged += SetTargetActive;

        SetTargetActive();

        void SetFollowTarget(Node3D target)
        {
            if (target is ITarget) Target = target;
            else if (Target is ITarget) Target = null;
        }

        void OnTargetChanged()
        {
            (_curTarget as IActive)?.Active = false;
            _curTarget = Target;
            (_curTarget as IActive)?.Active = true;

            SelectMode = false;
        }

        void SetTargetActive()
            => (Target as IActive)?.Active = !SelectMode;
    }

    private bool OnRayCamInput()
    {
        return
            this.Handle(RayCamInput.Toggle(), OnToggle) ||
            SelectMode && this.Handle(RayCamInput.Select(), OnSelect);

        void OnToggle()
            => SelectMode = !SelectMode;

        void OnSelect()
            => _doRayCast = true;
    }

    private void UpdateSelect()
    {
        if (_doRayCast)
        {
            PerformRayCast();
            _doRayCast = false;
        }

        void PerformRayCast()
        {
            var screenPos = Camera3D.GetViewport().GetMousePosition();
            var rayStart = Camera3D.ProjectRayOrigin(screenPos);
            var rayNormal = Camera3D.ProjectRayNormal(screenPos);
            var hitTarget = Camera3D.RayCastHitBody(rayStart, rayNormal, Camera3D.Far);
            this.CallDeferred(() => Select?.Invoke(hitTarget));
        }
    }

    #endregion
}
