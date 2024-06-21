using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Godot;
using ControlPair = (Godot.Control Label, Godot.Control EditControl);
using Range = (float? Min, float? Max, float? Step);

namespace F00F
{
    public static partial class UI
    {
        public interface IBuilder
        {
            void AddSep();

            void AddGroup(string title, string prefix = null, bool fold = false);
            void EndGroup();

            void AddOption(string name, string hint = null, params string[] items);
            void AddValue(string name, string hint = null, bool @int = false, Range range = default);
            void AddVec2(string name, string hint = null, bool @int = false, Range range = default);
            void AddVec3(string name, string hint = null, bool @int = false, Range range = default);

            void AddText(string name, string hint = null);
            void AddCheck(string name, string hint = null);
            void AddColor(string name, string hint = null, bool noAlpha = true);
            void AddCurve(string name, string hint = null);
            void AddScene(string name, string hint = null);
            void AddTexture(string name, string hint = null);
            void AddResource(string name, string hint = null, bool fold = false, IEnumerable<ControlPair> controls = null);
            void AddResource<T>(string name, string hint = null, bool fold = false, bool nullable = true, IEnumerable<ControlPair> controls = null, Action<T> SetData = null) where T : Resource;
            void AddArray<T>(string name, string hint = null, bool fold = false, bool editable = true, Func<(IEnumerable<ControlPair> ItemControls, Action<T> SetItemData)> GetItemControls = null) where T : Resource;

            Control[] GetControls(params string[] names);
            T GetEditControl<T>(string name) where T : Control;
            T GetEditControlComponent<T>(string name, string part = null) where T : Control;
        }

        public static IEnumerable<ControlPair> Create<T>(out Action<T> SetData, params Action<IBuilder>[] build) where T : Resource
        {
            var builder = new BuilderUI();
            build?.ForEach(x => x?.Invoke(builder));
            SetData = x => builder.Data = x;
            return builder.Controls;
        }

        public static IEnumerable<ControlPair> Create(params Action<IBuilder>[] build)
        {
            var builder = new BuilderUI { Data = new() };
            build?.ForEach(x => x?.Invoke(builder));
            return builder.Controls;
        }

        private class ResRoot
        {
            private Resource _data;
            public Resource Data { get => _data; set => this.Set(ref _data, value, OnDataSet); }

            protected event Action DataChanged;

            private void OnDataSet()
            {
                OnDataChanged();

                if (Data is not null)
                    Data.Changed += OnDataChanged;

                void OnDataChanged()
                    => DataChanged?.Invoke();
            }
        }

        private class BuilderBase : ResRoot
        {
            private List<ControlPair> ControlList { get; } = [];
            public IEnumerable<ControlPair> Controls => ControlList;
            private Dictionary<string, ControlPair> ControlLookup { get; } = [];

            public Control[] GetControls(params string[] names)
            {
                return GetControls().ToArray();

                IEnumerable<Control> GetControls()
                {
                    foreach (var name in names)
                    {
                        var pair = ControlLookup[name];

                        yield return pair.Label;
                        yield return pair.EditControl;
                    }
                }
            }

            public T GetEditControl<T>(string name) where T : Control
                => (T)ControlLookup[name].EditControl;

            public T GetEditControlComponent<T>(string name, string part) where T : Control
                => GetEditControl<Control>(name).GetNode<T>(part ?? name);

            protected virtual void AddPair(Control label, Control control)
                => ControlList.Add((label, control));

            protected virtual void AddPair(string name, Control label, Control control)
            {
                ControlList.Add((label, control));
                ControlLookup.Add(name, (label, control));
            }
        }

        private class GroupBuilder : BuilderBase
        {
            private readonly Stack<Button> groupStack = [];

            private void StartGroup(Button group)
                => groupStack.Push(group);

            public void EndGroup()
                => groupStack.Pop();

            private bool HasGroup(out Button group)
                => groupStack.TryPeek(out group);

            protected void StartGroup(Button group, string prefix = null, bool init = true)
            {
                if (init) InitGroup(group, prefix);
                StartGroup(group);
            }

