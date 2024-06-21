using System;
using Godot;

namespace F00F
{
    public partial class GlbOptions : Resource
    {
        public static PackedScene DefaultScene { get; set; }

        public Action NameSet;
        public string Name { get => _Name; set => this.Set(ref _Name, value, NameSet); }
        private string _Name;

        public Action SceneSet;
        public PackedScene Scene { get => _Scene; set => this.Set(ref _Scene, value, SceneSet); }
        private PackedScene _Scene = DefaultScene;

        public Action BodyTypeSet;
        public GlbBodyType BodyType { get => _BodyType; set => this.Set(ref _BodyType, value, BodyTypeSet); }
        private GlbBodyType _BodyType = GlbBodyType.Rigid;

        public Action FrontFaceSet;
        public GlbFrontFace FrontFace { get => _FrontFace; set => this.Set(ref _FrontFace, value, FrontFaceSet); }
        private GlbFrontFace _FrontFace;

        public Action MassMultiplierSet;
        public float MassMultiplier { get => _MassMultiplier; set => this.Set(ref _MassMultiplier, value, MassMultiplierSet); }
        private float _MassMultiplier = 1;

        public Action ScaleMultiplierSet;
        public float ScaleMultiplier { get => _ScaleMultiplier; set => this.Set(ref _ScaleMultiplier, value, ScaleMultiplierSet); }
        private float _ScaleMultiplier = 1;

        public Action NodesSet;
        public GlbNode[] Nodes { get => _Nodes; set => this.Set(ref _Nodes, value, NodesSet); }
        private GlbNode[] _Nodes = [];

        public Action ImportOriginalSet;
        public bool ImportOriginal { get => _ImportOriginal; set => this.Set(ref _ImportOriginal, value, ImportOriginalSet); }
        private bool _ImportOriginal;

        public partial class GlbNode : Resource
        {
            public Action NameSet;
            public string Name { get => _Name; set => this.Set(ref _Name, value, NameSet); }
            private string _Name;

            public Action ShapeTypeSet;
            public GlbShapeType ShapeType { get => _ShapeType; set => this.Set(ref _ShapeType, value, ShapeTypeSet); }
            private GlbShapeType _ShapeType = GlbShapeType.MultiConvex;

            public Action MultiConvexLimitSet;
            public int MultiConvexLimit { get => _MultiConvexLimit; set => this.Set(ref _MultiConvexLimit, value, MultiConvexLimitSet); }
            private int _MultiConvexLimit;
        }
    }
}
