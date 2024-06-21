using System;
using Godot;

namespace F00F;

[Tool]
public partial class PlayerBody3D : RigidBody3D, IPlayer
{
    public event Action ActiveSet;
    public bool Active { get; set => this.Set(ref field, value, ActiveSet); }

    #region Export

    [Export] public PlayerInput Input { get; set => this.Set(ref field, value); }
    [Export] public PlayerModel Model { get; set => this.Set(ref field, value, OnModelSet); }
    [Export] public PlayerConfig Config { get; set => this.Set(ref field, value, OnConfigSet); }

    #endregion

    public bool Grounded { get; private set; } // TODO
    public Vector3 Velocity { get; private set; } // TODO

    #region Godot

    protected virtual void OnReady() { }
    public sealed override void _Ready()
    {
        OnReady();
        Input ??= new();
        Model ??= new();
        Config ??= new();
        Editor.Disable(this);
        this.InitCollisions();
    }

    #endregion

    protected virtual void OnModelSet()
        => Model.Init(this);

    protected virtual void OnConfigSet()
        => Config.Init(this);
}
