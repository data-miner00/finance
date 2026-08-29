import type { ColumnDef } from '@tanstack/table-core';

import { formatCurrency } from '$lib';
import DataTableSortButton from '$lib/components/custom/table-common/data-table-sort-button.svelte';
import { renderComponent } from '$lib/components/ui/data-table';
import type { RecurringAction } from '$lib/services/types';

const recurringTypeLabels = ['Expense', 'Income'] as const;
const recurrenceTypeLabels = ['Daily', 'Weekly', 'Monthly', 'Yearly'] as const;

export const columns: ColumnDef<RecurringAction>[] = [
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
		cell: ({ row }) => formatCurrency(row.original.amount, row.original.currency)
	},
	{
		accessorKey: 'type',
		header: 'Type',
		cell: ({ getValue }) => {
			const value = getValue<number>();
			return recurringTypeLabels[value] ?? 'Unknown';
		}
	},
	{
		accessorKey: 'recurrenceType',
		header: 'Frequency',
		cell: ({ getValue }) => {
			const value = getValue<number>();
			return recurrenceTypeLabels[value] ?? 'Unknown';
		}
	},
	{
		accessorKey: 'recurringAt',
		header: ({ column }) =>
			renderComponent(DataTableSortButton, {
				onclick: column.getToggleSortingHandler(),
				title: 'Next Date'
			}),
		cell: ({ row }) => Intl.DateTimeFormat('en-MY').format(new Date(row.original.recurringAt))
	}
];
