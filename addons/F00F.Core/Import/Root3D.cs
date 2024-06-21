using F00F.Core;
using Godot;

namespace F00F
{
    [Tool]
    public partial class Root3D : Node3D
    {
        protected virtual RigidBody3D Body => field ??= GetNodeOrNull<RigidBody3D>("Body");

        #region Export

        [Export] public PhysicsConfig Source { get; private set => this.Set(ref field, value ?? new(), OnSourceSet); } = new();

        #endregion

        #region Godot

        public override void _Ready()
            => Source.Init(Body);

        #endregion

        #region Private

        private void OnSourceSet()
        {
            if (IsNodeReady())
                Source.Init(Body);
        }

        #endregion
    }
}
