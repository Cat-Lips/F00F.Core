using System;
using Godot;

namespace F00F;

[Tool]
public partial class Player3D : CharacterBody3D, IPlayer
{
    public event Action ActiveSet;
    public bool Active { get; set => this.Set(ref field, value, ActiveSet); }

    #region Export

    [Export] public PlayerInput Input { get; set => this.Set(ref field, value); }
    [Export] public PlayerModel Model { get; set => this.Set(ref field, value, OnModelSet); }
    [Export] public PlayerConfig Config { get; set => this.Set(ref field, value, OnConfigSet); }

    #endregion

    public bool Grounded { get; private set; }

    #region Godot

    protected virtual void OnReady() { }
    public sealed override void _Ready()
    {
        OnReady();
        Input ??= new();
        Model ??= new();
        Config ??= new();
        Editor.Disable(this);
    }

    protected virtual void OnPhysicsProcess(ref Vector3 velocity, float delta)
    {
        this.ApplyJump(ref velocity);
        this.ApplyGravity(ref velocity, delta);
        this.ApplyMovement(ref velocity);
    }

    public sealed override void _PhysicsProcess(double _delta)
    {
        var delta = (float)_delta;
        var velocity = Velocity;
        Grounded = IsOnFloor();

        OnPhysicsProcess(ref velocity, delta);

        Velocity = velocity;
        MoveAndSlide();
    }

    #endregion

    protected virtual void OnModelSet()
        => Model.Init(this);

    protected virtual void OnConfigSet()
        => Config.Init(this);
}
