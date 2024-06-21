using Godot;

namespace F00F;

[Tool]
public partial class StrEdit : LineEdit
{
    public new string Text { get; set => this.OnPreSet(ref field, Filter(value), OnPreSet, OnPostSet); } = string.Empty;

    #region Godot

    protected virtual void OnReady() { }
    public sealed override void _Ready()
    {
        Text = base.Text;
        TextChanged += text => Text = text;

        if (Editor.NotEditor)
            OnReady();
    }

    #endregion

    #region Private

    private int caret;
    protected virtual void OnPreSet()
        => caret = CaretColumn;

    protected virtual void OnPostSet()
    {
        base.Text = Text;
        CaretColumn = caret;
    }

    protected virtual string Filter(string value) => value;

    #endregion
}
