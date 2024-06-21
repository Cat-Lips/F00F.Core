using System;
using Godot;

namespace F00F;

[Tool]
public partial class PopupView : Container
{
    #region Private

    private Label _Title => field ??= (Label)GetNode("%Title");
    private Separator _TitleSep => field ??= (Separator)_Title.GetSibling("TitleSep");

    private Container _Buttons => field ??= (Container)_Title.GetSibling("Buttons");
    private Separator _ButtonsSep => field ??= (Separator)_Title.GetSibling("ButtonsSep");

    private Button _Cancel => field ??= (Button)GetNode("%Cancel");
    private Button _Proceed => field ??= (Button)GetNode("%Proceed");

    protected Config cfg => field ??= new Config(GetType().Name, false);
    protected MyPopup root => field ??= this.FindParent<MyPopup>();

    #endregion

    public event Action Cancel;
    public event Action Proceed;

    [Export] public bool ShowTitle { get; set => this.Set(ref field, value, OnShowTitleSet); } = true;
    [Export] public bool ShowButtons { get; set => this.Set(ref field, value, OnShowButtonsSet); } = true;

    #region Godot

    protected virtual void OnReady() { }
    public sealed override void _Ready()
    {
        InitButtons();
        LoadState();
        OnReady();

        void InitButtons()
        {
            _Cancel.Pressed += OnCancelPressed;
            _Proceed.Pressed += OnProceedPressed;

            void OnCancelPressed()
            {
                root?.Hide();
                Cancel?.Invoke();
            }

            void OnProceedPressed()
            {
                root?.Hide();
                SaveState();
                cfg.Save();

                Proceed?.Invoke();
            }
        }
    }

    #endregion

    #region Private

    protected virtual void LoadState() { }
    protected virtual void SaveState() { }

    private void OnShowTitleSet()
    {
        _Title.Visible = ShowTitle;
        _TitleSep.Visible = ShowTitle;
    }

    private void OnShowButtonsSet()
    {
        _Buttons.Visible = ShowButtons;
        _ButtonsSep.Visible = ShowButtons;
    }

    #endregion
}
