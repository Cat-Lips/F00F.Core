using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Godot;

namespace F00F;

internal partial class BatchAction : Node
{
    private readonly Queue<Action> q = [];

    public void Add(Action action)
        => q.Enqueue(action);

    public sealed override void _Process(double delta)
    {
        if (q.TryDequeue(out var action))
            action?.Invoke();
        if (q.Empty())
            this.Detach();
    }
}

public static class BatchActionExtensions
{
    private static Dictionary<string, BatchAction> BatchList { get; } = [];

    public static void Batch(this Node node, Action action, [CallerArgumentExpression(nameof(node))] string TagA = "", [CallerArgumentExpression(nameof(action))] string TagB = "")
    {
        var tag = string.Join(".", new[] { TagA, TagB }
            .Where(x => !string.IsNullOrWhiteSpace(x)));

        if (!BatchList.TryGetValue(tag, out var batch))
        {
            BatchList.Add(tag, batch = new BatchAction());
            batch.TreeExiting += () => BatchList.Remove(tag);
            node.GetTree().Root.AddChild(batch);
            batch.SetProcess(true);
        }

        batch.Add(action);
    }
}
