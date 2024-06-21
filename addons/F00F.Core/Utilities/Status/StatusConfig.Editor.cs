#if TOOLS
using Godot;

namespace F00F;

public partial class StatusConfig
{
    [ExportToolButton("MostCommonColor")]
    private Callable MostCommonColor => Callable.From(() =>
    {
        InfoColor = InfoIcon?.GetImage()?.MostCommonColor() ?? default;
        WarnColor = WarnIcon?.GetImage()?.MostCommonColor() ?? default;
        ErrorColor = ErrorIcon?.GetImage()?.MostCommonColor() ?? default;
        SuccessColor = SuccessIcon?.GetImage()?.MostCommonColor() ?? default;
    });

    [ExportToolButton("AverageColor")]
    private Callable AverageColor => Callable.From(() =>
    {
        InfoColor = InfoIcon?.GetImage()?.AverageColor() ?? default;
        WarnColor = WarnIcon?.GetImage()?.AverageColor() ?? default;
        ErrorColor = ErrorIcon?.GetImage()?.AverageColor() ?? default;
        SuccessColor = SuccessIcon?.GetImage()?.AverageColor() ?? default;
    });
}
#endif
