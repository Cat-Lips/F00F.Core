#if STEAM
using System;

namespace F00F;

public static class UnsafeExtensions_Steam
{
    public static unsafe ReadOnlySpan<byte> Span(this nint ptr, int size)
        => new(ptr.ToPointer(), size);
}
#endif
