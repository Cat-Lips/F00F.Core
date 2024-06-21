using System;

namespace F00F;

public interface IStatus
{
    public event Action<Status, string> Status;
}