            protected void InitGroup(Button group, string prefix = null)
            {
                SetDepth();
                SetPrefix();
                LinkParent();

                void SetDepth()
                {
                    group.SetMeta("Depth", ParentDepth() + 1);

                    int ParentDepth()
                        => HasGroup(out var parent) ? (int)parent.GetMeta("Depth") : 0;
                }

                void SetPrefix()
                    => group.SetMeta("Prefix", prefix);

                void LinkParent()
                {
                    if (!HasGroup(out var parent)) return;

                    var childToggle = group.ButtonPressed;
                    OnParentToggle(parent.ButtonPressed);
                    parent.Toggled += OnParentToggle;

                    void OnParentToggle(bool on)
                    {
                        if (on) RestoreChildToggle();
                        else SaveChildToggleAndHide();

                        void RestoreChildToggle()
                            => group.ButtonPressed = childToggle;

                        void SaveChildToggleAndHide()
                        {
                            childToggle = group.ButtonPressed;
                            group.ButtonPressed = false;
                        }
                    }
                }
            }

            protected override void AddPair(Control label, Control control)
            {
                InitGroup(ref label, control);
                base.AddPair(label, control);
            }

            protected override void AddPair(string name, Control label, Control control)
            {
                InitGroup(ref label, control);
                base.AddPair(name, label, control);
            }

            protected void InitGroup(ref Control label, Control control)
            {
                if (HasGroup(out var group))
                {
                    TrimPrefix(label);
                    ApplyIndent(ref label);
                    HideWithGroup(label, control);
                }

                void TrimPrefix(Control label)
                {
                    var prefix = (string)group.GetMeta("Prefix");
                    if (string.IsNullOrEmpty(prefix)) return;

                    label.RecurseChildren<Label>(self: true)
                        .ForEach(x => x.Text = x.Text.TrimPrefix(prefix));
                }

                void ApplyIndent(ref Control label)
                {
                    var size = group.Size.X;
                    var depth = (int)group.GetMeta("Depth");
                    label = NewIndent(label, size, depth, out var SetIndentSize).Root;
                    group.Resized += () => SetIndentSize(group.Size.X);
                }

                void HideWithGroup(Control label, Control control)
                {
                    HideWithGroup(label);
                    HideWithGroup(control);

                    void HideWithGroup(Control x)
                    {
                        var @internal = false;
                        var currentVisibility = x.Visible;

                        x.VisibilityChanged += OnVisibilityChanged;
                        group.Toggled += SetGroupVisibility;
                        SetGroupVisibility(group.ButtonPressed);

                        void OnVisibilityChanged()
                        {
                            if (!@internal)
                                currentVisibility = x.Visible;
                        }

                        void SetGroupVisibility(bool on)
                        {
                            @internal = true;
                            x.Visible = on && currentVisibility;
                            @internal = false;
                        }
                    }
                }
            }
        }

        private class BuilderUI : GroupBuilder, IBuilder
        {
            public void AddSep()
                => AddPair(NewSep(), NewSep());

            public void AddGroup(string title, string prefix, bool fold)
            {
                var (root, group, _) = NewToggleTitle(title, null, fold);
                AddPair(root, NewSep());
                StartGroup(group, prefix);
            }

            public void AddOption(string name, string hint, string[] items)
            {
                var label = NewLabel(name, null, hint);
                var edit = GetEditControl();
                AddPair(name, label, edit);

                Control GetEditControl()
                {
                    var edit = NewOptionEdit(name, hint, items);
                    edit.ItemSelected += OnItemSelected;
                    DataChanged += OnDataChanged;
                    OnDataChanged();
                    return edit;

                    void OnItemSelected(long x)
                        => Data?.Set(name, (int)x);

                    void OnDataChanged()
                    {
                        edit.Disabled = Data is null;
                        var value = (int?)Data?.Get(name) ?? default;
                        if (edit.Selected != value)
                        {
                            edit.Select(value);
                            Debug.Assert(edit.Selected == value);
                            edit.EmitSignal(OptionButton.SignalName.ItemSelected, value);
                        }
                    }
                }
            }

