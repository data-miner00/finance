import type { ColumnDef } from '@tanstack/table-core';

import { renderComponent } from '$lib/components/ui/data-table';
import type { Expense } from '$lib/services/types';

import DataTableActions from './data-table-actions.svelte';
import DataTableAmountButton from './data-table-amount-button.svelte';

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
		accessorKey: 'categoryName',
		header: 'Category'
	},
	{
		cell: () => 'Credit Card',
		header: 'Method'
	},
	{
		accessorKey: 'amount',
		header: ({ column }) =>
			renderComponent(DataTableAmountButton, {
				onclick: column.getToggleSortingHandler()
			}),
		cell: ({ row }) => {
			return row.original.amount.toLocaleString('en-MY', { style: 'currency', currency: 'MYR' });
		}
	},
	{
		accessorFn: (row) => Intl.DateTimeFormat('en-MY').format(new Date(row.createdAt)),
		header: 'Date'
	},
	{
		id: 'actions',
		cell: ({ row }) => {
			return renderComponent(DataTableActions, { id: row.original.id });
		}
	}
];
