using System;
using System.Runtime.InteropServices;

namespace F00F;

public readonly ref struct PinBuffer : IDisposable
{
    private readonly GCHandle handle;

    public nint Ptr { get; }
    public uint Size { get; }

    public PinBuffer(byte[] data)
        : this(data, data.Length) { }

    public PinBuffer(byte[] data, int size)
    {
        handle = GCHandle.Alloc(data, GCHandleType.Pinned);
        Ptr = handle.AddrOfPinnedObject();
        Size = (uint)size;
    }

    public void Dispose()
        => handle.Free();
}
