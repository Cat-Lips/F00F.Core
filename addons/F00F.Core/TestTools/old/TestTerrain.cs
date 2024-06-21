//using Godot;

//namespace F00F;

//[Tool]
//public partial class TestTerrain : StaticBody3D
//{
//    #region Enums

//    public class Enum
//    {
//        public enum ShapeType
//        {
//            Trimesh,
//            HeightMap,
//            Polygon,
//        }
//    }

//    #endregion

//    #region Defaults

//    public class Default
//    {
//        public static int Size => 100;
//        public static int Amplitude => 25;
//        public static Enum.ShapeType ShapeType => Enum.ShapeType.Trimesh;
//        public static Color Color => Colors.ForestGreen;
//        public static FastNoiseLite Noise => New.Noise();
//    }

//    #endregion

//    #region Export

//    [Export] public int Size { get; set => this.Set(ref field, value.ClampMin(0), OnSizeSet); } = Default.Size;
//    [Export] public int Amplitude { get; set => this.Set(ref field, value, OnAmplitudeSet); } = Default.Amplitude;
//    [Export] public Enum.ShapeType ShapeType { get; set => this.Set(ref field, value, ResetShape); } = Default.ShapeType;
//    [Export(PropertyHint.ColorNoAlpha)] public Color Color { get; set => this.Set(ref field, value, OnColorSet); } = Default.Color;
//    [Export] public FastNoiseLite Noise { get; set => this.Set(ref field, value ?? Default.Noise, OnNoiseSet); } = Default.Noise;

//    #endregion

//    #region Godot

//    public sealed override void _Ready()
//    {

//    }

//    #endregion

//    //#region Godot

//    //public sealed override void _Ready()
//    //{
//    //    InitMesh();
//    //    InitBounds();
//    //    InitTextures();
//    //    SetShaderAmplitude();

//    //    void InitMesh()
//    //        => Mesh.Mesh = New.PlaneMesh(Size, material: Shader);

//    //    void InitBounds()
//    //        => Bounds.Size = Size;

//    //    void InitTextures()
//    //    {
//    //        SetShaderHeightMap(HeightTexture = New.NoiseTexture(Size, Noise));
//    //        SetShaderColorMap(ColorTexture = New.NoiseTexture(Size, Noise, colors: [Colors.Black, Color]));

//    //        HeightTexture.Changed += () =>
//    //        {
//    //            HeightData = null;
//    //            ResetShape();
//    //        };
//    //    }
//    //}

//    //#endregion

//    //#region Private

//    //private CollisionShape3D Shape => field ??= GetNode<CollisionShape3D>("Shape");
//    //private MeshInstance3D Mesh => field ??= Shape.GetNode<MeshInstance3D>("Mesh");
//    //private WorldsEnd Bounds => field ??= GetNode<WorldsEnd>("Bounds");

//    //private NoiseTexture2D HeightTexture { get; set; }
//    //private NoiseTexture2D ColorTexture { get; set; }
//    //private Image HeightData { get; set; }

//    //private void OnSizeSet()
//    //{
//    //    SetMeshSize();
//    //    SetBoundsSize();
//    //    SetTextureSizes();

//    //    void SetMeshSize()
//    //        => ((PlaneMesh)Mesh.Mesh).Size = Vector2.One * Size;

//    //    void SetBoundsSize()
//    //        => Bounds.Size = Size;

//    //    void SetTextureSizes()
//    //    {
//    //        SetSize(HeightTexture);
//    //        SetSize(ColorTexture);

//    //        void SetSize(NoiseTexture2D texture)
//    //        {
//    //            texture.Width = Size;
//    //            texture.Height = Size;
//    //        }
//    //    }
//    //}

//    //private void OnAmplitudeSet()
//    //{
//    //    SetShaderAmplitude();
//    //    ResetShape();
//    //}

//    //private void OnColorSet()
//    //    => ColorTexture.ColorRamp.SetColor(1, Color);

//    //private void OnNoiseSet()
//    //{
//    //    SetNoise(HeightTexture);
//    //    SetNoise(ColorTexture);

//    //    void SetNoise(NoiseTexture2D texture)
//    //        => texture.Noise = Noise;
//    //}

//    //private ShaderMaterial Shader => field ??= New.ShaderMaterial(this.LoadShader());
//    //private void SetShaderAmplitude(float? x = null) => Shader.SetShaderParameter("amplitude", x ?? Amplitude);
//    //private void SetShaderHeightMap(Texture2D x) => Shader.SetShaderParameter("heightmap", x);
//    //private void SetShaderColorMap(Texture2D x) => Shader.SetShaderParameter("colormap", x);

//    //private AutoAction _ResetShape;
//    //private void ResetShape()
//    //{
//    //    (_ResetShape ??= new(ResetShape)).Run();

//    //    void ResetShape()
//    //    {
//    //        Shape.Shape = ShapeType switch
//    //        {
//    //            Enum.ShapeType.Trimesh => NewTrimeshShape(),
//    //            Enum.ShapeType.HeightMap => NewHeightMapShape(),
//    //            Enum.ShapeType.Polygon => NewPolygonShape(),
//    //            _ => throw new System.NotImplementedException(),
//    //        };

//    //        ConcavePolygonShape3D NewTrimeshShape()
//    //        {
//    //            //var shape = New.TrimeshShape(Size);
//    //            var shape = new ConcavePolygonShape3D { Data = new[]; }
//    //            if (Amplitude is 0) return shape;

//    //            var data = shape.Data;
//    //            SetPolygonShape(Size, data);
//    //            shape.Data = data;
//    //            return shape;
//    //        }

//    //        HeightMapShape3D NewHeightMapShape()
//    //        {
//    //            var shape = New.PolygonShape(Size);
//    //            if (Amplitude is 0) return shape;

//    //            var data = shape.Points;
//    //            SetPolygonShape(Size, data);
//    //            shape.Points = data;
//    //            return shape;

//    //            var shape = new HeightMapShape3D();
//    //            shape.UpdateMapDataFromImage(HeightData ??= GetHeightData(), 0, Amplitude);
//    //            return shape;
//    //        }

//    //        ConvexPolygonShape3D NewPolygonShape()
//    //        {
//    //            var shape = New.PolygonShape(Size);
//    //            if (Amplitude is 0) return shape;

//    //            var data = shape.Points;
//    //            SetPolygonShape(Size, data);
//    //            shape.Points = data;
//    //            return shape;
//    //        }

//    //        Image GetHeightData()
//    //        {
//    //            var data = HeightTexture.GetImage();
//    //            data.Convert(Image.Format.R8);
//    //            return data;
//    //        }

//    //        void SetPolygonShape(int size, Vector3[] data)
//    //        {
//    //            data = Points().ToArray();

//    //            IEnumerable<Vector3> Points()
//    //            {
//    //                HeightData ??= GetHeightData();

//    //                for (var x = 0; x < size - 1; ++x)
//    //                {
//    //                    for (var z = 0; z < size - 1; ++z)
//    //                    {
//    //                        var y = HeightData.GetPixel(x, z).R;
//    //                        yield return new(x, y, z);
//    //                    }
//    //                }
//    //            }
//    //        }
//    //    }
//    //}

//    //#endregion
//}
