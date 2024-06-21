using System.Collections.Generic;
using System.Linq;
using Godot;
using static Godot.AStarGrid2D;
using Cell = Godot.Vector2I;

namespace F00F;

using CellLookup = Godot.Collections.Dictionary<Node2D, Cell>;
using WeightLookup = Godot.Collections.Dictionary<string, float>;

[Tool]
public partial class NavGrid2D : Node
{
    #region Private

    private AStarGrid2D NavGrid { get; set; }

    private CellLookup CellsToAvoid { get; } = [];
    private CellLookup CellsToAttack { get; } = [];
    private CellLookup CellsThatBlock { get; } = [];

    #endregion

    #region Export

    [Export] public TileMapLayer TileMap { get; set => this.Set(ref field, value, Reset); }
    [Export] public DiagonalModeEnum DiagonalMode { get; set => this.Set(ref field, value, Reset); } = DiagonalModeEnum.Never;

    [Export(PropertyHint.TypeString)] public string[] Avoid { get; set => this.Set(ref field, value, Reset); }
    [Export(PropertyHint.TypeString)] public string[] Attack { get; set => this.Set(ref field, value, Reset); }
    [Export(PropertyHint.TypeString)] public string[] Blocking { get; set => this.Set(ref field, value, Reset); }
    [Export(PropertyHint.TypeString)] public string[] NonBlocking { get; set => this.Set(ref field, value, Reset); }
    [Export(PropertyHint.TypeString)] public WeightLookup MovementWeights { get; set => this.Set(ref field, value, Reset); }

    #endregion

    public IList<Cell> GetPathToClosestTarget(Cell start)
    {
        return CellsToAttack.Values
            .Select(x => NavGrid.GetIdPath(start, x))
            .Where(x => x.Count is not 0)
            .OrderBy(x => x.Count)
            .FirstOrDefault();
    }

    #region Godot

    public sealed override void _Ready()
    {
        Editor.Disable(this);
        TileMap ??= GetParentOrNull<TileMapLayer>();
    }

    public sealed override void _PhysicsProcess(double _delta)
    {

    }

    #endregion

    #region Private

    private AutoAction _Reset;
    private void Reset()
    {
        if (Editor.IsEditor) return;
        (_Reset ??= new(Reset)).Run();

        void Reset()
        {
            NavGrid = new();
            if (TileMap is null) return;

            NavGrid.Region = TileMap.GetUsedRect();
            NavGrid.CellSize = TileMap.TileSet.TileSize;
            NavGrid.DiagonalMode = DiagonalMode;
            NavGrid.Update();

            AddTiles();
            AddScenes();

            void AddTiles()
            {
                TileMap.ForEachCell(cell =>
                {
                    if (TileMap.HasTile(cell))
                    {
                        if (NonBlocking(TileMap, cell)) return;
                        if (HasWeight(TileMap, cell, out var weight))
                            NavGrid.SetPointWeightScale(cell, weight);
                        else NavGrid.SetPointSolid(cell);
                    }
                });

                bool NonBlocking(TileMapLayer tileMap, in Cell cell)
                    => tileMap.HasTag(cell, this.NonBlocking);

                bool HasWeight(TileMapLayer tileMap, in Cell cell, out float weight)
                {
                    var weights = tileMap.GetTags(cell, MovementWeights?.Keys);
                    if (!weights.IsNullOrEmpty())
                    {
                        weight = weights.Max(x => MovementWeights[x]);
                        return true;
                    }
                    else
                    {
                        weight = default;
                        return false;
                    }
                }
            }

            void AddScenes()
            {
                var avoid = new HashSet<string>(Avoid);
                var attack = new HashSet<string>(Attack);
                var blocking = new HashSet<string>(Blocking);

                TileMap.ForEachChild(AddScene);
                TileMap.ChildEnteredTree += AddScene;
                TileMap.ChildExitingTree += RemoveScene;

                void AddScene(Node _scene)
                {
                    if (_scene is Node2D scene)
                    {
                        var type = scene.GetType().Name;
                        if (avoid.Contains(type)) AddNodeToAvoid(scene);
                        else if (attack.Contains(type)) AddNodeToAttack(scene);
                        else if (blocking.Contains(type)) AddBlockingNode(scene);
                    }

                    void AddNodeToAvoid(Node2D scene)
                    {
                        var cell = TileMap.LocalToMap(scene.Position);
                        CellsToAvoid.Add(scene, cell);
                    }

                    void AddNodeToAttack(Node2D scene)
                    {
                        var cell = TileMap.LocalToMap(scene.Position);
                        CellsToAttack.Add(scene, cell);
                    }

                    void AddBlockingNode(Node2D scene)
                    {
                        var cell = TileMap.LocalToMap(scene.Position);
                        CellsThatBlock.Add(scene, cell);
                        NavGrid.SetPointSolid(cell);
                    }
                }

                void RemoveScene(Node _scene)
                {
                    if (_scene is Node2D scene)
                    {
                        var type = scene.GetType().Name;
                        if (avoid.Contains(type)) RemoveNodeToAvoid(scene);
                        else if (attack.Contains(type)) RemoveNodeToAttack(scene);
                        else if (blocking.Contains(type)) RemoveBlockingNode(scene);
                    }

                    void RemoveNodeToAvoid(Node2D scene)
                    {
                        var cell = TileMap.LocalToMap(scene.Position);
                        CellsToAvoid.Add(scene, cell);
                    }

                    void RemoveNodeToAttack(Node2D scene)
                    {
                        var cell = TileMap.LocalToMap(scene.Position);
                        CellsToAttack.Add(scene, cell);
                    }

                    void RemoveBlockingNode(Node2D scene)
                    {
                        var cell = TileMap.LocalToMap(scene.Position);
                        NavGrid.SetPointSolid(cell, false);
                        CellsThatBlock.Remove(scene);
                    }
                }
            }
        }
    }

    #endregion
}
