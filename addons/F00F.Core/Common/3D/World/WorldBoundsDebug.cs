using System.Diagnostics;
using Godot;
using static F00F.DebugDraw;

namespace F00F;

public static class WorldBoundsDebug
{
    [Conditional("TOOLS")]
    public static void DrawDebug(this WorldBounds self, Color? color = null, float size = 1, Scope scope = Scope.Forever)
        => DrawDebug(self, self, 0, color, size, scope);

    [Conditional("TOOLS")]
    public static void PrintDebug(this WorldBounds self)
        => PrintDebug(self, nameof(WorldBounds), self, 0);

    [Conditional("TOOLS")]
    public static void DrawDebug(this WorldBounds self, IBounds bounds, float y, Color? color = null, float size = 1, Scope scope = Scope.Forever)
    {
        var left = new Vector3(bounds.Left, y, 0);
        var back = new Vector3(0, y, bounds.Back);
        var right = new Vector3(bounds.Right, y, 0);
        var front = new Vector3(0, y, bounds.Front);

        self.DebugPoint(left, color, size, scope);
        self.DebugPoint(back, color, size, scope);
        self.DebugPoint(right, color, size, scope);
        self.DebugPoint(front, color, size, scope);

        var leftBack = new Vector3(bounds.Left, y, bounds.Back);
        var leftFront = new Vector3(bounds.Left, y, bounds.Front);
        var rightBack = new Vector3(bounds.Right, y, bounds.Back);
        var rightFront = new Vector3(bounds.Right, y, bounds.Front);

        self.DebugPoint(leftBack, color, size, scope);
        self.DebugPoint(leftFront, color, size, scope);
        self.DebugPoint(rightBack, color, size, scope);
        self.DebugPoint(rightFront, color, size, scope);
    }

    [Conditional("TOOLS")]
    public static void PrintDebug(this WorldBounds self, string label, IBounds bounds, float y)
    {
        var left = new Vector3(bounds.Left, y, 0);
        var back = new Vector3(0, y, bounds.Back);
        var right = new Vector3(bounds.Right, y, 0);
        var front = new Vector3(0, y, bounds.Front);

        GD.Print(label);
        GD.Print($" - Left:  {left}");
        GD.Print($" - Right: {right}");
        GD.Print($" - Front: {front}");
        GD.Print($" - Back:  {back}");
    }
}
