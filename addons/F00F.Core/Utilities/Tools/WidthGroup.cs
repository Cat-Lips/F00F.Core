using System.Collections.Generic;
using System.Linq;
using Godot;

namespace F00F;

[Tool, GlobalClass]
public partial class WidthGroup : Resource
{
    public static void Init(Control x, WidthGroup old, WidthGroup @new)
    {
        old?.Remove(x);
        @new?.Add(x);
    }

    #region Private

    private readonly HashSet<Control> MyMembers = [];

    private void Add(Control member)
    {
        if (member is null) return;
        if (MyMembers.Add(member))
        {
            member.Resized += ResizeMembers;
            ResetMinWidth(member);
            ResizeMembers();
        }
    }

    private void Remove(Control member)
    {
        if (MyMembers.Remove(member))
        {
            member.Resized -= ResizeMembers;
            ResetMinWidth(member);
            ResizeMembers();
        }
    }

    private bool wip = false;
    private AutoAction _ResizeMembers;
    private void ResizeMembers()
    {
        (_ResizeMembers ??= new(ResizeMembers)).Run();

        void ResizeMembers()
        {
            if (wip) return;
            if (MyMembers.Count is 0) return;

            wip = true;
            MyMembers.ForEach(ResetMinWidth);
            var w = GetMaxWidth();
            MyMembers.ForEach(x => SetMinWidth(x, w));
            wip = false;
        }
    }

    private float GetMaxWidth()
        => MyMembers.Max(x => x.GetCombinedMinimumSize().X);

    private void SetMinWidth(Control x, float w)
        => x.CustomMinimumSize = new(w, x.CustomMinimumSize.Y);

    private void ResetMinWidth(Control x)
        => SetMinWidth(x, 0);

    #endregion
}
