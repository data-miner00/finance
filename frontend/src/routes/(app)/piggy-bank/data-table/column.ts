import type { ColumnDef } from '@tanstack/table-core';

import DataTableSortButton from '$lib/components/custom/table-common/data-table-sort-button.svelte';
import { renderComponent } from '$lib/components/ui/data-table';
import type { PiggyBank } from '$lib/services/types';

import DataTableActions from './data-table-actions.svelte';
import DataTableStatus from './data-table-status.svelte';

export const columns: ColumnDef<PiggyBank>[] = [
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
		header: 'Target Amount',
		accessorFn: (row) =>
			row.amount.toLocaleString('en-MY', { style: 'currency', currency: 'MYR' }) +
			' / ' +
			row.target.toLocaleString('en-MY', { style: 'currency', currency: 'MYR' })
	},
	{
		header: 'Status',
		cell: ({ row }) => {
			const piggyBank = row.original;
			const progress = (piggyBank.amount / piggyBank.target) * 100;
			return renderComponent(DataTableStatus, { progress });
		}
	},
	{
		header: 'Progress',
		accessorFn: (row) => {
			const progress = (row.amount / row.target) * 100;
			return `${progress.toFixed(2)}%`;
		}
	},
	{
		accessorFn: (row) =>
			row.deadline ? Intl.DateTimeFormat('en-MY').format(new Date(row.deadline)) : 'N/A',
		header: 'Deadline'
	},
	{
		accessorKey: 'updatedAt',
		header: ({ column }) =>
			renderComponent(DataTableSortButton, {
				onclick: column.getToggleSortingHandler(),
				title: 'Date'
			}),
		cell: ({ row }) => Intl.DateTimeFormat('en-MY').format(new Date(row.original.updatedAt))
	},
	{
		id: 'actions',
		cell: ({ row }) => {
			return renderComponent(DataTableActions, { id: row.original.id });
		}
	}
];
