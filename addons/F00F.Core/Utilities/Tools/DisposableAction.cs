using System;

namespace F00F;

public class DisposableAction(Action action) : Disposable
{
    protected sealed override void OnDispose()
        => action?.Invoke();
}
