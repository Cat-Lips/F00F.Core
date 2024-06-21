using System;
using Godot;

namespace F00F;

public partial class GUI
{
    [Export] private PackedScene MainMenu { get; set => this.Set(ref field, value, OnMainMenuSet); }

    public void InitGame(Network network, Action OnGameStart, Action OnGameEnd)
    {
        var mm = GetNodeOrNull("MainMenu") as IMainMenu;
        mm?.InitGame(network, OnGameStart, OnGameEnd);
    }

    private void OnMainMenuSet()
    {
        var mm = GetNodeOrNull("MainMenu");
        if (mm is not null) this.RemoveChild(mm, free: true);

        mm = MainMenu?.New(mm => mm.Name = "MainMenu");
        if (mm is not null) AddChild(mm, forceReadableName: true);
    }
}
