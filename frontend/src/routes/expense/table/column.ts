import { renderComponent } from '$lib/components/ui/data-table';
import type { Expense } from '$lib/services/types';
import type { ColumnDef } from '@tanstack/table-core';
import DataTableActions from './data-table-actions.svelte';
export const columns: ColumnDef<Expense>[] = [
	{
		accessorKey: 'name',
		header: 'Name'
	},
	{
		accessorKey: 'category',
		header: 'Category'
	},
	{
		accessorKey: 'currency',
		header: 'Currency'
	},
	{
		accessorKey: 'amount',
		header: 'Amount'
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
