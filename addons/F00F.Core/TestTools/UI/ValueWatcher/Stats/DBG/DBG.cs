using Godot;

namespace F00F;

[Tool]
public partial class DBG : Stats
{
    #region Export

    [Export] public Node3D Player { get; set => this.Set(ref field, value, OnPlayerSet); }

    #endregion

    #region Godot

    //public DBG()
    //    => Visible = Editor.IsEditor;

    public sealed override void _UnhandledKeyInput(InputEvent e)
    {
        if (this.Handle(e.IsActionPressed(MyInput.Show, exactMatch: true), () => Visible = !Visible)) return;
        if (this.Handle(e.IsActionPressed(MyInput.ShowMore, exactMatch: true), () => ShowMore = !ShowMore)) return;
    }

    #endregion

    #region Private

    private void OnPlayerSet()
    {
        Player.SafeInit(AddPlayerStats, ClearPlayerStats);

        void AddPlayerStats()
        {
            Sep("Player");
            Add(" - Position", () => Player.GlobalPosition.Rounded());
            Add(" - Rotation", () => Player.RotationDegrees.Rounded());
        }

        void ClearPlayerStats()
            => Clear();
    }

    #endregion
}
