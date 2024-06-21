using Godot;

namespace F00F;

[Tool]
public partial class EnumEdit : OptionButton
{
    #region Private

    public EnumEdit()
        => Name = GetType().Name;

    #endregion
}
