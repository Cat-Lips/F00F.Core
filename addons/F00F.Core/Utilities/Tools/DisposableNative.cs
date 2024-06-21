using System;

namespace F00F;

public abstract class DisposableNative : IDisposable
{
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    ~DisposableNative()
        => Dispose(false);

    private bool disposed;
    private void Dispose(bool disposing)
    {
        if (!disposed)
        {
            if (disposing)
                OnDispose();
            OnDisposeNative();
            disposed = true;
        }
    }

    protected virtual void OnDispose() { }
    protected abstract void OnDisposeNative();
}
