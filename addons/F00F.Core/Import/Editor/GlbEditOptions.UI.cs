using System;
using System.Collections.Generic;
using ControlPair = (Godot.Control Label, Godot.Control EditControl);

namespace F00F
{
    public partial class GlbOptions : IEditable<GlbOptions>
    {
        public IEnumerable<ControlPair> GetEditControls() => GetEditControls(out var _);
        public IEnumerable<ControlPair> GetEditControls(out Action<GlbOptions> SetData)
        {
            var ec = EditControls(out SetData);
            SetData(this);
            return ec;
        }

        public static IEnumerable<ControlPair> EditControls(out Action<GlbOptions> SetData)
        {
            var childControls = () => (GlbNode.EditControls(out var SetChildData), SetChildData);
            return UI.Create(out SetData, CreateUI);

            void CreateUI(UI.IBuilder ui)
            {
                ui.AddText(nameof(Name));
                ui.AddScene(nameof(Scene));
                ui.AddOption(nameof(BodyType), items: UI.Items<GlbBodyType>());
                ui.AddOption(nameof(FrontFace), items: UI.Items<GlbFrontFace>());
                ui.AddValue(nameof(MassMultiplier), range: (0, null, null));
                ui.AddValue(nameof(ScaleMultiplier), range: (0, null, null));
                ui.AddArray(nameof(Nodes), GetItemControls: childControls, editable: false);
                ui.AddCheck(nameof(ImportOriginal));
            }
        }

        public partial class GlbNode : IEditable<GlbNode>
        {
            public IEnumerable<ControlPair> GetEditControls() => GetEditControls(out var _);
            public IEnumerable<ControlPair> GetEditControls(out Action<GlbNode> SetData)
            {
                var ec = EditControls(out SetData);
                SetData(this);
                return ec;
            }

            public static IEnumerable<ControlPair> EditControls(out Action<GlbNode> SetData)
            {
                return UI.Create(out SetData, CreateUI);

                static void CreateUI(UI.IBuilder ui)
                {
                    ui.AddText(nameof(Name));
                    ui.AddOption(nameof(ShapeType), items: UI.Items<GlbShapeType>());
                    ui.AddValue(nameof(MultiConvexLimit), @int: true, range: (0, null, null));
                }
            }
        }
    }
}
