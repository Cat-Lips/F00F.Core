using System.Collections.Generic;
using Godot;

namespace F00F;

[Tool]
public partial class WorldShader : Node3D
{
    #region Private

    private Godot.Camera3D Camera => field ??= GetViewport().GetCamera3D();

    #endregion

    #region Export

    [Export] public WorldConfig WorldConfig { get; set => this.Set(ref field, value ?? new(), OnWorldConfigSet); }
    [Export] public ChunkConfig ChunkConfig { get; set => this.Set(ref field, value ?? new(), OnChunkConfigSet); }

    #endregion

    #region Godot

    public sealed override void _Ready()
    {
        WorldConfig ??= new();
        ChunkConfig ??= new();
    }

    // Which _Process is better?

    //public sealed override void _Process(double _)
    //{
    //    if (Camera.NotNull())
    //        GlobalPosition = Camera.GlobalPosition;
    //}

    public sealed override void _Process(double _)
    {
        if (Camera is null) return;

        var camPos = Camera.GlobalPosition.XZ();
        var curPos = camPos.Snapped(ChunkConfig.Size).RoundInt();

        GlobalPosition = curPos.FromXZ();
    }

    //private Vector2I curPos;
    //private Vector2? camPos;
    //public sealed override void _Process(double _)
    //{
    //    if (Camera is null) return;

    //    var camPos = Camera.GlobalPosition.XZ();
    //    if (this.camPos == camPos) return;
    //    this.camPos = camPos;

    //    var curPos = camPos.Snapped(ChunkConfig.Size).RoundInt();
    //    if (this.curPos == curPos) return;
    //    this.curPos = curPos;

    //    GlobalPosition = curPos.FromXZ();
    //}

    #endregion

    #region Private

    private Dictionary<string, MeshInstance3D> Chunks { get; } = [];

    private void OnWorldConfigSet()
        => Chunks.Values.ForEach(x => x.MaterialOverride = WorldConfig?.ShaderMaterial);

    private void OnChunkConfigSet()
    {
        ChunkConfig.SafeInit(this, OnChunkConfigChanged);

        void OnChunkConfigChanged()
        {
            Chunks.Clear(x => this.RemoveChild(x.Value, free: true));

            foreach (var (cell, ring) in Utils.Spiral(ChunkConfig.Radius))
            {
                var pos = cell * ChunkConfig.Size;
                var lod = (ring - 1).ClampMin(0);
                var name = $"{cell}";

                var chunk = NewMesh(name, lod, pos);
                Chunks.Add(name, chunk);
                AddChild(chunk);
            }

            MeshInstance3D NewMesh(string name, int lod, in Vector2I pos)
            {
                return new()
                {
                    Name = name,
                    Mesh = Mesh(lod),
                    Position = pos.FromXZ(),
                    MaterialOverride = WorldConfig?.ShaderMaterial
                };

                Mesh Mesh(int lod) => ChunkConfig.Lod
                    ? New.PlaneMesh(ChunkConfig.Size, lod)
                    : New.PlaneMesh(ChunkConfig.Size);
            }
        }
    }

    #endregion
}
