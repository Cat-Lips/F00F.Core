using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Godot;

namespace F00F;

[Tool]
public partial class DataGrid : Tree
{
    #region Private

    private const string RowId = "RowId";

    private TreeItem Root { get; set; }
    private readonly Dictionary<ulong, TreeItem> Rows = [];

    #endregion

    protected virtual int ColumnCount => 0;
    protected virtual void InitColumns() { }
    protected virtual void InitRows() { }

    public event Action SelectionChanged;
    public event Action SelectionActivated;
    public TreeItem SelectedItem { get; private set => this.Set(ref field, value, SelectionChanged); }
    public ulong? SelectedId => (ulong?)SelectedItem?.GetMeta(RowId);

    public event Action Empty;
    public bool IsEmpty { get; private set => this.Set(ref field, value, Empty); }
    public bool HasRows => Root?.GetChildCount() > 0;

    #region Godot

    protected virtual void OnReady() { }
    public sealed override void _Ready()
    {
        Root = CreateItem();
        HideRoot = true;

        Columns = ColumnCount;
        if (Columns.NotZero())
            InitColumns();

        if (Editor.NotEditor)
        {
            InitRows();
            InitEvents();
            OnReady();
        }

        void InitEvents()
        {
            NothingSelected += SetSelectedItem;
            ItemActivated += OnItemActivated;
            ItemSelected += SetSelectedItem;

            void OnItemActivated()
                => SelectionActivated?.Invoke();

            void SetSelectedItem()
                => SelectedItem = GetSelected();
        }
    }

    #endregion

    #region Helpers

    protected new void SetColumnTitle(int column, [CallerArgumentExpression(nameof(column))] string title = null)
        => base.SetColumnTitle(column, title.Capitalise());

    protected TreeItem Row(ulong id) => Rows[id];
    protected TreeItem GetRow(ulong id) => Rows[id];

    protected TreeItem InsertRow(ulong id) => NewRow(id);
    protected TreeItem AddRow(ulong id) => NewRow(id);
    protected TreeItem NewRow(ulong id)
    {
        var row = CreateItem(Root);
        row.SetMeta(RowId, id);
        Rows.Add(id, row);
        OnRowAdded();
        return row;
    }

    protected void RemoveRow(ulong id)
    {
        if (Rows.Remove(id, out var row))
        {
            row.Free();
            OnRowRemoved();
        }
    }

    protected void ResetRows<TRowData>(IEnumerable<TRowData> RowData, Action<TRowData> InitRow)
    {
        var CurrentSelection = SelectedId;
        Clear(); RowData.ForEach(InitRow);
        SetSelectedItem(CurrentSelection);
        OnRowsReset();

        void SetSelectedItem(ulong? id)
            => SelectedItem = id.HasValue ? Rows.TryGet(id.Value) : null;
    }

    protected void ClearRows()
    {
        Clear();
        OnRowsRemoved();
    }

    private new void Clear()
    {
        base.Clear();
        Root = CreateItem();
    }

    private void OnRowAdded() => IsEmpty = !HasRows;
    private void OnRowsReset() => IsEmpty = !HasRows;
    private void OnRowRemoved() => IsEmpty = !HasRows;
    private void OnRowsRemoved() => IsEmpty = !HasRows;

    #endregion
}
