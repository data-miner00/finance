import { renderComponent } from '$lib/components/ui/data-table';
import type { Expense } from '$lib/services/types';
import type { ColumnDef } from '@tanstack/table-core';
import DataTableActions from './data-table-actions.svelte';
export const columns: ColumnDef<Expense>[] = [
	{
		header: 'No.',
		id: 'rowNumber',
		cell: ({ row, table }) => {
			const { pageIndex, pageSize } = table.getState().pagination;
			return pageIndex * pageSize + row.index + 1;
		}
	},
	{
		accessorKey: 'name',
		header: 'Name'
	},
	{
		accessorKey: 'category',
		header: 'Category'
	},
	{
		cell: () => 'Credit Card',
		header: 'Method'
	},
	{
		header: 'Amount',
		accessorFn: (row) => row.amount.toLocaleString('en-MY', { style: 'currency', currency: 'MYR' })
	},
	{
		accessorKey: 'createdAt',
		header: 'Date'
	},
	{
		id: 'actions',
		cell: ({ row }) => {
			return renderComponent(DataTableActions, { id: row.original.id });
		}
	}
];
