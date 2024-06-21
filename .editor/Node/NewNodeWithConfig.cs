using Godot;

namespace Game;

public partial class _CLASS_ : _BASE_
{
    #region Private

    //private TYPE NAME => field ??= GetNode<TYPE>("%NAME");

    #endregion

    #region Export

    [Export] public _CLASS_Config Config { get; set => this.Set(ref field, value ?? new(), OnConfigSet); }

    #endregion

    #region Godot

    public sealed override void _Ready()
        => Config ??= new();

    public sealed override void _Process(double _delta)
    {
        //var delta = (float)_delta;
        //DoStuff(delta);
    }

    #endregion

    #region Private

    //void DoStuff(float delta) { }

    #endregion
}

//
// _CLASS_.Init.cs
//

public partial class _CLASS_
{
    public void OnConfigSet(_CLASS_Config old, _CLASS_Config @new)
    {
        this.SafeInit(old, @new, OnConfigChanged);

        void OnConfigChanged()
        {
            // Do stuff
        }
    }
}