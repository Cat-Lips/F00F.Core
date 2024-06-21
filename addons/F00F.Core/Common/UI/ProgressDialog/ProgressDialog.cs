using System.Collections.Generic;
using System.Linq;
using Godot;

namespace F00F
{
    public partial class ProgressDialog : RootDialog
    {
        private ProgressRoot _Root;
        private ProgressRoot Root => _Root ??= this.GetChildren<ProgressRoot>().Single();

        public static ProgressRoot Show(Node parent, string name)
        {
            var window = Utils.GetScene<ProgressDialog>();
            window.Name = name;
            window.Title = name.Capitalize();
            parent.OnReady(Show);
            return window.Root;

            void Show()
                => window.PopupExclusiveCenteredClamped(parent);
        }

        protected override void OnReady()
        {
            InitTitle();
            InitWindow();

            void InitTitle()
            {
                var title = Title;
                Root.ProgressChanged += SetTitle;
                Root.IsCompleteChanged += SetTitle;
                Root.IsCancelledChanged += SetTitle;

                void SetTitle()
                {
                    Title = string.Join(" ", Parts());

                    IEnumerable<string> Parts()
                    {
                        yield return title;
                        if (Root.Progress is > 0 and < 1) yield return Progress();
                        if (Root.IsCancelled) yield return Cancelled();
                        else if (Root.IsComplete) yield return Complete();

                        string Progress() => $"({Root.Progress:P0})";
                        string Cancelled() => $"[Cancelled{WithErrors()}]";
                        string Complete() => $"[Complete{WithErrors()}]";
                        string WithErrors() => Root.HasErrors ? $" With Errors" : null;
                    }
                }
            }

            void InitWindow()
            {
                Root.CloseClicked += CloseWindow;
                CloseRequested += CloseOrCancel;

                void CloseWindow()
                    => GetParent().RemoveChild(this, free: true);

                void CloseOrCancel()
                {
                    if (Root.IsComplete || Root.IsCancelled)
                        CloseWindow();
                    else
                        Utils.ShowConfirm(Root, "Warning!", "Cancel all in progress?", Cancel);

                    void Cancel()
                    {
                        var btnCancel = Root.GetNode<Button>("%Cancel");
                        btnCancel.EmitSignal(Button.SignalName.Pressed);

                        if (Root.IsCancelled) CloseWindow();
                        else Root.IsCancelledChanged += CloseWindow;
                    }
                }
            }
        }
    }
}
