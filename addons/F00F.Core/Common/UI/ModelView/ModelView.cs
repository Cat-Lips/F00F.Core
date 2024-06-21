using System;
using System.Collections.Generic;
using Godot;
using ControlPair = (Godot.Control Label, Godot.Control EditControl);

namespace F00F
{
    [Tool]
    public partial class ModelView : Root
    {
        private GridContainer Grid => GetNode<GridContainer>("%Grid");
        private SubViewport View => GetNode<SubViewport>("%View");
        private Camera3D Camera => GetNode<Camera3D>("%Camera");

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
            Grid.RemoveChildren(free: true);
            View.RemoveChild(scene = Scene);
        }

        protected void ClearScene()
        {
            ClearScene(out var scene);
            scene.QueueFree();
        }

        public override void _Ready()
            => PhysicsServer3D.SpaceSetActive(View.World3D.Space, false);
    }
}
