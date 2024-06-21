using System;
using System.Collections.Generic;
using Godot;

namespace F00F;

using ControlPair = (Control Label, Control EditControl);

public interface IEditable<T> where T : Resource, IEditable<T>
{
    T Res => (T)this;

    IEnumerable<ControlPair> GetEditControls();// => GetEditControls(out var _);
    IEnumerable<ControlPair> GetEditControls(out Action<T> SetData);
}
