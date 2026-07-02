import type { ColumnDef } from '@tanstack/table-core';

import DataTableSortButton from '$lib/components/custom/table-common/data-table-sort-button.svelte';
import { renderComponent } from '$lib/components/ui/data-table';
import { type Account, AccountType } from '$lib/services/types';

import DataTableActions from './data-table-actions.svelte';

export const columns: ColumnDef<Account>[] = [
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
		accessorFn: (row) => {
			switch (row.type) {
				case AccountType.Savings:
					return 'Savings';
				case AccountType.EWallet:
					return 'E-Wallet';
				case AccountType.Cash:
					return 'Cash';
				case AccountType.App:
					return 'App';
				case AccountType.CreditCard:
					return 'Credit Card';
			}
		},
		header: 'Type'
	},
	{
		accessorKey: 'balance',
		header: ({ column }) =>
			renderComponent(DataTableSortButton, {
				onclick: column.getToggleSortingHandler(),
				title: 'Balance'
			}),
		cell: ({ row }) => {
			return row.original.balance.toLocaleString('en-MY', { style: 'currency', currency: 'MYR' });
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
