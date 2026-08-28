import type { ColumnDef } from '@tanstack/table-core';

import DataTableSortButton from '$lib/components/custom/table-common/data-table-sort-button.svelte';
import { renderComponent } from '$lib/components/ui/data-table';
import type { PiggyBank } from '$lib/services/types';

import SavingsGoalsStatus from './savings-goals-status.svelte';

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
		header: 'Saved / Target',
		accessorFn: (row) =>
			row.amount.toLocaleString('en-MY', { style: 'currency', currency: 'MYR' }) +
			' / ' +
			row.target.toLocaleString('en-MY', { style: 'currency', currency: 'MYR' })
	},
	{
		header: 'Status',
		cell: ({ row }) => {
			const progress = (row.original.amount / row.original.target) * 100;
			return renderComponent(SavingsGoalsStatus, { progress });
		}
	},
	{
		accessorFn: (row) =>
			row.deadline ? Intl.DateTimeFormat('en-MY').format(new Date(row.deadline)) : 'N/A',
		header: 'Deadline'
	}
];
