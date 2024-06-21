//#if TOOLS
//using Godot;
//using Godot.Collections;

//namespace F00F;

//public partial class TestTerrain
//{
//    public sealed override void _ValidateProperty(Dictionary property)
//    {
//        if (Editor.SetDisplayOnly(property, PropertyName.Size)) return;
//        if (Editor.SetDisplayOnly(property, PropertyName.Amplitude)) return;
//        if (Editor.SetDisplayOnly(property, PropertyName.ShapeType)) return;
//        if (Editor.SetDisplayOnly(property, PropertyName.Color)) return;
//        if (Editor.SetDisplayOnly(property, PropertyName.Noise)) return;
//    }

//    protected virtual void OnEditorSave() { }
//    public sealed override void _Notification(int what)
//    {
//        if (Editor.OnPreSave(what))
//        {
//            if (this.IsEditedSceneRoot())
//            {
//                Editor.DoPreSaveReset(Mesh, MeshInstance3D.PropertyName.Mesh);
//                Editor.DoPreSaveReset(Shape, CollisionShape3D.PropertyName.Shape);
//                Editor.DoPreSaveReset(Bounds, WorldsEnd.PropertyName.Size, 100);
//            }

//            OnEditorSave();
//            return;
//        }

//        if (Editor.OnPostSave(what))
//            Editor.DoPostSaveRestore();
//    }
//}
//#endif
