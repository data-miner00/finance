import { describe, expect, it } from 'vitest';

import { computeBudgetRows } from './budget';
import type { Category, Expense } from './services/types';

const now = new Date('2026-08-15T12:00:00');

function makeCategory(overrides: Partial<Category> = {}): Category {
	return {
		id: crypto.randomUUID(),
		createdAt: '2026-08-01T10:00:00',
		updatedAt: '2026-08-01T10:00:00',
		name: 'Food',
		...overrides
	};
}

function makeExpense(overrides: Partial<Expense> = {}): Expense {
	return {
		id: crypto.randomUUID(),
		createdAt: '2026-08-01T10:00:00',
		updatedAt: '2026-08-01T10:00:00',
		name: 'Test expense',
		amount: 10,
		currency: 'MYR',
		actionedAt: '2026-08-01T10:00:00',
		...overrides
	};
}

describe('computeBudgetRows', () => {
	it('returns nothing for categories without a budget', () => {
		const categories = [makeCategory({ budgetAmount: null })];
		expect(computeBudgetRows(categories, [], now)).toEqual([]);
	});

	it('sums only this-month expenses for the matching category', () => {
		const categories = [makeCategory({ name: 'Food', budgetAmount: 100 })];
		const expenses = [
			makeExpense({ categoryName: 'Food', amount: 30, actionedAt: '2026-08-05T10:00:00' }),
			makeExpense({ categoryName: 'Food', amount: 20, actionedAt: '2026-08-10T10:00:00' }),
			makeExpense({ categoryName: 'Food', amount: 999, actionedAt: '2026-07-10T10:00:00' }),
			makeExpense({ categoryName: 'Transport', amount: 999, actionedAt: '2026-08-10T10:00:00' })
		];

		const rows = computeBudgetRows(categories, expenses, now);
		expect(rows).toHaveLength(1);
		expect(rows[0]).toMatchObject({ budgetAmount: 100, spent: 50, remaining: 50, progress: 50 });
	});

	it('reports progress over 100 when spend exceeds budget', () => {
		const categories = [makeCategory({ name: 'Food', budgetAmount: 100 })];
		const expenses = [
			makeExpense({ categoryName: 'Food', amount: 150, actionedAt: '2026-08-05T10:00:00' })
		];

		const rows = computeBudgetRows(categories, expenses, now);
		expect(rows[0]).toMatchObject({ spent: 150, remaining: -50, progress: 150 });
	});

	it('does not divide by zero when budgetAmount is 0', () => {
		const categories = [makeCategory({ name: 'Food', budgetAmount: 0 })];
		const rows = computeBudgetRows(categories, [], now);
		expect(rows[0].progress).toBe(0);
	});
});
