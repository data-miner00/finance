import type { ColumnDef } from '@tanstack/table-core';

import { renderComponent } from '$lib/components/ui/data-table';
import type { Income } from '$lib/services/types';

import DataTableActions from './data-table-actions.svelte';

export const columns: ColumnDef<Income>[] = [
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
		header: 'Amount',
		accessorFn: (row) => row.amount.toLocaleString('en-MY', { style: 'currency', currency: 'MYR' })
	},
	{
		accessorFn: (row) => Intl.DateTimeFormat('en-MY').format(new Date(row.actionedAt)),
		header: 'Date'
	},
	{
		id: 'actions',
		cell: ({ row }) => {
			return renderComponent(DataTableActions, { id: row.original.id });
		}
	}
];
