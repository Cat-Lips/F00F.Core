using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Godot;
using static Godot.Control;
using static Godot.FileDialog;
using static Godot.TextureRect;
using FileSelect = (Godot.Container Root, Godot.LineEdit FilePreview, Godot.Button FileClear, Godot.Button FileSelect, Godot.FileDialog FileDialog);
using Indent = (Godot.Container Root, Godot.Control Indent, Godot.Control Content);
using Layout = (Godot.Container Root, Godot.Control[] Content);
using MultiEdit = (Godot.Container Root, Godot.SpinBox[] EditControls);
using Range = (float? Min, float? Max, float? Step);
using ToggleEdit = (Godot.Container Root, Godot.Button Toggle);
using ToggleTitle = (Godot.Container Root, Godot.Button Toggle, Godot.Label Label);

namespace F00F;

public static partial class UI
{
    private static SceneTree Root => field ??= (SceneTree)Engine.GetMainLoop();

    #region Obsolete

    public static Separator NewSep(string name = "Sep1", bool v = false)
    {
        return v ? NewVSep() : NewHSep();

        VSeparator NewVSep() => new()
        {
            Name = name,
            SizeFlagsVertical = SizeFlags.ExpandFill,
        };

        HSeparator NewHSep() => new()
        {
            Name = name,
            SizeFlagsHorizontal = SizeFlags.ExpandFill,
        };
    }

    public static Label NewLabel(string name, string text = null, string hint = null, HorizontalAlignment alignment = HorizontalAlignment.Left) => new()
    {
        Name = $"{name}Label",
        Text = text ?? name.Capitalise(),
        TooltipText = hint,
        VerticalAlignment = VerticalAlignment.Center,
        HorizontalAlignment = alignment,
        MouseFilter = hint is null ? MouseFilterEnum.Ignore : MouseFilterEnum.Pass,
    };

    public static Label GetLabel(Node parent, string name)
        => (Label)parent.GetNode($"{name}Label");

    public static Button NewButton(string name, string text = null, string hint = null) => new()
    {
        Name = name,
        Text = text ?? name.Capitalise(),
        TooltipText = hint,
    };

    public static OptionButton NewOptionEdit<T>(Action<T> OnSelect) where T : struct, Enum => NewOptionEdit(typeof(T).Name, null, OnSelect);
    public static OptionButton NewOptionEdit<T>(string name, Action<T> OnSelect) where T : struct, Enum => NewOptionEdit(name, null, OnSelect);
    public static OptionButton NewOptionEdit<T>(string name, string hint, Action<T> OnSelect) where T : struct, Enum
    {
        var items = ItemIds<T>();
        var ec = NewOptionEdit(name, hint, items);
        ec.ItemSelected += idx => OnSelect((T)Enum.ToObject(typeof(T), items[idx].Id));
        return ec;
    }

    public static OptionButton NewOptionEdit(string name, string[] items, Action<string> OnSelect) => NewOptionEdit(name, null, items, OnSelect);
    public static OptionButton NewOptionEdit(string name, Action<string> OnSelect, params string[] items) => NewOptionEdit(name, null, OnSelect, items);
    public static OptionButton NewOptionEdit(string name, string hint, string[] items, Action<string> OnSelect) => NewOptionEdit(name, hint, OnSelect, items);
    public static OptionButton NewOptionEdit(string name, string hint, Action<string> OnSelect, params string[] items)
    {
        var ec = NewOptionEdit(name, hint, items);
        ec.ItemSelected += x => OnSelect(items[x]);
        return ec;
    }

    public static OptionButton NewOptionEdit(string name, string hint = null, params string[] items) => NewOptionEdit(name, hint, items.Select((x, i) => (x, i)).ToArray());
    public static OptionButton NewOptionEdit(string name, string hint = null, params (string Name, int Id)[] items)
    {
        var ec = NewOptionEdit();
        items.ForEach(x => ec.AddItem(x.Name.Capitalise(), x.Id));
        return ec;

        OptionButton NewOptionEdit() => new()
        {
            Name = name,
            TooltipText = hint,
            FitToLongestItem = true,
        };
    }

