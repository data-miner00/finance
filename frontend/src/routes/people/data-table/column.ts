import type { ColumnDef } from '@tanstack/table-core';

import DataTableSortButton from '$lib/components/custom/table-common/data-table-sort-button.svelte';
import { renderComponent } from '$lib/components/ui/data-table';
import type { Person } from '$lib/services/types';

import DataTableActions from './data-table-actions.svelte';

export const columns: ColumnDef<Person>[] = [
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
		header: 'Alias',
		accessorFn: (row) => row.alias || '-'
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
