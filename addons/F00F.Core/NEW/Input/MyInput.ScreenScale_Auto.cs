using System;
using Godot;

namespace F00F;

public partial class MyInput
{
    public event Action ScreenSizeChanged;
    public event Action WindowSizeChanged;
    public event Action ControlScaleChanged;

    public Vector2I ScreenSize { get; private set => this.Set(ref field, value, ScreenSizeChanged); }
    public Vector2I WindowSize { get; private set => this.Set(ref field, value, WindowSizeChanged); }
    public float ControlScale { get; private set => this.Set(ref field, value, ControlScaleChanged); }

    public Window MainWindow => field ??= this.GetMainWindow();

    #region Private

    private void InitScreenScale()
    {
        if (Editor.NotEditor)
        {
            InitScreenSize();
            InitWindowSize();
            InitControlScale();
            ApplyControlScale();

            MainWindow.DpiChanged += InitScreenSize;
            MainWindow.SizeChanged += InitWindowSize;
            ScreenSizeChanged += InitControlScale;
            WindowSizeChanged += InitControlScale;
            ControlScaleChanged += ApplyControlScale;
        }

        void InitWindowSize() => WindowSize = MainWindow.Size;
        void InitScreenSize() => ScreenSize = DisplayServer.ScreenGetSize(MainWindow.CurrentScreen);
        void InitControlScale() => ControlScale = Mathf.Max(ScreenSize.X / (float)WindowSize.X, ScreenSize.Y / (float)WindowSize.Y);
        void ApplyControlScale() => MainWindow.ContentScaleFactor = ControlScale;
    }

    #endregion
}
