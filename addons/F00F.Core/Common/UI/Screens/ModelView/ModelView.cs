using System;
using System.Collections.Generic;
using Godot;

namespace F00F;

using ControlPair = (Control Label, Control EditControl);

[Tool]
public partial class ModelView : Root
{
    private GridContainer Grid => field ??= (GridContainer)GetNode("%Grid");
    private SubViewport View => field ??= (SubViewport)GetNode("%View");

    public Node Scene { get; private set; }

    protected void ViewScene<T>(Node scene, T data, IEnumerable<ControlPair> controls, Action<T> SetData)
    {
        if (Scene is not null)
            ClearScene();

        View.AddChild(Scene = scene, forceReadableName: true);
        Grid.Init(controls);
        SetData(data);
    }

    protected virtual void ClearScene(out Node scene)
    {
        Grid.RemoveChildren();
        View.RemoveChild(scene = Scene);
    }

    protected void ClearScene()
    {
        ClearScene(out var scene);
        scene.QueueFree();
    }

    protected virtual void OnReady1() { }
    protected sealed override void OnReady()
    {
        OnReady1();
        PhysicsServer3D.SpaceSetActive(View.World3D.Space, false);
    }
}