    public static SpinBox NewValueEdit(string name, string hint = null, bool @int = false, Range range = default)
    {
        var ec = NewValueEdit();
        ec.GetLineEdit().LoseFocusOnEnter();
        ec.GetLineEdit().ContextMenuEnabled = false;
        if (ec.AllowLesser) ec.ValueChanged += x => ec.MinValue = Math.Min(ec.MinValue, x);
        if (ec.AllowGreater) ec.ValueChanged += x => ec.MaxValue = Math.Max(ec.MaxValue, x);
        return ec;

        SpinBox NewValueEdit() => new()
        {
            Name = name,
            TooltipText = hint,
            SelectAllOnFocus = true,
            UpdateOnTextChanged = true,
            Alignment = HorizontalAlignment.Right,

            Step = Step(),
            Rounded = @int,
            MinValue = Min(),
            MaxValue = Max(),
            AllowLesser = NoMin(),
            AllowGreater = NoMax(),
            CustomArrowStep = NavStep(),
        };

        float Min() => range.Min ?? 0;
        float Max() => range.Max ?? 0;
        bool NoMin() => range.Min is null;
        bool NoMax() => range.Max is null;
        float Step() => @int ? 1 : Const.Epsilon;
        float NavStep() => range.Step ?? DefaultNav();
        float DefaultNav() => @int ? 1 : Max() - Min() <= 2 ? .1f : 1;
    }

    public static Button NewAddButton(string name, Action Add) => NewSquareButton($"{name}Add", '+', Add);
    public static Button NewRemoveButton(string name, Action Remove) => NewSquareButton($"{name}Remove", '-', Remove);
    public static Button NewUpButton(string name, Action Up) => NewSquareButton($"{name}Up", '\u2191', Up);
    public static Button NewDownButton(string name, Action Down) => NewSquareButton($"{name}Down", '\u2193', Down);
    public static Button NewOpenButton(string name, Action Open) => NewSquareButton($"{name}Open", '\u2026', Open);
    public static Button NewCloseButton(string name, Action Close) => NewSquareButton($"{name}Close", '\u00D7', Close);
    public static Button NewSquareButton(string name, char text, Action action)
    {
        var button = NewButton(name, $"{text}").SetSquareLayout();
        button.Pressed += action;
        return button;
    }

    public static MultiEdit NewMultiEdit(string name, string hint = null, bool @int = false, Range range = default, params string[] parts)
    {
        var root = Utils.NewEditLayout(name);
        var editControls = GetEditControls().ToArray();
        return (root, editControls);

        IEnumerable<SpinBox> GetEditControls()
        {
            var first = true;
            var sep = parts.Length > 2 ? "Sep1" : "Sep";
            foreach (var part in parts)
            {
                if (first) first = false;
                else root.AddChild(new VSeparator { Name = sep }, forceReadableName: true);

                var valueEdit = NewValueEdit(part, hint, @int, range);
                root.AddChild(valueEdit, forceReadableName: true);
                yield return valueEdit;
            }
        }
    }

    #endregion

    private static Layout NewCustomEdit(string name, params Control[] content)
        => (Utils.NewEditLayout(name, content), content);

    private static ToggleTitle NewToggleTitle(string name, string hint = null, bool fold = false)
    {
        var label = NewLabel(name, null, hint);
        var toggle = Utils.NewToggle(name, on: !fold);
        var root = Utils.NewEditLayout(name, toggle, label);

        return (root, toggle, label);
    }

    private static ToggleEdit NewToggleEdit(string name, string hint = null)
    {
        var toggle = NewCheckEdit(name, hint);
        var root = Utils.NewEditLayout(name, NewSep(), toggle);
        return (root, toggle);
    }

    private static Indent NewIndent(Control source, float size, int depth, out Action<float> SetIndentSize)
    {
        var indent = new Control { Name = "Indent" };
        var (root, _) = Link(source, indent);

        SetIndent(size);
        SetIndentSize = SetIndent;

        return (root, indent, source);

        void SetIndent(float size)
            => indent.CustomMinimumSize = new(size * depth, 0);

        static Layout Link(Control source, Control indent)
        {
            var root = Utils.NewEditLayout(source.Name, indent, source);

            var @internal = false;
            SetVisible(source.Visible);
            root.VisibilityChanged += () => SetVisible(root.Visible);
            source.VisibilityChanged += () => SetVisible(source.Visible);

            return (root, [indent, source]);

            void SetVisible(bool visible)
            {
                if (!@internal)
                {
                    @internal = true;
                    root.Visible = visible;
                    source.Visible = visible;
                    @internal = false;
                }
            }
        }
    }

    public static LineEdit NewTextEdit(string name, string hint = null) => new()
    {
        Name = name,
        TooltipText = hint,
    };

    public static CheckButton NewCheckEdit(string name, string hint = null) => new()
    {
        Name = name,
        TooltipText = hint,
    };

    public static ColorPickerButton NewColorEdit(string name, string hint = null, bool editAlpha = false) => new()
    {
        Name = name,
        TooltipText = hint,
        EditAlpha = editAlpha,
    };

