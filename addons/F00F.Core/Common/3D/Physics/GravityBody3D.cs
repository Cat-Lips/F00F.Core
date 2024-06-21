using Godot;

namespace F00F;

[Tool]
public partial class GravityBody3D : RigidBody3D
{
    private Area3D Area => field ??= (Area3D)GetNode(nameof(Area));

    [Export] public Model Model { get; set => this.Set(ref field, value, OnModelSet); }

    protected virtual void OnModelSet()
    {
        Model.Init(this, OnPostInit);

        void OnPostInit()
        {
            var bb = this.GetAabb();
            var center = bb.GetCenter();
            var gravity = bb.GetLongestAxisSize();

            Area.Gravity = gravity;
            Area.GravityPointCenter = center;
            Area.GravityPointUnitDistance = gravity;
        }
    }
}
