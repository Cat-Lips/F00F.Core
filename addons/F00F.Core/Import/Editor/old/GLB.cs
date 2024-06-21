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
            own: true,
            root, source,
            options.Rotate,
            options.MassMultiplier,
            options.ScaleMultiplier,
            x => NodeOptions(x).Name,
            x => NodeOptions(x).ShapeType,
            x => NodeOptions(x).MultiConvexLimit,
            (x, v) => NodeOptions(x).MultiConvexLimit = v);

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
}