    private static Control NewCurvePreview(string name, out Action<Curve> SetCurve)
    {
        // For now, just preview
        var curvePreview = new Utils.CurvePreview(name);
        SetCurve = curvePreview.SetCurve;
        return curvePreview;
    }

    public static FileSelect NewSceneSelect(string name, out Action<PackedScene> SetScene, out Action<Error, string> SetError, Action<PackedScene> OnSceneSet, bool nullable = false)
        => NewResSelect(name, out SetScene, out SetError, OnSceneSet, nullable, Utils.SceneFilters);

    public static FileSelect NewTextureSelect(string name, out Action<Texture2D> SetTexture, out Action<Error, string> SetError, Action<Texture2D> OnTextureSet, bool nullable = false)
        => NewResSelect(name, out SetTexture, out SetError, OnTextureSet, nullable, Utils.ImageFilters, new Utils.TexturePreview(name));

    public static FileSelect NewResSelect<T>(string name, out Action<T> SetRes, out Action<Error, string> SetError, Action<T> OnResSet, bool nullable = false, string[] filter = null) where T : Resource
        => NewResSelect(name, out SetRes, out SetError, OnResSet, nullable, filter, null);

    private static FileSelect NewResSelect<T>(string name, out Action<T> SetRes, out Action<Error, string> SetError, Action<T> OnResSet,
        bool nullable = false, string[] filter = null, Utils.PreviewPanel preview = null) where T : Resource
    {
        var fileSelect = NewFileSelect(name, out var SetFile, out SetError, OnFileSet, nullable, filter, preview);
        SetRes = x => SetFile(x?.ResourcePath);
        return fileSelect;

        void OnFileSet(string path)
            => OnResSet(path is null ? null : F00F.Utils.Load<T>(path));
    }

    public static FileSelect NewFileSelect(string name, out Action<string> SetFile, out Action<Error, string> SetError, Action<string> OnFileSet,
        bool nullable = false, string[] filter = null) => NewFileSelect(name, out SetFile, out SetError, OnFileSet, nullable, filter, null);

    private static FileSelect NewFileSelect(string name, out Action<string> SetFile, out Action<Error, string> SetError, Action<string> OnFileSet,
        bool nullable = false, string[] filter = null, Utils.PreviewPanel preview = null)
    {
        preview ??= new(name);

        var fileClear = (Button)null;
        var fileDialog = Utils.NewFileOpenDialog(name, preview, filter);
        if (nullable) fileClear = NewCloseButton(name, OnFileCleared);
        var fileSelect = NewOpenButton(name, ShowOpenFileDialog);
        var root = Utils.NewEditLayout(name, preview, fileClear, fileSelect);

        SetFile = SetSelectedFile;
        SetError = OnFileError;
        fileDialog.FileSelected += OnFileSelected;

        return (root, preview, fileClear, fileSelect, fileDialog);

        void ShowOpenFileDialog()
            => fileDialog.PopupCenteredClamped((Vector2I)preview.GetDisplayRect().Size);

        void OnFileSelected(string path)
            => SetSelectedFile(path);

        void OnFileCleared()
            => SetSelectedFile(null);

        void OnFileError(Error err, string msg)
        {
            preview.Text = $"{err}".Capitalise();
            preview.TooltipText = msg;

            if (nullable)
                fileClear.Disabled = true;
        }

        void SetSelectedFile(string path)
        {
            if (path is null)
            {
                preview.Text = null;
                preview.TooltipText = null;

                if (nullable)
                    fileClear.Disabled = true;
            }
            else
            {
                var file = path.GetFile();
                var dir = path.GetBaseDir();

                fileDialog.CurrentPath = path;
                fileDialog.CurrentFile = file;
                fileDialog.CurrentDir = dir;

                preview.Text = file;
                preview.TooltipText = path;

                if (nullable)
                    fileClear.Disabled = false;
            }

            Debug.Assert(!preview.Editable);
            preview.EmitSignal(LineEdit.SignalName.TextChanged, preview.Text);

            OnFileSet?.Invoke(path);
        }
    }

    private static partial class Utils
    {
        public static HBoxContainer NewEditLayout(string name, params Control[] kids)
        {
            var root = GetEditLayout();
            kids.ForEach(x => { if (x is not null) root.AddChild(x, forceReadableName: true); });
            return root;

            HBoxContainer GetEditLayout() => new()
            {
                Name = $"{name}Layout",
                SizeFlagsVertical = SizeFlags.ShrinkCenter,
                SizeFlagsHorizontal = SizeFlags.ExpandFill,
            };
        }

