using System;
using System.Runtime.CompilerServices;
using Godot;

namespace F00F;

public static partial class UI
{
    public static Button Button(Action OnClick, string text = null, string hint = null, [CallerArgumentExpression(nameof(OnClick))] string name = null) => Button(name, OnClick, text, hint);
    public static Button Button(string name, Action OnClick, string text = null, string hint = null)
    {
        var ec = NewButton();
        ec.Pressed += OnClick;
        return ec;

        Button NewButton() => new()
        {
            Name = name,
            Text = text ?? name.Capitalise(),
            TooltipText = hint,
        };
    }

    public static CheckButton Toggle(bool on, Action<bool> OnToggle, string text = null, string hint = null, [CallerArgumentExpression(nameof(on))] string name = null) => Toggle(name, on, OnToggle, text, hint);
    public static CheckButton Toggle(string name, bool on, Action<bool> OnToggle, string text = null, string hint = null)
    {
        var ec = NewToggle();
        ec.Toggled += on => OnToggle(on);
        return ec;

        CheckButton NewToggle() => new()
        {
            Name = name,
            Text = text,
            TooltipText = hint,
            ButtonPressed = on,
        };
    }

    public static Button AddButton(string name, Action OnClick) => Button($"{name}.Add", OnClick, '\u002B'); // +
    public static Button RemButton(string name, Action OnClick) => Button($"{name}.Rem", OnClick, '\u002D'); // -

    public static Button UpButton(string name, Action OnClick) => Button($"{name}.Up", OnClick, '\u2191'); // ↑
    public static Button DownButton(string name, Action OnClick) => Button($"{name}.Down", OnClick, '\u2193'); // ↓
    public static Button LeftButton(string name, Action OnClick) => Button($"{name}.Left", OnClick, '\u2190'); // ←
    public static Button RightButton(string name, Action OnClick) => Button($"{name}.Right", OnClick, '\u2192'); // →

    public static Button OpenButton(string name, Action OnClick) => Button($"{name}.Open", OnClick, '\u2026'); // …
    public static Button CloseButton(string name, Action OnClick) => Button($"{name}.Close", OnClick, '\u00D7'); // ×

    public static Button AddButton(Action OnClick, [CallerArgumentExpression(nameof(OnClick))] string name = null) => AddButton(name, OnClick);
    public static Button RemButton(Action OnClick, [CallerArgumentExpression(nameof(OnClick))] string name = null) => RemButton(name, OnClick);

    public static Button UpButton(Action OnClick, [CallerArgumentExpression(nameof(OnClick))] string name = null) => UpButton(name, OnClick);
    public static Button DownButton(Action OnClick, [CallerArgumentExpression(nameof(OnClick))] string name = null) => DownButton(name, OnClick);
    public static Button LeftButton(Action OnClick, [CallerArgumentExpression(nameof(OnClick))] string name = null) => LeftButton(name, OnClick);
    public static Button RightButton(Action OnClick, [CallerArgumentExpression(nameof(OnClick))] string name = null) => RightButton(name, OnClick);

    public static Button OpenButton(Action OnClick, [CallerArgumentExpression(nameof(OnClick))] string name = null) => OpenButton(name, OnClick);
    public static Button CloseButton(Action OnClick, [CallerArgumentExpression(nameof(OnClick))] string name = null) => CloseButton(name, OnClick);

    public static Button Button(Action OnClick, char text, string hint = null, [CallerArgumentExpression(nameof(OnClick))] string name = null) => Button(name, OnClick, text, hint);
    public static Button Button(string name, Action OnClick, char text, string hint = null) => Button(name, OnClick, $"{text}", hint).SetSquareLayout();
}
