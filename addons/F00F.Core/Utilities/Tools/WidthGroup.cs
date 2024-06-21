using System.Collections.Generic;
using System.Linq;
using Godot;

namespace F00F;

[Tool, GlobalClass]
public partial class WidthGroup : Resource
{
    private readonly HashSet<Control> MyMembers = [];

    public void Add(Control member)
    {
        if (member is null) return;
        if (MyMembers.Add(member))
        {
            member.Resized += ResizeMembers;
            ResetMinWidth(member);
            ResizeMembers();
        }
    }

    public void Remove(Control member)
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
        (_ResizeMembers ??= new(ResizeMembers, 0)).Run();

        void ResizeMembers()
        {
            if (MyMembers.Count is 0) return;
            if (wip) return;

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
}
