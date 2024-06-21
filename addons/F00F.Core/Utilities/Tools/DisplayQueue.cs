using System;
using System.Collections.Concurrent;
using Godot;

namespace F00F;

[Tool]
public partial class DisplayQueue : Node
{
    #region Private

    private readonly ConcurrentQueue<Action> Q = [];

    #endregion

    public int MaxOp { get; init; } = -1;

    public void Add(Action action)
        => Q.Enqueue(action);

    #region Godot

    public sealed override void _Process(double _)
    {
        var opCount = -1;
        while (MaxOp < 0 || ++opCount < MaxOp)
        {
            if (Q.TryDequeue(out var action))
                action?.Invoke();
            else break;
        }
    }

    #endregion
}
