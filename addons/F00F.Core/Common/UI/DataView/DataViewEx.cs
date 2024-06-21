using Godot;

namespace F00F
{
    [Tool]
    public partial class DataViewEx : DataView
    {
        protected Label Title => GetNode<Label>("%Title");
    }
}
