using Godot;

namespace F00F
{
    [Tool]
    public partial class DataView : Root
    {
        public GridContainer Grid => GetNode<GridContainer>("%Grid");

        public void AddSep()
        {
            Grid.AddChild(UI.NewSep(), forceReadableName: true);
            Grid.AddChild(UI.NewSep(), forceReadableName: true);
        }

        public void Add(Control value)
        {
            Grid.AddChild(UI.NewLabel(value.Name), forceReadableName: true);
            Grid.AddChild(value, forceReadableName: true);
        }

        public void SetDisplayName(string name, string displayName)
            => Grid.GetNode<Label>($"{name}Label").Text = displayName;
    }
}