            public void AddValue(string name, string hint, bool @int, Range range)
            {
                var label = NewLabel(name, null, hint);
                var edit = GetEditControl();
                AddPair(name, label, edit);

                Control GetEditControl()
                {
                    var edit = NewValueEdit(name, hint, @int, range);
                    edit.ValueChanged += OnValueChanged;
                    DataChanged += OnDataChanged;
                    OnDataChanged();
                    return edit;

                    void OnValueChanged(double x)
                        => Data?.Set(name, (float)x);

                    void OnDataChanged()
                    {
                        edit.Editable = Data is not null;
                        edit.Value = (float?)Data?.Get(name) ?? default;
                    }
                }
            }

            public void AddVec2(string name, string hint, bool @int, Range range)
            {
                var label = NewLabel(name, null, hint);
                var edit = GetEditControl();
                AddPair(name, label, edit);

                Control GetEditControl()
                {
                    var (root, edit) = NewMultiEdit(name, hint, @int, range, "X", "Y");
                    edit[0].ValueChanged += OnValueChanged;
                    edit[1].ValueChanged += OnValueChanged;
                    DataChanged += OnDataChanged;
                    OnDataChanged();
                    return root;

                    void OnValueChanged(double _)
                        => Data?.Set(name, new Vector2((float)edit[0].Value, (float)edit[1].Value));

                    void OnDataChanged()
                    {
                        edit[0].Editable = Data is not null;
                        edit[1].Editable = Data is not null;
                        var (x, y) = (Vector2?)Data?.Get(name) ?? default;
                        edit[0].Value = x;
                        edit[1].Value = y;
                    }
                }
            }

            public void AddVec3(string name, string hint, bool @int, Range range)
            {
                var label = NewLabel(name, null, hint);
                var edit = GetEditControl();
                AddPair(name, label, edit);

                Control GetEditControl()
                {
                    var (root, edit) = NewMultiEdit(name, hint, @int, range, "X", "Y", "Z");
                    edit[0].ValueChanged += OnValueChanged;
                    edit[1].ValueChanged += OnValueChanged;
                    edit[2].ValueChanged += OnValueChanged;
                    DataChanged += OnDataChanged;
                    OnDataChanged();
                    return root;

                    void OnValueChanged(double _)
                        => Data?.Set(name, new Vector3((float)edit[0].Value, (float)edit[1].Value, (float)edit[2].Value));

                    void OnDataChanged()
                    {
                        edit[0].Editable = Data is not null;
                        edit[1].Editable = Data is not null;
                        edit[2].Editable = Data is not null;
                        var (x, y, z) = (Vector3?)Data?.Get(name) ?? default;
                        edit[0].Value = x;
                        edit[1].Value = y;
                        edit[2].Value = z;
                    }
                }
            }

            public void AddText(string name, string hint)
            {
                var label = NewLabel(name, null, hint);
                var edit = GetEditControl();
                AddPair(name, label, edit);

                Control GetEditControl()
                {
                    var edit = NewTextEdit(name, hint);
                    edit.TextChanged += OnValueChanged;
                    DataChanged += OnDataChanged;
                    OnDataChanged();
                    return edit;

                    void OnValueChanged(string x)
                        => Data?.Set(name, x);

                    void OnDataChanged()
                    {
                        edit.Editable = Data is not null;
                        edit.Text = (string)Data?.Get(name);
                    }
                }
            }

            public void AddCheck(string name, string hint)
            {
                var label = NewLabel(name, null, hint);
                var edit = GetEditControl();
                AddPair(name, label, edit);

                Control GetEditControl()
                {
                    var edit = NewCheckEdit(name, hint);
                    edit.Toggled += OnValueChanged;
                    DataChanged += OnDataChanged;
                    OnDataChanged();
                    return edit;

                    void OnValueChanged(bool x)
                        => Data?.Set(name, x);

                    void OnDataChanged()
                    {
                        edit.Disabled = Data is null;
                        edit.ButtonPressed = (bool?)Data?.Get(name) ?? default;
                    }
                }
            }

            public void AddColor(string name, string hint, bool noAlpha)
            {
                var label = NewLabel(name, null, hint);
                var edit = GetEditControl();
                AddPair(name, label, edit);

                Control GetEditControl()
                {
                    var edit = NewColorEdit(name, hint);
                    edit.ColorChanged += OnValueChanged;
                    DataChanged += OnDataChanged;
                    OnDataChanged();
                    return edit;

                    void OnValueChanged(Color x)
                        => Data?.Set(name, x);

                    void OnDataChanged()
                    {
                        edit.Disabled = Data is null;
                        edit.Color = (Color?)Data?.Get(name) ?? default;
                    }
                }
            }

