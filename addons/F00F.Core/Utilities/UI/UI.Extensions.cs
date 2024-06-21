using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using ControlPair = (Godot.Control Label, Godot.Control EditControl);

namespace F00F
{
    public static partial class UI
    {
        public static void Init(this GridContainer grid, IEnumerable<ControlPair> content)
        {
            content.ForEach(pair =>
            {
                grid.AddChild(pair.Label, forceReadableName: true);
                grid.AddChild(pair.EditControl, forceReadableName: true);
            });
        }

        public static string[] Items<T>() where T : struct, Enum
            => Enum.GetNames<T>().Select(x => x.Capitalize()).ToArray();

        public static Label ExpandToFitText(this Label source)
        {
            source.Resized += PreventShrink;
            return source;

            void PreventShrink()
            {
                var newWidth = source.Size.X;
                var curWidth = source.CustomMinimumSize.X;
                var maxWidth = Math.Max(newWidth, curWidth);
                source.CustomMinimumSize = new(maxWidth, default);
            }
        }

        public static T SetSquareLayout<T>(this T source) where T : Control
        {
            source.Resized += OnSizeChanged;
            return source;

            void OnSizeChanged()
            {
                var minSize = Math.Max(source.Size.X, source.Size.Y);
                source.CustomMinimumSize = new(minSize, minSize);
            }
        }
    }
}
