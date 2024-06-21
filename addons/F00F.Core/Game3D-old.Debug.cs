using System;
using System.Linq;
using static Godot.Viewport;

namespace F00F;

public partial class Game3Dold
{
    [Flags]
    public enum DebugType
    {
        None = 0,
        DebugDraw = 1,
        DebugRend = 2,
    }

    public DebugType Debug { get; set => this.Set(ref field, value, OnDebugSet); }

    public void ClearDebug() => Debug = default;
    public void ToggleDebug(DebugType flag) => Debug ^= flag;
    public void SetDebug(params DebugType[] flags) => Debug = flags.Aggregate((a, b) => a | b);

    public void SetNextDebug()
    {
        Debug = Debug switch
        {
            DebugType.None => DebugType.DebugDraw,
            DebugType.DebugDraw => DebugType.DebugRend,
            DebugType.DebugRend => SetNextRenderType() ? DebugType.DebugRend : DebugType.None,
            _ => DebugType.None,
        };

        bool SetNextRenderType()
        {
            return (GetViewport().DebugDraw = NextRenderType()) is not DebugDrawEnum.Disabled;

            DebugDrawEnum NextRenderType() => GetViewport().DebugDraw switch
            {
                // Add as required
                DebugDrawEnum.Disabled => DebugDrawEnum.Wireframe,
                DebugDrawEnum.Wireframe => DebugDrawEnum.Overdraw,
                DebugDrawEnum.Overdraw => DebugDrawEnum.Lighting,
                DebugDrawEnum.Lighting => DebugDrawEnum.Unshaded,
                DebugDrawEnum.Unshaded => DebugDrawEnum.NormalBuffer,
                DebugDrawEnum.NormalBuffer => DebugDrawEnum.InternalBuffer,
                DebugDrawEnum.InternalBuffer => DebugDrawEnum.MotionVectors,
                DebugDrawEnum.MotionVectors => DebugDrawEnum.Occluders,
                DebugDrawEnum.Occluders => DebugDrawEnum.Disabled,
                _ => DebugDrawEnum.Disabled,
            };
        }
    }

    public void SetPrevDebug()
    {
        Debug = Debug switch
        {
            DebugType.None => DebugType.DebugRend,
            DebugType.DebugRend => DebugType.DebugDraw,
            DebugType.DebugDraw => DebugType.None,
            _ => DebugType.None,
        };
    }

    private void OnDebugSet()
    {
        EnableDebugDraw(Debug.HasFlag(DebugType.DebugDraw));
        EnableDebugRend(Debug.HasFlag(DebugType.DebugRend));

        void EnableDebugDraw(bool enable)
            => DebugDraw.Enabled = enable;

        void EnableDebugRend(bool enable)
            => GetViewport().DebugDraw = enable ? DebugDrawEnum.Wireframe : default;
    }
}
