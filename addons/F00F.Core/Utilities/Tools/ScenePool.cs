using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Godot;

namespace F00F;

public partial class ScenePool : Node
{
    #region Export

    [Export] public PackedScene Source { get; set; }

    #endregion

    private Pool _pool;
    protected void Initialise(Pool pool)
        => _pool = pool;

    #region Godot

    private const int PreDelete = (int)NotificationPredelete;
    public sealed override void _Notification(int what)
    {
        if (what is PreDelete)
        {
            Debug.Assert(_pool is not null, "Please Initialise(pool) before use");
            _pool?.Dispose();
        }
    }

    #endregion

    #region Pool

    protected abstract class Pool : Disposable;
    protected class Pool<NodeId, NodeType> : Pool where NodeType : Node
    {
        private readonly LinkedList<NodeId> queue = [];
        private readonly Dictionary<NodeId, NodeType> active = [];
        private readonly Dictionary<NodeId, (NodeType Node, LinkedListNode<NodeId> QueueId)> available = [];

        public NodeType GetOrCreate(in NodeId id, PackedScene source)
        {
            if (active.TryGetValue(id, out var _active))
            {
                Debug.Assert(!queue.Contains(id));
                Debug.Assert(!available.ContainsKey(id));

                return _active;
            }

            if (available.Remove(id, out var _available))
            {
                Debug.Assert(queue.Contains(id));
                Debug.Assert(!active.ContainsKey(id));

                queue.Remove(_available.QueueId);
                active.Add(id, _available.Node);

                return _available.Node;
            }

            Debug.Assert(!queue.Contains(id));
            var _new = source.New<NodeType>();
            active.Add(id, _new);
            return _new;
        }

        public void ReturnToPool(in NodeId id)
        {
            Debug.Assert(active.ContainsKey(id));

            if (!active.Remove(id, out var node))
                throw new InvalidOperationException($"Attempting to return unknown active item to pool [Pool: {typeof(NodeType).Name}, Id: {id}]");

            Debug.Assert(!queue.Contains(id));
            Debug.Assert(!available.ContainsKey(id));

            var queueId = queue.AddLast(id);
            available.Add(id, (node, queueId));
        }

        public void FreeInactiveItems()
        {
            Debug.Assert(available.Keys.ToHashSet().SetEquals(queue));

            available.Values.ForEach(x => x.Node.Free());
            available.Clear();

            queue.Clear();
        }

        public IReadOnlyDictionary<NodeId, NodeType> GetActiveItems()
            => active.AsReadOnly();

        protected sealed override void OnDispose()
        {
            FreeInactiveItems();
            active.Clear();
        }
    }

    #endregion
}
