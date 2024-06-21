using System;
using Godot;

namespace F00F.NEW;

public partial class Test3D
{
    #region Private

    private TestArena arena;
    private TestTerrain terrain;

    #endregion

    #region Export

    protected event Action WorldTypeChanged;
    protected enum TestWorldType { None, Arena, Terrain }
    [Export] protected TestWorldType WorldType { get; set => this.Set(ref field, value, WorldTypeChanged); } = TestWorldType.Arena;

    #endregion

    #region Private

    private void InitWorld()
    {
        LoadWorld();
        OnWorldTypeChanged();
        WorldTypeChanged += OnWorldTypeChanged;

        void OnWorldTypeChanged()
        {
            DestroyWorld();
            CreateWorld();
            SaveWorld();

            void DestroyWorld()
            {
                arena = null;
                terrain = null;
                ResetWorld();
            }

            void CreateWorld()
            {
                switch (WorldType)
                {
                    case TestWorldType.None:
                        InitWorld(Vector3.Zero);
                        break;
                    case TestWorldType.Arena:
                        CreateWorld(ref arena);
                        break;
                    case TestWorldType.Terrain:
                        CreateWorld(ref terrain);
                        break;
                }

                void CreateWorld<T>(ref T world) where T : Node, IWorld
                {
                    world = Preload.New<T>();
                    World.AddChild(world);

                    if (world.SpawnPoint.HasValue)
                        InitWorld(world.SpawnPoint.Value);

                    var curWorld = world;
                    world.SpawnReady += OnSpawnReady;

                    void OnSpawnReady(Vector3 spawn)
                    {
                        ClearWorld(curWorld);
                        InitWorld(spawn);
                    }
                }

                void InitWorld(in Vector3 spawn)
                {
                    this.InitWorld(spawn);
                    InitCamera(spawn);
                }
            }

            #region Utils

            void ResetWorld()
                => World.RemoveChildren(x => x.Owner is null);

            void ClearWorld(Node world)
                => World.RemoveChildren(x => x.Owner is null && x != world);

            #endregion
        }

        #region Load/Save

        void LoadWorld()
        {
            if (Editor.IsEditor) return;
            WorldType = LoadEnum(WorldType);
        }

        void SaveWorld()
        {
            if (Editor.IsEditor) return;
            SaveEnum(WorldType);
        }

        #endregion
    }

    #endregion
}
