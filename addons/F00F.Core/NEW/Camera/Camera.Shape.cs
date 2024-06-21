using System.Diagnostics;
using Godot;

namespace F00F;

public partial class Camera
{
    private Shape3D MyShape { get; set; }
    private World3D MyWorld => field ??= GetWorld3D();

    private void InitShape()
    {
        var old = MyShape;
        MyShape = Config.GetShape(Camera3D);
        ShowShape(this);
        old?.Dispose();

        [Conditional("TOOLS")]
        static void ShowShape(Camera self)
        {
            self.RemoveChildren(where: GLB.IsPart);
#if TOOLS
            if (!self.Config.ShapeShow) return;
#endif
            if (self.MyShape is null) return;

            AddShapeNode();

            void AddShapeNode()
            {
                var shapeNode = NewShapeNode();
                self.AddChild(shapeNode, owner: self);
                GLB.SetAsPart(shapeNode);

                CollisionShape3D NewShapeNode() => new()
                {
                    Name = "Shape",
                    Shape = self.MyShape,
                    DebugColor = Colors.Red,
                };
            }
        }
    }
}
