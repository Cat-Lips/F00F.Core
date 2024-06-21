using System;

namespace F00F;

public abstract class Disposable : IDisposable
{
    private bool disposed;

    private void Dispose(bool disposing)
    {
        if (!disposed)
        {
            if (disposing)
                OnDispose();

            disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    protected abstract void OnDispose();
}
