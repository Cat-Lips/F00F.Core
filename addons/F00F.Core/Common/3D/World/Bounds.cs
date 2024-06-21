namespace F00F;

public interface IBounds
{
    float Top => Front;
    float Left { get; }
    float Back { get; }
    float Right { get; }
    float Front { get; }
    float Bottom => Back;

    IBounds Grow(float by)
        => new Bounds(Left - by, Back + by, Right + by, Front - by);
}

public readonly record struct Bounds(float Left, float Back, float Right, float Front) : IBounds;
