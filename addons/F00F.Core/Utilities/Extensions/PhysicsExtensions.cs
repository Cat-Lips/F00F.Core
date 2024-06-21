using System.Collections.Generic;
using System.Linq;
using Godot;
using Godot.Collections;

namespace F00F;

public static class PhysicsExtensions
{
    public static Array<Rid> Rids(this IEnumerable<PhysicsBody3D> source)
    {
        return source.IsNullOrEmpty() ? null : [with(Rids())];

        IEnumerable<Rid> Rids()
            => source.Select(x => x.GetRid());
    }

    public static Array<Rid> Rids(this IEnumerable<Node3D> source)
    {
        return source.IsNullOrEmpty() ? null : [with(Rids())];

        IEnumerable<Rid> Rids()
            => source.AsBody().Select(x => x.GetRid());
    }

    private static IEnumerable<PhysicsBody3D> AsBody(this IEnumerable<Node3D> source)
        => source.Select(x => x as PhysicsBody3D).Where(x => x.NotNull());
}