            public void AddCurve(string name, string hint)
            {
                var label = NewLabel(name, null, hint);
                var edit = GetEditControl();
                AddPair(name, label, edit);

                Control GetEditControl()
                {
                    var preview = NewCurvePreview(name, out var SetValue);
                    DataChanged += OnDataChanged;
                    OnDataChanged();
                    return preview;

                    void OnDataChanged()
                        => SetValue((Curve)Data?.Get(name));
                }
            }

            public void AddScene(string name, string hint)
            {
                var label = NewLabel(name, null, hint);
                var edit = GetEditControl();
                AddPair(name, label, edit);

                Control GetEditControl()
                {
                    var (root, _, clear, select, _) = NewSceneSelect(name, out var SetValue, out var _, OnValueChanged);
                    DataChanged += OnDataChanged;
                    OnDataChanged();
                    return root;

                    void OnValueChanged(PackedScene value)
                        => Data?.Set(name, value);

                    void OnDataChanged()
                    {
                        clear.Disabled = Data is null;
                        select.Disabled = Data is null;
                        SetValue((PackedScene)Data?.Get(name));
                    }
                }
            }

            public void AddTexture(string name, string hint)
            {
                var label = NewLabel(name, null, hint);
                var edit = GetEditControl();
                AddPair(name, label, edit);

                Control GetEditControl()
                {
                    var (root, _, clear, select, _) = NewTextureSelect(name, out var SetValue, out var _, OnValueChanged);
                    DataChanged += OnDataChanged;
                    OnDataChanged();
                    return root;

                    void OnValueChanged(Texture2D value)
                        => Data?.Set(name, value);

                    void OnDataChanged()
                    {
                        clear.Disabled = Data is null;
                        select.Disabled = Data is null;
                        SetValue((Texture2D)Data?.Get(name));
                    }
                }
            }

            public void AddResource(string name, string hint, bool fold, IEnumerable<(Control Label, Control EditControl)> controls)
            {
                var (root, group, _) = NewToggleTitle(name, hint, fold);
                AddPair(name, root, NewSep());

                StartGroup(group);
                controls.ForEach(x => AddPair(x.Label, x.EditControl));
                EndGroup();
            }

            public void AddResource<T>(string name, string hint, bool fold, bool nullable, IEnumerable<(Control Label, Control EditControl)> controls, Action<T> SetData) where T : Resource
            {
                var (root, group, _) = NewToggleTitle(name, hint, fold);
                var edit = nullable ? GetNullToggle() : NewSep();
                AddPair(name, root, edit);

                StartGroup(group);
                controls.ForEach(x => AddPair(x.Label, x.EditControl));
                EndGroup();

                Control GetNullToggle()
                {
                    var (root, edit) = NewToggleEdit(name, hint);
                    edit.Toggled += OnValueChanged;
                    DataChanged += OnDataChanged;
                    OnDataChanged();
                    return root;

                    void OnValueChanged(bool on)
                        => Data?.Set(name, on ? Activator.CreateInstance<T>() : null);

                    void OnDataChanged()
                    {
                        edit.Disabled = Data is null;
                        var newValue = (T)Data?.Get(name);

                        SetData(newValue);

                        edit.ButtonPressed = newValue is not null;
                        group.ButtonPressed = edit.ButtonPressed;
                    }
                }
            }

