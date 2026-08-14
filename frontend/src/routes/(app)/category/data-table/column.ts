import type { ColumnDef } from '@tanstack/table-core';

import { getCategoryIcon } from '$lib/category-icons';
import DataTableSortButton from '$lib/components/custom/table-common/data-table-sort-button.svelte';
import { renderComponent } from '$lib/components/ui/data-table';
import type { Category } from '$lib/services/types';
import { appState } from '$lib/states.svelte';

import ColorSwatch from './color-swatch.svelte';
import DataTableActions from './data-table-actions.svelte';

export const columns: ColumnDef<Category>[] = [
	{
		header: 'No.',
		id: 'rowNumber',
		cell: ({ row, table }) => {
			const { pageIndex, pageSize } = table.getState().pagination;
			return pageIndex * pageSize + row.index + 1;
		}
	},
	{
		id: 'icon',
		header: 'Icon',
		cell: ({ row }) => {
			const Icon = getCategoryIcon(row.original.icon);
			return renderComponent(Icon, { class: 'size-4' });
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
		id: 'color',
		header: 'Color',
		cell: ({ row }) => renderComponent(ColorSwatch, { color: row.original.color })
	},
	{
		id: 'usageCount',
		header: 'Used By',
		accessorFn: (row) => appState.expenses.filter((e) => e.categoryName === row.name).length,
		cell: ({ row }) => {
			const count = appState.expenses.filter((e) => e.categoryName === row.original.name).length;
			return `${count} expense${count === 1 ? '' : 's'}`;
		}
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
