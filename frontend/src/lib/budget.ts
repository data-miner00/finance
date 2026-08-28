import type { Category, Expense } from '$lib/services/types';

export type BudgetRow = {
	id: string;
	name: string;
	icon?: string | null;
	budgetAmount: number;
	spent: number;
	remaining: number;
	progress: number;
};

// Takes an injected `now` (rather than $lib's isCurrentMonth, which hardcodes new Date())
// so this is testable with a fixed date, matching the pattern in insights.ts.
function isSameMonth(dateString: string, now: Date): boolean {
	const date = new Date(dateString);
	return date.getMonth() === now.getMonth() && date.getFullYear() === now.getFullYear();
}

export function computeBudgetRows(
	categories: Category[],
	expenses: Expense[],
	now: Date = new Date()
): BudgetRow[] {
	return categories
		.filter((c) => c.budgetAmount != null)
		.map((category) => {
			const spent = expenses
				.filter((e) => e.categoryName === category.name && isSameMonth(e.actionedAt, now))
				.reduce((sum, e) => sum + e.amount, 0);
			const budgetAmount = category.budgetAmount ?? 0;
			return {
				id: category.id,
				name: category.name,
				icon: category.icon,
				budgetAmount,
				spent,
				remaining: budgetAmount - spent,
				progress: budgetAmount > 0 ? (spent / budgetAmount) * 100 : 0
			};
		});
}
