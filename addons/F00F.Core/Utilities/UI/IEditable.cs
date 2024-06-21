using System;
using System.Collections.Generic;
using Godot;

namespace F00F;

using ControlPair = (Control Label, Control EditControl);

public interface IEditable
{
    Resource Res => (Resource)this;

    IEnumerable<ControlPair> GetEditControls() => GetEditControls(out var _);
    IEnumerable<ControlPair> GetEditControls(out Action<Resource> SetData);
}

public interface IEditable<T> : IEditable where T : Resource, IEditable<T>
{
    new T Res => (T)this;

    IEnumerable<ControlPair> GetEditControls(out Action<T> SetData);
    IEnumerable<ControlPair> IEditable.GetEditControls(out Action<Resource> SetData)
    {
        var controls = GetEditControls(out var SetT);
        SetData = x => SetT((T)x);
        return controls;
    }
}
