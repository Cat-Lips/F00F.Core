using System;

namespace F00F;

public interface IActive
{
    event Action ActiveSet;

    bool Active { get; set; }
}
