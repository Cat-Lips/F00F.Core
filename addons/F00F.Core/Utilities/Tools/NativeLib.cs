using System;
using System.IO;
using System.Runtime.InteropServices;

namespace F00F;

public class NativeLib : Disposable
{
    private readonly nint handle;

    private NativeLib(nint handle)
        => this.handle = handle;

    public static NativeLib Load(string dll, out string error)
    {
        if (dll.IsNullOrEmpty())
        {
            error = "Platform not supported";
            //Log.Info(error);
            return null;
        }

        var path = Path.Join(AppContext.BaseDirectory, dll);

        Log.Debug($"Loading {path}");
        if (TryLoad(path, out var handle, out error))
            return new NativeLib(handle);

        //Log.Error(error);
        return null;

        static bool TryLoad(string path, out nint handle, out string error)
        {
            try { handle = NativeLibrary.Load(path); error = null; return true; }
            catch (Exception e) { error = e.Message; handle = 0; return false; }
        }
    }

    protected override void OnDispose()
        => NativeLibrary.Free(handle);
}
