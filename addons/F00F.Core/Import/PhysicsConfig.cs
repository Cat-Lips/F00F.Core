using System.Collections.Generic;
using System.Linq;
using Godot;

namespace F00F.Core
{
    [Tool, GlobalClass]
    public partial class PhysicsConfig : CustomResource
    {
        #region Export

        [Export] public PackedScene Model { get; set => this.Set(ref field, value, notify: true); }
        [Export] public GlbRotate Rotate { get; set => this.Set(ref field, value); }

        [Export(PropertyHint.Range, "0,1,or_greater")] public float MassMultiplier { get; set => this.Set(ref field, value); } = 1;
        [Export(PropertyHint.Range, "0,1,or_greater")] public float ScaleMultiplier { get; set => this.Set(ref field, value); } = 1;

        [Export] public PartConfig[] Parts { get; private set => this.Set(ref field, value ?? []); } = [];

        #endregion

        private RigidBody3D root;
        public void Init(RigidBody3D root)
        {
            this.root = root;

            GenerateParts();
            Changed += ClearParts;
            Changed += GenerateParts;
        }

        public void OnPreSave()
        {
            ClearParts();
            GLB.RemoveMeta(root);
        }

        public void OnPostSave()
            => GenerateParts();

        #region Private

        private void GenerateParts()
        {
            if (root is null) return;

            var model = Model?.Instantiate();
            var gform = root.GlobalTransform();
            root.GlobalTransform(Transform3D.Identity);

            DisableChangedEvent();
            AddParts();
            EnableChangedEvent();

            root.GlobalTransform(gform);
            root.Freeze = model is null;
            model?.QueueFree();

            void AddParts()
            {
                var oldParts = Parts.ToDictionary(x => x.Path);
                var newParts = new Dictionary<NodePath, PartConfig>();

                GLB.AddParts(own: true, root, model, Rotate, MassMultiplier, ScaleMultiplier,
                    GetPartName, GetShapeType, GetShapeCount, SetShapeCount, OnPartAdded);

                Parts = [.. newParts.Values];

                string GetPartName(MeshInstance3D source)
                    => GetPart(source).Name;

                GlbShapeType GetShapeType(MeshInstance3D source)
                    => GetPart(source).Shape;

                int GetShapeCount(MeshInstance3D source)
                    => GetPart(source).MaxShapes;

                void SetShapeCount(MeshInstance3D source, int count)
                    => GetPart(source).MaxShapes = count;

                void OnPartAdded(MeshInstance3D source, Node3D part)
                {
                    part.SetMeta(IsGenerated, true);

                    var cfg = GetPart(source);
                    part.Visible = cfg.Visible;

                    if (part is CollisionShape3D shape)
                        shape.Disabled = cfg.Disabled;
                }

                PartConfig GetPart(MeshInstance3D source)
                {
                    var path = source.Owner.GetPathTo(source);

                    if (newParts.TryGetValue(path, out var part))
                        return part;

                    if (!oldParts.Remove(path, out part))
                        part = NewPart();

                    newParts.Add(path, part);
                    return part;

                    PartConfig NewPart() => new()
                    {
                        Path = path,
                        Name = GLB.DefaultPartName(source),
                        Shape = GLB.DefaultShapeType(source),
                    };
                }
            }
        }

        private void ClearParts()
        {
            root?.RecurseChildren()
                .Where(x => x.HasMeta(IsGenerated))
                .ForEach(x => x.GetParent().RemoveChild(x, free: true));
        }

        private static readonly StringName IsGenerated = "_PhysicsConfig_IsGenerated_";

        #endregion
    }
}
