//using System;
//using Godot;

//namespace F00F;

//public partial class MyInput
//{
//    public event Action ScreenSizeChanged;
//    public Vector2I ScreenSize { get; private set => this.Set(ref field, value, ScreenSizeChanged); }

//    private const float _DefaultScale = 1f;
//    private const float _MinScale = .5f;
//    private const float _MaxScale = 5f;

//    public float DefaultScale => _DefaultScale;
//    public float MinScale => _MinScale;
//    public float MaxScale => _MaxScale;

//    public event Action<float> ScreenScaleChanged;
//    public float ScreenScale { get; set => this.Set(ref field, value.Clamp(MinScale, MaxScale), ScreenScaleChanged); } = _DefaultScale;

//    #region Private

//    private readonly Config cfg = new("ScreenScale");

//    private void InitScreenScale()
//    {
//        if (Editor.NotEditor)
//        {
//            SetScreenSize();
//            this.GetMainWindow().SizeChanged += SetScreenSize;

//            LoadAndSetScreenScale();
//            SetContentScaleFactor(ScreenScale);

//            ScreenSizeChanged += LoadAndSetScreenScale;
//            ScreenScaleChanged += SetContentScaleFactor;
//            ScreenScaleChanged += SaveScaleForScreenSize;
//        }

//        void SetScreenSize()
//            => ScreenSize = this.GetMainWindow().Size;

//        void LoadAndSetScreenScale()
//            => ScreenScale = cfg.Get($"{ScreenSize}", DefaultScale);

//        void SaveScaleForScreenSize(float scale)
//            => cfg.Set($"{ScreenSize}", scale);

//        void SetContentScaleFactor(float scale)
//            => this.GetMainWindow().ContentScaleFactor = scale;
//    }

//    #endregion
//}
