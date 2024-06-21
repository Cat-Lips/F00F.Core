using System.Collections.Generic;
using System.Linq;
using Godot;

namespace F00F;

// Currently unused and out of date
public partial class ProgressDialog : RootDialog
{
    private ProgressRoot Root => field ??= this.GetChildren<ProgressRoot>().Single();

    public static ProgressRoot Show(Node parent, string name)
    {
        var window = Utils.New<ProgressDialog>();
        window.Name = name;
        window.Title = name.Capitalise();
        parent.OnReady(Show);
        return window.Root;

        void Show()
            => window.PopupExclusiveCenteredClamped(parent);
    }

    protected sealed override void OnReady()
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
                    var btnCancel = (Button)Root.GetNode("%Cancel");
                    btnCancel.EmitSignal(BaseButton.SignalName.Pressed);

                    if (Root.IsCancelled) CloseWindow();
                    else Root.IsCancelledChanged += CloseWindow;
                }
            }
        }
    }
}
