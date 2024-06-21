using System;
using System.Linq;
using Godot;

namespace F00F;

using static F00F.TestArenaConfig.Enum;

[Tool]
public partial class TestArena : Node3D, IWorld
{
    #region Private

    private Camera3D Camera => field ??= GetViewport().GetCamera3D();
    private WorldBounds Bounds => field ??= (WorldBounds)GetNode("Bounds");

    private Node3D Stunts => field ??= (Node3D)GetNode("Stunts");

    private StaticBody3D Floor => field ??= (StaticBody3D)GetNode("Floor");
    private MeshInstance3D FloorMesh => field ??= (MeshInstance3D)Floor.GetNode("Mesh");
    private CollisionShape3D FloorShape => field ??= (CollisionShape3D)Floor.GetNode("Shape");
    private PlaneMesh FloorPlane => field ??= (PlaneMesh)FloorMesh.Mesh;

    private StaticBody3D Loop => field ??= (StaticBody3D)Stunts.GetNode("Loop");
    private MeshInstance3D LoopMesh => field ??= (MeshInstance3D)Loop.GetNode("Mesh");
    private CollisionShape3D LoopShape => field ??= (CollisionShape3D)Loop.GetNode("Shape");

    private StaticBody3D Ribs => field ??= (StaticBody3D)Stunts.GetNode("Ribs");
    private StaticBody3D Beams => field ??= (StaticBody3D)Stunts.GetNode("Beams");
    private CollisionShape3D[] BeamShapes => field ??= [.. Beams.GetChildren<CollisionShape3D>()];
    private MeshInstance3D[] BeamMeshes => field ??= [.. BeamShapes.Select(x => (MeshInstance3D)x.GetNode("Mesh"))];

    private StaticBody3D Ramp => field ??= (StaticBody3D)Stunts.GetNode("Ramp");
    private StaticBody3D Steps => field ??= (StaticBody3D)Stunts.GetNode("Steps");
    private MeshInstance3D StepsMesh => field ??= (MeshInstance3D)Steps.GetNode("Mesh");
    private CollisionShape3D StepsShape => field ??= (CollisionShape3D)Steps.GetNode("Shape");

    #endregion

    public event Action<Vector3> SpawnReady;
    public Vector3? SpawnPoint { get; private set; }

    #region Export

    [Export] public TestArenaConfig Config { get; set => this.Set(ref field, value ?? new(), OnConfigSet); }

    #endregion

    #region Godot

    public sealed override void _Ready()
    {
        InitLoop();
        Config ??= new();
        Editor.Disable(this);

        void InitLoop()
        {
            var mesh = New.StuntLoopMesh();
            var shape = New.StaticShape(mesh);

            LoopMesh.Mesh = mesh;
            LoopShape.Shape = shape;
        }
    }

    #endregion

    #region Private

    private void OnConfigSet()
    {
        if (this.IsReady())
        {
            InitBeams();
            InitSteps();
            ShowStunts();
            SetFloorSize();
            SetFloorShape();
            SetStuntsScale();
            ShowRibsOrBeams();
            ShowRampOrSteps();

            Config.FloorSizeSet.Action += SetFloorSize;
            Config.FloorShapeSet.Action += SetFloorShape;
            Config.ShowStuntsSet.Action += ShowStunts;
            Config.StuntsScaleSet.Action += SetStuntsScale;
            Config.BeamConfigChanged.Action += InitBeams;
            Config.StepConfigChanged.Action += InitSteps;
            Config.ShowRibsOrBeamsSet.Action += ShowRibsOrBeams;
            Config.ShowRampOrStepsSet.Action += ShowRampOrSteps;
        }

        void SetFloorSize()
        {
            SetFloorSize();
            SetFloorShape();
            SetSpawnReady();

            void SetFloorSize()
            {
                Bounds.Size = Vector2I.One * Config.FloorSize;
                FloorPlane.Size = Vector2.One * Config.FloorSize;
            }

            void SetSpawnReady()
            {
                SpawnPoint = new(0, 0, Config.FloorSize * .25f);
                SpawnReady?.Invoke(SpawnPoint.Value);
            }
        }

        void SetFloorShape()
        {
            FloorShape.Shape = Config.FloorShape switch
            {
                ShapeType.Plane => new WorldBoundaryShape3D(),
                ShapeType.Polygon => New.ConvexPlane(Config.FloorSize),
                ShapeType.Trimesh => New.ConcavePlane(Config.FloorSize),
                _ => throw new NotImplementedException(),
            };
        }

        void ShowStunts()
        {
            Stunts.Enabled(Config.ShowStunts);
            if (Config.ShowStunts) { ShowRibsOrBeams(); ShowRampOrSteps(); }
        }

        void SetStuntsScale()
            => Stunts.Scale = Vector3.One * Config.StuntsScale;

        void ShowRibsOrBeams()
        {
            Ribs.Enabled(Config.ShowRibsOrBeams is RibsOrBeams.Ribs);
            Beams.Enabled(Config.ShowRibsOrBeams is RibsOrBeams.Beams);
        }

        void ShowRampOrSteps()
        {
            Ramp.Enabled(Config.ShowRampOrSteps is RampOrSteps.Ramp);
            Steps.Enabled(Config.ShowRampOrSteps is RampOrSteps.Steps);
        }

        void InitBeams()
        {
            var count = BeamShapes.Length;
            var beamGap = Config.BeamGap;
            var beamWidth = Config.BeamWidth;
            var beamHeight = Config.BeamHeight;

            var totalWidth = count * beamWidth + (count - 1) * beamGap;
            var startZ = -totalWidth * .5f + beamWidth * .5f;

            var size = new Vector3(1f, beamHeight, beamWidth);
            var yPos = beamHeight * .5f;

            for (var i = 0; i < count; ++i)
            {
                var mesh = BeamMeshes[i];
                var shape = BeamShapes[i];

                var zPos = startZ + i * (beamWidth + beamGap);
                shape.Position = new Vector3(0f, yPos, zPos);

                ((BoxMesh)mesh.Mesh).Size = size;
                ((BoxShape3D)shape.Shape).Size = size;
            }
        }

        void InitSteps()
        {
            const float StepWidth = 1;

            var mesh = New.StepsMesh(Config.StepCount, StepWidth, Config.StepDepth, Config.StepHeight);
            var shape = New.StaticShape(mesh);

            StepsMesh.Mesh = mesh;
            StepsShape.Shape = shape;
        }
    }

    #endregion
}
