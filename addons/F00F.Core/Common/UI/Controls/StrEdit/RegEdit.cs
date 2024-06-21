using Godot;

namespace F00F;

[Tool]
public partial class RegEdit : StrEdit
{
    public string FilterPattern { get; set => this.Set(ref field, TrySetRegex(value)); }

    #region Private

    private RegEx RegEx;
    private string TrySetRegex(string regex)
    {
        var x = new RegEx();
        if (x.Compile(regex).IsOk())
        {
            RegEx = x;
            return regex;
        }
        else
        {
            RegEx = null;
            return null;
        }
    }

    protected override string Filter(string value)
        => RegEx?.Sub(value, string.Empty, all: true) ?? value;

    #endregion
}
