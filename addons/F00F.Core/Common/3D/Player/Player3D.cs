using System;
using Godot;

namespace F00F;

[Tool]
public partial class Player3D : CharacterBody3D, IPlayer
{
    public event Action ActiveChanged;
    public bool Active { get; set => this.Set(ref field, value, ActiveChanged); }

    #region Export

    public PlayerInput Input { get; set => this.Set(ref field, value ?? new()); }
    [Export] public ModelConfig Model { get; set => this.Set(ref field, value ?? new(), OnModelSet); }
    [Export] public PlayerConfig Config { get; set => this.Set(ref field, value ?? new(), OnConfigSet); }

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

    protected virtual void ProcessPhysics(ref Vector3 velocity, float delta)
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

        ProcessPhysics(ref velocity, delta);

        Velocity = velocity;
        MoveAndSlide();
    }

    #endregion

    protected virtual void OnModelSet()
        => Model.Init(this);

    protected virtual void OnConfigSet()
        => Config.Init(this);
}
