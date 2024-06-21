using System;

namespace F00F;

public abstract class Disposable : IDisposable
{
    private bool disposed;
    public void Dispose()
    {
        if (!disposed)
        {
            OnDispose();
            disposed = true;
        }
    }

    protected abstract void OnDispose();
}
