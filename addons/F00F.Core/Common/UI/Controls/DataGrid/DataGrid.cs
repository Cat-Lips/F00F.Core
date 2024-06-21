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
    protected TreeItem Root { get; private set; }
    protected readonly Dictionary<ulong, TreeItem> Rows = [];

    #endregion

    protected virtual int ColumnCount => 0;
    protected virtual void InitColumns() { }
    protected virtual void InitRows() { }

    public event Action SelectionChanged;
    public event Action SelectionActivated;
    public TreeItem SelectedItem { get; private set => this.Set(ref field, value, SelectionChanged); }
    public ulong? SelectedId => (ulong?)SelectedItem?.GetMeta(RowId);

    #region Godot

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
        }
    }

    #endregion

    #region Private

    private void InitEvents()
    {
        NothingSelected += SetSelectedItem;
        ItemActivated += OnItemActivated;
        ItemSelected += SetSelectedItem;

        void OnItemActivated()
            => SelectionActivated?.Invoke();

        void SetSelectedItem()
            => SelectedItem = GetSelected();
    }

    #region Helpers

    protected new void SetColumnTitle(int column, [CallerArgumentExpression(nameof(column))] string title = null)
        => base.SetColumnTitle(column, title.Capitalise());

    protected new void Clear()
    {
        base.Clear();
        Root = CreateItem();
    }

    protected TreeItem NewRow(ulong id)
    {
        var row = CreateItem(Root);
        row.SetMeta(RowId, id);
        Rows.Add(id, row);
        return row;
    }

    protected void ResetRows<TRowData>(IEnumerable<TRowData> RowData, Action<TRowData> InitRow)
    {
        var CurrentSelection = SelectedId;
        Clear(); RowData.ForEach(InitRow);
        SetSelectedItem(CurrentSelection);

        void SetSelectedItem(ulong? id)
            => SelectedItem = id.HasValue ? Rows.TryGet(id.Value) : null;
    }

    #endregion

    #endregion
}
