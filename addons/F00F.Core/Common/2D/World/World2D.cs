using System;
using System.Linq;
using Godot;

namespace F00F;

[Tool]
public partial class World2D : Node
{
    #region Export

    [Export] public PackedScene[] Levels { get; private set => this.Set(ref field, value ?? [], () => Level = Level); } = [];

    #endregion

    public event Action LevelChanged;
    public int Level { get; private set => this.Set(ref field, value.Clamp(0, Levels.Length()), SetLevel, LevelChanged); }

    public TileMapLayer CurrentLevel { get; private set; }
    public Vector2[] SpawnPoints { get; private set; }

    public Vector2 GetSpawnPoint()
        => SpawnPoints.PickRandom();

    #region Godot

    protected virtual void OnReady() { }
    public sealed override void _Ready()
    {
        SetLevel();
        OnReady();
    }

    #endregion

    #region Private

    private void SetLevel()
    {
        NotifyClients();
        UnloadLevel();
        LoadLevel();

        void NotifyClients()
        {
            if (this.IsServer())
                Rpc(MethodName.SetRemoteLevel, Level);
        }

        void UnloadLevel()
        {
            CurrentLevel?.DetachChild(free: true);
            CurrentLevel = null;
            SpawnPoints = null;
        }

        void LoadLevel()
        {
            if (Levels.Length() is 0) return;

            AddChild(CurrentLevel = Levels[Level].New<TileMapLayer>());
            SpawnPoints = CurrentLevel.GetChildren<Marker2D>().Select(x => x.Position).ToArray();
            GetViewport().GetCamera2D().SetTileMap(CurrentLevel);
        }
    }

    #endregion

    #region RPC

    [Rpc]
    private void SetRemoteLevel(int level)
        => Level = level;

    #endregion
}
