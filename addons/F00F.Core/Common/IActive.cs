using System;

namespace F00F;

public interface IActive
{
    event Action ActiveChanged;
    bool Active { get; set; }
}
