using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace F00F;

[Tool]
public partial class KeyEdit : OptionButton
{
    public new event Action<string> ItemSelected;

    public new string Selected
    {
        get => GetKeyFromIndex(Base.Selected);
        set => Base.Selected = GetIndexOfKey(value);
    }

    public event Action KeysChanged;
    [Export] public string[] Keys { get; set => this.Set(ref field, value, KeysChanged); }

    public void EnableItem(string key) => Base.EnableItem(GetIndexOfKey(key));
    public void DisableItem(string key) => Base.DisableItem(GetIndexOfKey(key));
    public bool IsItemEnabled(string key) => Base.IsItemEnabled(GetIndexOfKey(key));
    public bool IsItemDisabled(string key) => Base.IsItemDisabled(GetIndexOfKey(key));
    public IEnumerable<string> GetEnabledItems() => Base.GetEnabledItems().Select(GetKeyFromIndex);
    public IEnumerable<string> GetDisabledItems() => Base.GetDisabledItems().Select(GetKeyFromIndex);

    #region Private

    public KeyEdit()
    {
        Name = GetType().Name;
        KeysChanged += OnKeysChanged;
        base.ItemSelected += OnItemSelected;

        void OnKeysChanged()
            => this.SetItems(Keys);

        void OnItemSelected(long idx)
            => ItemSelected?.Invoke(GetKeyFromIndex((int)idx));
    }

    private OptionButton Base => this;
    private string GetKeyFromIndex(int idx) => idx is -1 ? null : this.GetItem(idx);
    private int GetIndexOfKey(string key) => key is null ? -1 : this.IndexOf(key);

    #endregion
}
