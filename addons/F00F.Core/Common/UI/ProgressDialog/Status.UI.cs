using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using StatusItem = (Godot.Control Icon, Godot.Control Text);

namespace F00F
{
    public static partial class UI
    {
        public interface IStatusBar
        {
            float Set(Status status, string msg = null, float? bytesOrPercent = null);
            void Reset();
        }

        public static Control NewStatusBar(string name, out IStatusBar status)
        {
            var control = new Utils.StatusBar(name);
            status = control;
            return control;
        }

        private static partial class Utils
        {
            public partial class StatusBar(string name) : PreviewPanel(name), IStatusBar
            {
                private const string ShowToolTip = "ShowToolTip";
                private readonly List<Func<StatusItem>> MyToolTip = [];

                private string curText = null;
                private string lastMsg = null;
                public float Set(Status status, string msg, float? bytesOrPercent)
                {
                    UpdateMsg();
                    UpdateText();
                    UpdateToolTip();
                    return GetProgress();

                    void UpdateMsg()
                        => msg ??= (status is Status.Info ? lastMsg : $"{status}");

                    void UpdateText()
                    {
                        this.OverrideUneditableFontColor(status.Color());
                        if (!IsSubMsg()) curText = msg;
                        Text = GetText();

                        string GetText()
                        {
                            return
                                IsBytes() ? $"{curText} [{bytesOrPercent:0}]" :
                                IsPercent() ? $"[{bytesOrPercent:P0}] {curText}" :
                                curText;
                        }
                    }

                    void UpdateToolTip()
                    {
                        if (msg == lastMsg) return;

                        lastMsg = msg;
                        MyToolTip.Add(() => (NewStatusIcon(), NewStatusText()));
                        TooltipText = ShowToolTip;

                        Control NewStatusIcon() => NewTextureRect("Icon", status.Icon()).SetSquareLayout();
                        Control NewStatusText() => NewSunkLabel("Text", msg);
                    }

                    bool IsSubMsg() => msg.StartsWith(' ');
                    bool IsBytes() => bytesOrPercent is > 1;
                    bool IsPercent() => bytesOrPercent is >= 0 and <= 1;
                    float GetProgress() => IsPercent()
                        ? bytesOrPercent.Value
                        : status is Status.Info ? 0 : 1;
                }

                public void Reset()
                {
                    Clear();
                    MyToolTip.Clear();
                    TooltipText = null;
                }

                protected override Control GetPreview(string tooltip)
                {
                    return tooltip is ShowToolTip ? CreatePreview() : null;

                    Control CreatePreview()
                    {
                        // FIXME:  May need to scroll if many items
                        var tt = F00F.Utils.GetScene<DataView>();
                        tt.Grid.Init(MyToolTip.Select(x => x()));
                        return tt;
                    }
                }
            }
        }
    }
}
