import type { ColumnDef } from '@tanstack/table-core';

import DataTableSortButton from '$lib/components/custom/table-common/data-table-sort-button.svelte';
import { renderComponent } from '$lib/components/ui/data-table';
import type { RecurringAction } from '$lib/services/types';

import DataTableActions from './data-table-actions.svelte';

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
		cell: ({ row }) => {
			return row.original.amount.toLocaleString('en-MY', { style: 'currency', currency: 'MYR' });
		}
	},
	{
		accessorKey: 'type',
		header: 'Type',
		cell: ({ getValue }) => {
			const typeValue = getValue<number>();
			return recurringTypeLabels[typeValue] ?? 'Unknown';
		}
	},
	{
		accessorKey: 'intervalValue',
		header: 'Interval Value',
		cell: ({ getValue }) => {
			const intervalValue = getValue<number>();
			return intervalValue === 1 ? 'Every' : `Every ${intervalValue}`;
		}
	},
	{
		accessorKey: 'recurrenceType',
		header: 'Recurrence',
		cell: ({ getValue }) => {
			const recurrenceTypeValue = getValue<number>();
			return recurrenceTypeLabels[recurrenceTypeValue] ?? 'Unknown';
		}
	},
	{
		accessorKey: 'recurringAt',
		header: ({ column }) =>
			renderComponent(DataTableSortButton, {
				onclick: column.getToggleSortingHandler(),
				title: 'Next Action Date'
			}),
		cell: ({ row }) => Intl.DateTimeFormat('en-MY').format(new Date(row.original.recurringAt))
	},
	{
		id: 'actions',
		cell: ({ row }) => {
			return renderComponent(DataTableActions, { id: row.original.id });
		}
	}
];
