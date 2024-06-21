using System;
using Godot;

namespace F00F;

public static class ControlExtensions
{
    public static Control GetRoot(this Control source)
    {
        if (source.TopLevel) return source;
        var parent = source.GetParentOrNull<Control>();
        return parent is null ? source : parent.GetRoot();
    }

    #region MouseOver

    public static bool MouseOver(this Popup source)
        => new Rect2(default, source.GetSize()).HasPoint(source.GetMousePosition());

    public static bool MouseOver(this Control source)
        => new Rect2(default, source.GetSize()).HasPoint(source.GetLocalMousePosition());

    public static void InitMouseOver(this Control source, Action<bool> OnMouseOver)
    {
        source.SafeInit(OnEnterTree, OnExitTree);

        void OnEnterTree()
        {
            if (source.MouseOver()) OnMouseEntered();
            source.MouseEntered += OnMouseEntered;
            source.MouseExited += OnMouseExited;
        }

        void OnExitTree()
        {
            if (source.MouseOver()) OnMouseExited();
            source.MouseEntered -= OnMouseEntered;
            source.MouseExited -= OnMouseExited;
        }

        void OnMouseEntered() => OnMouseOver(true);
        void OnMouseExited() => OnMouseOver(false);
    }

    #endregion

    #region KidCheck

    private static bool HasKids(this Container source) => source.GetChildCount() > 0;
    public static void InitKidCheck(this Container source, Action<bool> HasKidsChanged)
    {
        bool? HasKids = null;
        source.SafeInit(OnEnterTree, OnExitTree);

        void OnEnterTree()
        {
            CheckHasKidsChanged();
            source.ChildEnteredTree += OnChildEntered;
            source.ChildExitingTree += OnChildExiting;
        }

        void OnExitTree()
        {
            source.ChildEnteredTree -= OnChildEntered;
            source.ChildExitingTree -= OnChildExiting;
        }

        void OnChildEntered(Node _) => CheckHasKidsChanged();
        void OnChildExiting(Node _) => source.CallDeferred(CheckHasKidsChanged);
        void CheckHasKidsChanged()
        {
            var _HasKids = source.HasKids();
            if (HasKids != _HasKids)
            {
                HasKids = _HasKids;
                HasKidsChanged(HasKids.Value);
            }
        }
    }

    #endregion

    #region Disable/Restore

    private static readonly StringName DeepState = "__D33PST4T3__";
    public static void DeepDisable(this Control root)
        => root.RecurseChildren<Control>(self: true).ForEach(_Disable);

    public static void DeepRestore(this Control root)
        => root.RecurseChildren<Control>(self: true).ForEach(_Restore);

    private static void _Disable(this Control source)
    {
        if (!source.HasMeta(DeepState))
        {
            switch (source)
            {
                case BaseButton ctrl: ctrl.SetMeta(DeepState, ctrl.Disabled); ctrl.Disabled = true; break;
                case LineEdit ctrl: ctrl.SetMeta(DeepState, ctrl.Editable); ctrl.Editable = false; break;
                case TextEdit ctrl: ctrl.SetMeta(DeepState, ctrl.Editable); ctrl.Editable = false; break;
                case Slider ctrl: ctrl.SetMeta(DeepState, ctrl.Editable); ctrl.Editable = false; break;
                case SpinBox ctrl: ctrl.SetMeta(DeepState, ctrl.Editable); ctrl.Editable = false; break;
            }
        }
    }

    private static void _Restore(this Control source)
    {
        if (source.HasMeta(DeepState))
        {
            switch (source)
            {
                case BaseButton ctrl: ctrl.Disabled = (bool)ctrl.GetMeta(DeepState, false); break;
                case LineEdit ctrl: ctrl.Editable = (bool)ctrl.GetMeta(DeepState, true); break;
                case TextEdit ctrl: ctrl.Editable = (bool)ctrl.GetMeta(DeepState, true); break;
                case Slider ctrl: ctrl.Editable = (bool)ctrl.GetMeta(DeepState, true); break;
                case SpinBox ctrl: ctrl.Editable = (bool)ctrl.GetMeta(DeepState, true); break;
            }

            source.RemoveMeta(DeepState);
        }
    }

    #endregion
}