            public void AddArray<T>(string name, string hint, bool fold, bool editable, Func<(IEnumerable<ControlPair> ItemControls, Action<T> SetItemData)> GetItemControls) where T : Resource
            {
                var (arrayTitle, arrayGroup, arrayLabel) = NewToggleTitle(name, hint, fold);
                var arrayEdit = editable ? GetArrayHeaderEditControl() : NewSep();
                AddPair(name, arrayTitle, arrayEdit);
                InitGroup(arrayGroup);

                var itemData = GetData();
                var itemRoot = arrayEdit;
                var itemLast = itemRoot;
                var itemList = new List<(List<Control> ItemControls, Action<T> SetItemData)>();
                Debug.Assert(itemRoot == Controls.Last().EditControl, "ItemRoot should be last control added");

                var arrayPrefix = arrayLabel.Text;
                var itemPrefix = arrayPrefix.TrimEnd('s');

                UpdateItems();
                SetArrayTitle();
                DataChanged += OnDataChanged;

                void OnDataChanged()
                {
                    var newData = GetData();
                    if (newData == itemData) return;
                    itemData = newData;

                    UpdateItems();
                    SetArrayTitle();
                }

                void SetArrayTitle()
                    => arrayLabel.Text = $"{arrayPrefix} ({itemData.Length()})";

                string GetItemName(int idx)
                    => $"{itemPrefix}{idx}";

                Control GetArrayHeaderEditControl()
                {
                    var btnAdd = NewAddButton(name, AddItem);
                    var (root, _) = NewCustomEdit(name, NewSep(), btnAdd);
                    return root;
                }

                Control GetItemHeaderEditControl(string itemName, int idx)
                {
                    var btnUp = NewUpButton(itemName, () => MoveItemUp(idx));
                    var btnDown = NewDownButton(itemName, () => MoveItemDown(idx));
                    var btnRemove = NewCloseButton(itemName, () => RemoveItem(idx));
                    var (root, _) = NewCustomEdit(itemName, NewSep(), btnUp, btnDown, btnRemove);
                    return root;
                }

                T NewData() => Activator.CreateInstance<T>();
                void SetData(T[] array) => Data?.Set(name, array);
                T[] GetData() => Data?.Get(name).AsGodotObjectArray<T>();

                void AddItem() => SetData(GetData().Add(NewData()));
                void RemoveItem(int idx) => SetData(GetData().Remove(idx));
                void MoveItemUp(int idx) => SetData(GetData().ShiftLeft(idx));
                void MoveItemDown(int idx) => SetData(GetData().ShiftRight(idx));

                void UpdateItems()
                {
                    if (itemRoot.GetParent() is null) return;

                    StartGroup(arrayGroup, init: false);
                    UpdateItems();
                    EndGroup();

                    void UpdateItems()
                    {
                        CreateControls();
                        RemoveControls();

                        void CreateControls()
                        {
                            itemLast = itemRoot;

                            for (var idx = 0; idx < itemData.Length(); ++idx)
                            {
                                if (idx < itemList.Count)
                                    ReuseExistingControls(idx);
                                else
                                    CreateNewControls(idx);
                            }

                            void ReuseExistingControls(int idx)
                            {
                                itemList[idx].SetItemData(itemData[idx]);
                                itemLast = itemList[idx].ItemControls.Last();
                            }

                            void CreateNewControls(int idx)
                            {
                                var itemName = GetItemName(idx);
                                var (itemControls, SetItemData) = GetItemControls();

                                SetItemData(itemData[idx]);

                                Debug.Assert(itemList.Count == idx);
                                var addedItems = new List<Control>();
                                itemList.Add((addedItems, SetItemData));

                                var (itemTitle, itemGroup, _) = NewToggleTitle(itemName, hint, fold);
                                var itemEdit = editable ? GetItemHeaderEditControl(itemName, idx) : NewSep();
                                AddItem(itemTitle, itemEdit);

                                StartGroup(itemGroup);
                                itemControls.ForEach(x => AddItem(x.Label, x.EditControl));
                                EndGroup();

                                void AddItem(Control label, Control control)
                                {
                                    InitGroup(ref label, control);
                                    Add(label); Add(control);

                                    void Add(Control x)
                                    {
                                        itemLast.AddSibling(x, forceReadableName: true);
                                        itemLast = x;

                                        addedItems.Add(x);
                                    }
                                }
                            }
                        }

                        void RemoveControls()
                        {
                            RemoveControls();
                            PurgeList();

                            void RemoveControls()
                            {
                                var parent = itemRoot.GetParent();
                                for (var idx = itemData.Length(); idx < itemList.Count; ++idx)
                                    itemList[idx].ItemControls.ForEach(x => parent.RemoveChild(x, free: true));
                            }

                            void PurgeList()
                                => itemList.RemoveRange(itemData.Length(), itemList.Count - itemData.Length());
                        }
                    }
                }
            }
        }
    }
}
