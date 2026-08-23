import type { ColumnDef } from '@tanstack/table-core';

import DataTableSortButton from '$lib/components/custom/table-common/data-table-sort-button.svelte';
import { renderComponent } from '$lib/components/ui/data-table';
import { formatCurrency } from '$lib/index';
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
		header: ({ column }) =>
			renderComponent(DataTableSortButton, {
				onclick: column.getToggleSortingHandler(),
				title: 'Name'
			})
	},
	{
		accessorKey: 'amount',
		header: ({ column }) =>
			renderComponent(DataTableSortButton, {
				onclick: column.getToggleSortingHandler(),
				title: 'Amount'
			}),
		cell: ({ row }) => {
			return formatCurrency(row.original.amount, row.original.currency);
		}
	},
	{
		accessorKey: 'actionedAt',
		header: ({ column }) =>
			renderComponent(DataTableSortButton, {
				onclick: column.getToggleSortingHandler(),
				title: 'Date'
			}),
		cell: ({ row }) => Intl.DateTimeFormat('en-MY').format(new Date(row.original.actionedAt))
	},
	{
		accessorKey: 'accountName',
		header: 'Account',
		cell: ({ row }) => row.original.accountName ?? '—'
	},
	{
		id: 'actions',
		cell: ({ row }) => {
			return renderComponent(DataTableActions, { id: row.original.id });
		}
	}
];
