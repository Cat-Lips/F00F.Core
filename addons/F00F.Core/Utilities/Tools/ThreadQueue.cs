using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Godot;

namespace F00F;

[Tool]
public partial class ThreadQueue : Node
{
    #region Private

    private readonly ConcurrentQueue<Action> Q = [];

    #endregion

    public int MaxOp { get; init; } = OS.GetProcessorCount();

    public void Add(Action action)
        => Q.Enqueue(action);

    #region Godot

    private int opCount = 0;
    public sealed override void _Process(double _)
    {
        while (MaxOp < 0 || opCount < MaxOp)
        {
            if (Q.TryDequeue(out var action))
            {
                ++opCount;
                Task.Run(() =>
                {
                    try { action?.Invoke(); }
                    finally { this.CallDeferred(() => --opCount); }
                });
            }
            else break;
        }
    }

    #endregion
}
