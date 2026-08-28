import type { ColumnDef } from '@tanstack/table-core';

import { formatCurrency } from '$lib';
import DataTableSortButton from '$lib/components/custom/table-common/data-table-sort-button.svelte';
import { renderComponent } from '$lib/components/ui/data-table';
import type { Expense, Income } from '$lib/services/types';

import RecentActivityKind from './recent-activity-kind.svelte';

export type RecentActivityRow = {
	id: string;
	kind: 'expense' | 'income';
	name: string;
	categoryOrAccount: string;
	amount: number;
	currency: string;
	actionedAt: string;
};

export function mergeRecentActivity(expenses: Expense[], incomes: Income[]): RecentActivityRow[] {
	const expenseRows: RecentActivityRow[] = expenses.map((e) => ({
		id: `expense-${e.id}`,
		kind: 'expense',
		name: e.name,
		categoryOrAccount: e.categoryName ?? e.accountName ?? 'Uncategorized',
		amount: e.amount,
		currency: e.currency,
		actionedAt: e.actionedAt
	}));
	const incomeRows: RecentActivityRow[] = incomes.map((i) => ({
		id: `income-${i.id}`,
		kind: 'income',
		name: i.name,
		categoryOrAccount: i.accountName ?? 'Uncategorized',
		amount: i.amount,
		currency: i.currency,
		actionedAt: i.actionedAt
	}));

	return [...expenseRows, ...incomeRows].sort(
		(a, b) => new Date(b.actionedAt).getTime() - new Date(a.actionedAt).getTime()
	);
}

export const columns: ColumnDef<RecentActivityRow>[] = [
	{
		header: 'No.',
		id: 'rowNumber',
		cell: ({ row, table }) => {
			const { pageIndex, pageSize } = table.getState().pagination;
			return pageIndex * pageSize + row.index + 1;
		}
	},
	{
		accessorKey: 'actionedAt',
		header: ({ column }) =>
			renderComponent(DataTableSortButton, {
				onclick: column.getToggleSortingHandler(),
				title: 'Date'
			}),
		cell: ({ row }) => Intl.DateTimeFormat('en-MY').format(new Date(row.original.actionedAt))
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
		accessorKey: 'categoryOrAccount',
		header: 'Category / Account'
	},
	{
		id: 'kind',
		header: 'Type',
		cell: ({ row }) => renderComponent(RecentActivityKind, { kind: row.original.kind })
	},
	{
		accessorKey: 'amount',
		header: ({ column }) =>
			renderComponent(DataTableSortButton, {
				onclick: column.getToggleSortingHandler(),
				title: 'Amount'
			}),
		cell: ({ row }) => formatCurrency(row.original.amount, row.original.currency)
	}
];