        public static Button NewToggle(string name, bool on = true, string onText = "-", string offText = "+", string onTooltip = "Hide", string offTooltip = "Show")
        {
            var toggle = GetToggle();

            SetToggleState(on);
            toggle.ButtonPressed = on;
            toggle.Toggled += SetToggleState;

            return toggle;

            void SetToggleState(bool on)
            {
                toggle.Text = on ? onText : offText;
                toggle.TooltipText = on ? onTooltip : offTooltip;
            }

            Button GetToggle() => new()
            {
                Name = $"{name}Toggle",
                Flat = true,
                ToggleMode = true,
                FocusMode = FocusModeEnum.None,
            };
        }

        private static readonly Config<FileDialog> config = new();
        public static FileDialog NewFileOpenDialog(string name, Control parent, string[] filter = null, AccessEnum access = AccessEnum.Filesystem)
        {
            var dlg = GetFileDialog();
            dlg.FileSelected += OnFileSelected;
            dlg.AboutToPopup += OnAboutToPopup;
            parent.AddChild(dlg, forceReadableName: true);
            return dlg;

            void OnFileSelected(string _)
                => config.Set(parent.Name, dlg.CurrentDir);

            void OnAboutToPopup()
            {
                if (string.IsNullOrEmpty(dlg.CurrentFile))
                    dlg.CurrentDir = config.Get(parent.Name, dlg.CurrentDir);
            }

            FileDialog GetFileDialog() => new()
            {
                Name = $"{name}Dialog",
                Access = access,
                Filters = filter,
                FileMode = FileModeEnum.OpenFile,
            };
        }

        public static readonly string[] ImageFilters = GetImageFilters().ToArray();
        private static IEnumerable<string> GetImageFilters()
        {
            yield return "*.png;PNG";
            yield return "*.jpg,*.jpeg;JPEG";
            yield return "*.bmp;BMP";
            yield return "*.svg;SVG";
            yield return "*.webp;WebP";
            yield return "*.exr;OpenEXR";
            yield return "*.hdr;Radiance HDR";
            yield return "*.ktx;Khronos Texture";
            yield return "*.tga;Truevision Targa";
            yield return "*.dds;DirectDraw Surface";
        }

        public static readonly string[] SceneFilters = GetSceneFilters().ToArray();
        private static IEnumerable<string> GetSceneFilters()
        {
            yield return "*.scn,*.tscn";
            yield return "*.glb,*.gltf";
            yield return "";
            yield return "*.fbx;FBX";
            yield return "*.dae;COLLADA";
            yield return "*.obj;Wavefront";
            yield return "*.blend;Blender";
        }

        public static LineEdit NewSunkLabel(string name, string text = null, string hint = null) => new LineEdit()
        {
            Name = name,
            Text = text ?? name.Capitalise(),
            Editable = false,
            TooltipText = hint,
            ExpandToTextLength = true,
        }.OverrideUneditableFontColor();

        public static TextureRect NewTextureRect(string name, string texture, string hint = null) => NewTextureRect(name, F00F.Utils.Load<Texture2D>(texture), hint);
        public static TextureRect NewTextureRect(string name, Texture2D texture = null, string hint = null) => new()
        {
            Name = name,
            Texture = texture,
            ExpandMode = ExpandModeEnum.FitHeightProportional,
            StretchMode = StretchModeEnum.KeepAspectCentered,
            TooltipText = hint,
        };

        public partial class PreviewPanel : LineEdit
        {
            public PreviewPanel(string name)
            {
                Name = name;
                Editable = false;
                ExpandToTextLength = true;
                SizeFlagsHorizontal = SizeFlags.ExpandFill;
            }

            public sealed override GodotObject _MakeCustomTooltip(string tooltip)
                => GetPreview(tooltip);

            protected virtual Control GetPreview(string tooltip)
                => null;
        }

        public partial class TexturePreview(string name) : PreviewPanel(name)
        {
            protected sealed override Control GetPreview(string tooltip)
            {
                var root = new VBoxContainer { Name = "ToolTip" };
                var label = NewSunkLabel("Text", tooltip);
                var texture = NewTextureRect("Texture", tooltip);

                root.AddChild(texture, forceReadableName: true);
                root.AddChild(NewSep("Sep"), forceReadableName: true);
                root.AddChild(label, forceReadableName: true);

                return root;
            }
        }

        public partial class CurvePreview(string name) : PreviewPanel(name)
        {
            private Curve curve;
            private TextureRect preview;

            public void SetCurve(Curve curve)
            {
                this.curve = curve;
                if (preview is null) return;
                preview.Texture = GetTexture();
            }

            protected sealed override Control GetPreview(string tooltip)
                => preview = curve is null ? null : NewTextureRect("Texture", GetTexture());

            private CurveTexture GetTexture()
                => curve is null ? null : new() { Curve = curve };
        }
    }
}
