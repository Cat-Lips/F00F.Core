using System;
using Godot;

namespace F00F;

[Tool]
public partial class PlayerBody3D : RigidBody3D, IPlayer
{
    public event Action ActiveChanged;
    public bool Active { get; set => this.Set(ref field, value, ActiveChanged); }

    #region Export

    public PlayerInput Input { get; set => this.Set(ref field, value ?? new()); }
    [Export] public ModelConfig Model { get; set => this.Set(ref field, value ?? new(), OnModelSet); }
    [Export] public PlayerConfig Config { get; set => this.Set(ref field, value ?? new(), OnConfigSet); }

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
