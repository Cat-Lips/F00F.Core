using System;

namespace F00F;

public class DisposableAction(Action action) : Disposable
{
    protected override void OnDispose()
        => action?.Invoke();
}
