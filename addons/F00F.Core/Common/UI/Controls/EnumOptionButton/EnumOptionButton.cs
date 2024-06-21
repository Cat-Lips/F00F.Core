using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace F00F;

public partial class EnumOptionButton<T> : OptionButton where T : struct, Enum
{
    public new event Action<T> ItemSelected;

    public new T Selected
    {
        get => GetEnumFromIndex(base.Selected);
        set => Base.SelectFirst(GetIndexOfEnum(value));
    }

    public void EnableItem(T value) => Base.EnableItem(GetIndexOfEnum(value));
    public void DisableItem(T value) => Base.DisableItem(GetIndexOfEnum(value));
    public bool IsItemEnabled(T value) => Base.IsItemEnabled(GetIndexOfEnum(value));
    public bool IsItemDisabled(T value) => Base.IsItemDisabled(GetIndexOfEnum(value));
    public IEnumerable<T> GetEnabledItems() => Base.GetEnabledItems().Select(GetEnumFromIndex);
    public IEnumerable<T> GetDisabledItems() => Base.GetDisabledItems().Select(GetEnumFromIndex);

    #region Private

    public EnumOptionButton()
    {
        Name = GetType().Name;

        this.AddItems<T>();
        base.ItemSelected += OnItemSelected;

        void OnItemSelected(long idx)
            => ItemSelected?.Invoke(GetEnumFromIndex((int)idx));
    }

    private OptionButton Base => this;
    private T GetEnumFromIndex(int idx) => GetItemId(idx).AsEnum<T>();
    private int GetIndexOfEnum(T @enum) => GetItemIndex(@enum.AsInt());

    #endregion
}
