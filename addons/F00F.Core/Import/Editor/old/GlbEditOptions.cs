using System;

namespace F00F;

public partial class GlbOptions : CustomResource
{
    private string _Name;
    public string Name { get => _Name; set => this.Set(ref _Name, value, NameSet); }
    public Action NameSet;

    private string _Scene;
    public string Scene { get => _Scene; set => this.Set(ref _Scene, value, SceneSet); }
    public Action SceneSet;

    private GlbBodyType _BodyType = GlbBodyType.RigidBody;
    public GlbBodyType BodyType { get => _BodyType; set => this.Set(ref _BodyType, value, BodyTypeSet); }
    public Action BodyTypeSet;

    private GlbRotate _Rotate;
    public GlbRotate Rotate { get => _Rotate; set => this.Set(ref _Rotate, value, RotateSet); }
    public Action RotateSet;

    private float _MassMultiplier = 1;
    public float MassMultiplier { get => _MassMultiplier; set => this.Set(ref _MassMultiplier, value, MassMultiplierSet); }
    public Action MassMultiplierSet;

    private float _ScaleMultiplier = 1;
    public float ScaleMultiplier { get => _ScaleMultiplier; set => this.Set(ref _ScaleMultiplier, value, ScaleMultiplierSet); }
    public Action ScaleMultiplierSet;

    private GlbNode[] _Nodes = [];
    public GlbNode[] Nodes { get => _Nodes; set => this.Set(ref _Nodes, value, NodesSet); }
    public Action NodesSet;

    private bool _ImportOriginal;
    public bool ImportOriginal { get => _ImportOriginal; set => this.Set(ref _ImportOriginal, value, ImportOriginalSet); }
    public Action ImportOriginalSet;

    public partial class GlbNode : CustomResource
    {
        public string Name { get; set => this.Set(ref field, value, NameSet); }
        public Action NameSet;

        public GlbShapeType ShapeType { get; set => this.Set(ref field, value, ShapeTypeSet); } = GlbShapeType.MultiConvex;
        public Action ShapeTypeSet;

        public int MultiConvexLimit { get; set => this.Set(ref field, value, MultiConvexLimitSet); }
        public Action MultiConvexLimitSet;
    }
}
