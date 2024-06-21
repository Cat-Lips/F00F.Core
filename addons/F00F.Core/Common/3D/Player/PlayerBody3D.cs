using System;
using Godot;

namespace F00F;

[Tool]
public partial class PlayerBody3D : RigidBody3D, IActive, ITarget
{
    public event Action ActiveSet;
    public bool Active { get; set => this.Set(ref field, value, ActiveSet); }

    #region Export

    [Export] private PlayerInput Input { get; set => this.Set(ref field, value ?? new()); }
    [Export] private PlayerModel Model { get; set => this.Set(ref field, value ?? new(), OnModelSet); }

    #endregion

    #region Godot

    protected virtual void OnReady() { }
    public sealed override void _Ready()
    {
        OnReady();
        Input ??= new();
        Model ??= new();
        Editor.Disable(this);
    }

    #endregion

    protected virtual void OnModelSet()
        => Model.Init(this);
}
