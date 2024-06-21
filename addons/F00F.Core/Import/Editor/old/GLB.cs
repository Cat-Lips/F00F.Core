using System.Linq;
using Godot;

namespace F00F;

public static partial class GLB
{
    public static Node ApplyPhysics(Node source, GlbOptions options)
    {
        if (options.ImportOriginal)
            return source;

        var optIdx = 0;
        var nodeOptions = options.Nodes.ToDictionary(x => x.Name, x => (Opt: x, Idx: 0));

        CreateRoot(out var root);

        AddParts(
            root,
            owner: null,
            source,
            MeshMode.Dynamic,
            options.Rotate,
            options.MassMultiplier,
            options.ScaleMultiplier,
            shapeScaleMultiplier: 1,
            OnPartAdded: null,
            GetShapeCount: x => NodeOptions(x).MultiConvexLimit,
            SetShapeCount: (x, v) => NodeOptions(x).MultiConvexLimit = v,
            GetPartName: x => NodeOptions(x).Name,
            GetPartShape: x => NodeOptions(x).ShapeType,
            GetBoundingShape: null);

        options.Nodes = nodeOptions.Values
            .Where(x => x.Idx is not 0)
            .OrderBy(x => x.Idx)
            .Select(x => x.Opt)
            .ToArray();

        return root;

        void CreateRoot(out Node root)
        {
            root = LoadScene(options.Scene) ?? CreateBody(options.BodyType);
            root.Name = options.Name ??= source.Name;
        }

        GlbOptions.GlbNode NodeOptions(Node part)
        {
            if (!nodeOptions.TryGetValue(part.Name, out var options))
                options = (new() { Name = part.Name }, 0);

            if (options.Idx is 0)
            {
                options.Idx = ++optIdx;
                nodeOptions[options.Opt.Name] = options;
            }

            return options.Opt;
        }
    }

    //#region Variations

    //public static Node ApplyPhysics(
    //    bool own,
    //    Node source,
    //    Node parent = null,
    //    Func<Node, string> GetRootType = null,
    //    Func<Node, string> GetRootName = null,
    //    Func<Node, GlbRotate> GetRotation = null,
    //    Func<Node, float> GetMassMultiplier = null,
    //    Func<Node, float> GetScaleMultiplier = null,
    //    Func<MeshInstance3D, string> GetPartName = null,
    //    Func<MeshInstance3D, GlbShapeType> GetShapeType = null,
    //    Func<MeshInstance3D, int> GetShapeCount = null,
    //    Action<MeshInstance3D, int> SetShapeCount = null,
    //    Action<Node3D> OnPartAdded = null)
    //{
    //    if (source is null) return null;

    //    var root = CreateRoot();
    //    if (root is not null)
    //    {
    //        GetRootName ??= Default.GetRootName;
    //        GetRotation ??= Default.GetRotation;
    //        GetMassMultiplier ??= Default.GetMassMultiplier;
    //        GetScaleMultiplier ??= Default.GetScaleMultiplier;

    //        var owner = parent ?? root;
    //        parent?.AddChild(root, owner);
    //        root.Name = GetRootName(source);

    //        AddParts(root, owner, source, MeshMode.Dynamic,
    //            GetRotation(source), GetMassMultiplier(source), GetScaleMultiplier(source),
    //            GetPartName, GetShapeType, GetShapeCount, SetShapeCount, OnPartAdded);
    //    }

    //    source.Free();
    //    return root;

    //    Node CreateRoot()
    //    {
    //        var rootType = GetRootType?.Invoke(source) ?? Default.RootType;
    //        return Enum.TryParse(rootType, out GlbBodyType bodyType)
    //            ? CreateBody(bodyType) : LoadScene(rootType);
    //    }
    //}

    //public static Node ApplyPhysics(
    //    bool own,
    //    PackedScene scene,
    //    Node parent = null,
    //    Func<Node, string> GetRootType = null,
    //    Func<Node, string> GetRootName = null,
    //    Func<Node, GlbRotate> GetRotation = null,
    //    Func<Node, float> GetMassMultiplier = null,
    //    Func<Node, float> GetScaleMultiplier = null,
    //    Func<MeshInstance3D, string> GetPartName = null,
    //    Func<MeshInstance3D, GlbShapeType> GetShapeType = null,
    //    Func<MeshInstance3D, int> GetShapeCount = null,
    //    Action<MeshInstance3D, int> SetShapeCount = null,
    //    Action<Node3D> OnPartAdded = null)
    //{
    //    return ApplyPhysics(own, GetSource(), parent,
    //        GetRootType, GetRootName, GetRotation, GetMassMultiplier, GetScaleMultiplier,
    //        GetPartName, GetShapeType, GetShapeCount, SetShapeCount, OnPartAdded);

    //    Node GetSource()
    //        => scene?.InstantiateOrNull<Node>();
    //}

    //public static Node ApplyPhysics(
    //    bool own,
    //    string scene,
    //    ConfigFile cfg,
    //    Node parent = null,
    //    Action<Node3D> OnPartAdded = null)
    //{
    //    return ApplyPhysics(own, GetSource(), parent,
    //        source => cfg.GetV(scene, "RootType", Default.RootType),
    //        source => cfg.GetV(scene, "RootName", Default.NodeName(source.Name)),
    //        source => cfg.GetE(scene, "Rotation", Default.Rotation),
    //        source => cfg.GetV(scene, "MassMultiplier", Default.MassMultiplier),
    //        source => cfg.GetV(scene, "ScaleMultiplier", Default.ScaleMultiplier),
    //        part => cfg.GetV(scene, $"{part.Name}.PartName", Default.NodeName(part.Name)),
    //        part => cfg.GetE(scene, $"{part.Name}.ShapeType", Default.ShapeType),
    //        part => cfg.GetV(scene, $"{part.Name}.MultiConvexLimit", Default.MultiConvexShapeLimit),
    //        (part, value) => cfg.SetV(scene, $"{part.Name}.MultiConvexLimit", value),
    //        OnPartAdded);

    //    PackedScene GetSource()
    //        => GD.Load<PackedScene>(scene);
    //}

    //#endregion
}
