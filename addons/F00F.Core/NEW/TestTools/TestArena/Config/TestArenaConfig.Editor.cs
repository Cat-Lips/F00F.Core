#if TOOLS
using Godot.Collections;

namespace F00F;

using static TestArenaConfig.Enum;

public partial class TestArenaConfig
{
    public sealed override void _ValidateProperty(Dictionary property)
    {
        if (Editor.Show(property, PropertyName.StuntsScale, @if: ShowStunts)) return;

        if (Editor.Show(property, PropertyName.ShowRibsOrBeams, @if: ShowStunts)) return;
        if (Editor.Show(property, PropertyName.BeamGap, @if: ShowRibsOrBeams is RibsOrBeams.Beams)) return;
        if (Editor.Show(property, PropertyName.BeamWidth, @if: ShowRibsOrBeams is RibsOrBeams.Beams)) return;
        if (Editor.Show(property, PropertyName.BeamHeight, @if: ShowRibsOrBeams is RibsOrBeams.Beams)) return;

        if (Editor.Show(property, PropertyName.ShowRampOrSteps, @if: ShowStunts)) return;
        if (Editor.Show(property, PropertyName.StepCount, @if: ShowRampOrSteps is RampOrSteps.Steps)) return;
        if (Editor.Show(property, PropertyName.StepDepth, @if: ShowRampOrSteps is RampOrSteps.Steps)) return;
        if (Editor.Show(property, PropertyName.StepHeight, @if: ShowRampOrSteps is RampOrSteps.Steps)) return;
    }
}
#endif
