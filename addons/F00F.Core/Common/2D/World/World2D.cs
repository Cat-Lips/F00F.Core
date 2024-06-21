using System;
using System.Collections.Generic;
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
    public List<Vector2> SpawnPoints { get; private set; }

    public virtual Vector2 GetSpawnPoint()
        => SpawnPoints.PickRandom();

    public virtual void Reset()
        => SetLevel();

    #region Godot

    protected virtual void OnReady() { }
    public sealed override void _Ready()
    {
        Editor.Disable(this);
        OnReady();
    }

    #endregion

    #region Protected

    protected virtual IEnumerable<Vector2> GetSpawnPoints(TileMapLayer lvl)
        => (lvl.GetNodeOrNull("SpawnPoints") ?? lvl).GetChildren<Marker2D>().Select(x => x.Position);

    protected virtual void InitLevel(TileMapLayer lvl) { }

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

            CurrentLevel = Levels[Level].New<TileMapLayer>();
            SpawnPoints = [.. GetSpawnPoints(CurrentLevel)];

            if (SpawnPoints.Count is 0)
                SpawnPoints.Add(CurrentLevel.GetWorldRect().GetCenter());

            AddChild(CurrentLevel);
            InitLevel(CurrentLevel);

            GetViewport().GetCamera2D().FitToWorld(CurrentLevel.GetWorldRect());
        }
    }

    #endregion

    #region RPC

    [Rpc]
    private void SetRemoteLevel(int level)
        => Level = level;

    #endregion
}
