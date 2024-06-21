using System.Collections.Generic;
using System.Linq;
using Godot;

namespace F00F;

[Tool, GlobalClass]
public partial class HeightGroup : Resource
{
    public static void Init(Control x, HeightGroup old, HeightGroup @new)
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
            ResetMinHeight(member);
            ResizeMembers();
        }
    }

    private void Remove(Control member)
    {
        if (MyMembers.Remove(member))
        {
            member.Resized -= ResizeMembers;
            ResetMinHeight(member);
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
            MyMembers.ForEach(ResetMinHeight);
            var w = GetMaxHeight();
            MyMembers.ForEach(x => SetMinHeight(x, w));
            wip = false;
        }
    }

    private float GetMaxHeight()
        => MyMembers.Max(x => x.GetCombinedMinimumSize().Y);

    private void SetMinHeight(Control x, float h)
        => x.CustomMinimumSize = new(x.CustomMinimumSize.X, h);

    private void ResetMinHeight(Control x)
        => SetMinHeight(x, 0);

    #endregion
}
