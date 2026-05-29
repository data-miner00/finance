import { renderComponent } from '$lib/components/ui/data-table';
import type { RecurringAction } from '$lib/services/types';
import type { ColumnDef } from '@tanstack/table-core';
import DataTableActions from './data-table-actions.svelte';

const recurringTypeLabels = ['Expense', 'Income'] as const;

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
		header: 'Name'
	},
	{
		header: 'Amount',
		accessorFn: (row) => row.amount.toLocaleString('en-MY', { style: 'currency', currency: 'MYR' })
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
		accessorKey: 'recurringAt',
		header: 'Recurring At'
	},
	{
		accessorKey: 'createdAt',
		header: 'Date'
	},
	{
		id: 'actions',
		cell: ({ row }) => {
			return renderComponent(DataTableActions, { id: row.original.id });
		}
	}
];
