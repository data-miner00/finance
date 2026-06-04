import { renderComponent } from '$lib/components/ui/data-table';
import { AccountType, type Account } from '$lib/services/types';
import type { ColumnDef } from '@tanstack/table-core';
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
		header: 'Name'
	},
	{
		accessorFn: (row) => {
			switch (row.type) {
				case AccountType.Bank:
					return 'Bank';
				case AccountType.EWallet:
					return 'E-Wallet';
				case AccountType.Cash:
					return 'Cash';
				case AccountType.App:
					return 'App';
				case AccountType.Card:
					return 'Card';
			}
		},
		header: 'Type'
	},
	{
		header: 'Balance',
		accessorFn: (row) => row.balance.toLocaleString('en-MY', { style: 'currency', currency: 'MYR' })
	},
	{
		accessorFn: (row) => Intl.DateTimeFormat('en-MY').format(new Date(row.updatedAt)),
		header: 'Updated'
	},
	{
		id: 'actions',
		cell: ({ row }) => {
			return renderComponent(DataTableActions, { id: row.original.id });
		}
	}
];
