using System;
using System.Collections.Generic;
using Godot;
using ControlPair = (Godot.Control Label, Godot.Control EditControl);

namespace F00F
{
    public interface IEditable<T> where T : Resource, IEditable<T>
    {
        T Res => (T)this;

        IEnumerable<ControlPair> GetEditControls() => GetEditControls(out var _);
        IEnumerable<ControlPair> GetEditControls(out Action<T> SetData);
    }
}
