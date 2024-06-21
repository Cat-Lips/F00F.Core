using Godot;

namespace F00F;

[Tool]
public partial class GravityBody3D : RigidBody3D
{
    #region Private

    private Area3D GravityArea => field ??= (Area3D)GetNode(nameof(GravityArea));

    #endregion

    #region Export

    [Export] public ModelConfig Model { get; set => this.Set(ref field, value ?? new(), OnModelSet); }

    #endregion

    #region Godot

    public GravityBody3D()
        => Inertia = Vector3.One;

    protected virtual void OnReady() { }
    public sealed override void _Ready()
    {
        OnReady();
        Model ??= new();
        Editor.Disable(this);
    }

    #endregion

    #region Private

    protected virtual void OnModelSet()
    {
        Model.Init(this, OnPostInit);

        void OnPostInit()
        {
            var bb = this.GetAabb();
            var center = bb.GetCenter();
            var gravity = bb.GetLongestAxisSize();

            GravityArea.Gravity = gravity;
            GravityArea.GravityPointCenter = center;
            GravityArea.GravityPointUnitDistance = gravity;
        }
    }

    #endregion
}
