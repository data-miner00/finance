import type { ColumnDef } from '@tanstack/table-core';

import { formatCurrency } from '$lib';
import type { BudgetRow } from '$lib/budget';
import { getCategoryIcon } from '$lib/category-icons';
import DataTableSortButton from '$lib/components/custom/table-common/data-table-sort-button.svelte';
import { renderComponent } from '$lib/components/ui/data-table';

import DataTableActions from './data-table-actions.svelte';
import DataTableStatus from './data-table-status.svelte';

export type { BudgetRow };

export const columns: ColumnDef<BudgetRow>[] = [
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
				title: 'Category'
			})
	},
	{
		id: 'budgetAmount',
		header: 'Budgeted',
		accessorFn: (row) => formatCurrency(row.budgetAmount)
	},
	{
		id: 'spent',
		header: 'Spent',
		accessorFn: (row) => formatCurrency(row.spent)
	},
	{
		id: 'remaining',
		header: 'Remaining',
		accessorFn: (row) => formatCurrency(row.remaining)
	},
	{
		id: 'status',
		header: 'Status',
		cell: ({ row }) => renderComponent(DataTableStatus, { progress: row.original.progress })
	},
	{
		id: 'progress',
		header: 'Progress',
		accessorFn: (row) => `${row.progress.toFixed(2)}%`
	},
	{
		id: 'actions',
		cell: ({ row }) => {
			return renderComponent(DataTableActions, { id: row.original.id });
		}
	}
];
