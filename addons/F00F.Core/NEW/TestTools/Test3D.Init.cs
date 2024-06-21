using Godot;

namespace F00F.NEW;

public partial class Test3D
{
    private TestArena arena;
    private TestTerrain terrain;

    private void InitWorld()
    {
        if (this.IsReady())
        {
            DestroyWorld();
            CreateWorld();
            SaveWorld();
        }

        void DestroyWorld()
        {
            arena = null; terrain = null;
            World.RemoveChildren(x => x.Owner is null);
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
                    PurgeWorld();
                    InitWorld(spawn);

                    void PurgeWorld()
                        => World.RemoveChildren(x => x != curWorld);
                }
            }

            void InitWorld(in Vector3 spawn)
            {
                this.InitWorld(spawn);
                InitCamera(spawn);
            }
        }

        void SaveWorld()
        {
            if (Editor.IsEditor) return;
            SaveEnum(WorldType);
        }
    }
}
