import type { ColumnDef } from '@tanstack/table-core';

import { formatCurrency } from '$lib';
import DataTableSortButton from '$lib/components/custom/table-common/data-table-sort-button.svelte';
import { renderComponent } from '$lib/components/ui/data-table';
import { type Account, AccountType } from '$lib/services/types';

import DataTableActions from './data-table-actions.svelte';
import DataTableStatus from './data-table-status.svelte';

export type AccountRow = Account & {
	spentThisYear: number;
	/** Progress toward `annualSpendTarget` as a percentage, or `null` when no target is set. */
	annualSpendProgress: number | null;
};

export const columns: ColumnDef<AccountRow>[] = [
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
			return formatCurrency(row.original.balance, row.original.currency);
		}
	},
	{
		id: 'annualSpendTarget',
		header: 'Annual Spend Target',
		accessorFn: (row) =>
			row.annualSpendTarget != null ? formatCurrency(row.annualSpendTarget) : '—'
	},
	{
		id: 'spentThisYear',
		header: 'Spent YTD',
		accessorFn: (row) => (row.annualSpendTarget != null ? formatCurrency(row.spentThisYear) : '—')
	},
	{
		id: 'annualSpendStatus',
		header: 'Goal Status',
		cell: ({ row }) => {
			const progress = row.original.annualSpendProgress;
			return progress != null ? renderComponent(DataTableStatus, { progress }) : '—';
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
