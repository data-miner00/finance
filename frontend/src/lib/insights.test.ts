import { describe, expect, it } from 'vitest';

import { MAX_CATEGORY_CHANGE_INSIGHTS, generateInsights } from './insights';
import type { Expense } from './services/types';

// Saturday. The last-30-day window (2026-07-17 → 2026-08-15) contains 9 weekend
// days and 21 weekdays.
const now = new Date('2026-08-15T12:00:00');

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

function monthlySpend(categoryName: string, month: string, amounts: number[]): Expense[] {
	return amounts.map((amount, i) =>
		makeExpense({
			categoryName,
			amount,
			actionedAt: `${month}-${String(i + 1).padStart(2, '0')}T10:00:00`
		})
	);
}

describe('generateInsights', () => {
	it('returns nothing for empty data', () => {
		expect(generateInsights([], { now })).toEqual([]);
	});

	it('orders insights by kind: category changes, largest expense, weekend pattern, top categories', () => {
		const expenses = [
			...monthlySpend('Food', '2026-08', [500, 10, 10, 10, 10]),
			...monthlySpend('Food', '2026-07', [100]),
			...monthlySpend('Transport', '2026-08', [5, 5, 5, 5, 5, 5])
		];
		const kinds = generateInsights(expenses, { now }).map((i) => i.kind);
		expect(kinds).toEqual([
			'category-change',
			'largest-expense',
			'weekend-pattern',
			'top-categories'
		]);
	});
});

describe('category spend change', () => {
	it('emits a negative insight when a category increases 25% month over month', () => {
		const expenses = [
			...monthlySpend('Food', '2026-07', [400]),
			...monthlySpend('Food', '2026-08', [500])
		];
		const insights = generateInsights(expenses, { now }).filter(
			(i) => i.kind === 'category-change'
		);
		expect(insights).toHaveLength(1);
		expect(insights[0].tone).toBe('negative');
		expect(insights[0].categoryName).toBe('Food');
		expect(insights[0].message).toContain('increased 25.0%');
	});

	it('emits a positive insight when a category decreases', () => {
		const expenses = [
			...monthlySpend('Food', '2026-07', [500]),
			...monthlySpend('Food', '2026-08', [250])
		];
		const insights = generateInsights(expenses, { now }).filter(
			(i) => i.kind === 'category-change'
		);
		expect(insights).toHaveLength(1);
		expect(insights[0].tone).toBe('positive');
		expect(insights[0].message).toContain('decreased 50.0%');
	});

	it('emits nothing when the change is below the threshold', () => {
		const expenses = [
			...monthlySpend('Food', '2026-07', [100]),
			...monthlySpend('Food', '2026-08', [110])
		];
		const insights = generateInsights(expenses, { now }).filter(
			(i) => i.kind === 'category-change'
		);
		expect(insights).toEqual([]);
	});

	it('emits nothing when spend is below the materiality floor', () => {
		const expenses = [
			...monthlySpend('Snacks', '2026-07', [10]),
			...monthlySpend('Snacks', '2026-08', [40])
		];
		const insights = generateInsights(expenses, { now }).filter(
			(i) => i.kind === 'category-change'
		);
		expect(insights).toEqual([]);
	});

	it('emits nothing (and no Infinity/NaN) for a category with no last-month spend', () => {
		const expenses = monthlySpend('Travel', '2026-08', [800]);
		const insights = generateInsights(expenses, { now });
		expect(insights.filter((i) => i.kind === 'category-change')).toEqual([]);
		for (const insight of insights) {
			expect(insight.message).not.toMatch(/Infinity|NaN/);
		}
	});

	it('caps category change insights and keeps the biggest movers', () => {
		const expenses = [
			...monthlySpend('A', '2026-07', [100]),
			...monthlySpend('A', '2026-08', [400]),
			...monthlySpend('B', '2026-07', [100]),
			...monthlySpend('B', '2026-08', [300]),
			...monthlySpend('C', '2026-07', [100]),
			...monthlySpend('C', '2026-08', [200]),
			...monthlySpend('D', '2026-07', [100]),
			...monthlySpend('D', '2026-08', [150])
		];
		const insights = generateInsights(expenses, { now }).filter(
			(i) => i.kind === 'category-change'
		);
		expect(insights).toHaveLength(MAX_CATEGORY_CHANGE_INSIGHTS);
		expect(insights.map((i) => i.categoryName)).toEqual(['A', 'B']);
	});
});

describe('largest expense', () => {
	it('emits when one expense dominates the month', () => {
		const expenses = [
			makeExpense({ name: 'New phone', categoryName: 'Shopping', amount: 3000 }),
			...monthlySpend('Food', '2026-08', [20, 20, 20, 20])
		];
		const insights = generateInsights(expenses, { now }).filter(
			(i) => i.kind === 'largest-expense'
		);
		expect(insights).toHaveLength(1);
		expect(insights[0].categoryName).toBe('Shopping');
		expect(insights[0].message).toContain('New phone');
	});

	it('emits nothing when expenses are evenly sized', () => {
		const expenses = monthlySpend('Food', '2026-08', [50, 50, 50, 50, 50, 50]);
		const insights = generateInsights(expenses, { now }).filter(
			(i) => i.kind === 'largest-expense'
		);
		expect(insights).toEqual([]);
	});

	it('emits nothing with too few expenses', () => {
		const expenses = [makeExpense({ amount: 1000 }), makeExpense({ amount: 10 })];
		const insights = generateInsights(expenses, { now }).filter(
			(i) => i.kind === 'largest-expense'
		);
		expect(insights).toEqual([]);
	});
});

describe('weekend pattern', () => {
	const weekendDates = [
		'2026-08-01',
		'2026-08-02',
		'2026-08-08',
		'2026-08-09',
		'2026-07-18',
		'2026-07-19',
		'2026-07-25',
		'2026-07-26'
	];
	const weekdayDates = ['2026-08-03', '2026-08-04', '2026-08-05', '2026-08-06'];

	it('emits when weekend spending per day is meaningfully higher', () => {
		const expenses = [
			...weekendDates.map((d) => makeExpense({ amount: 100, actionedAt: `${d}T12:00:00` })),
			...weekdayDates.map((d) => makeExpense({ amount: 10, actionedAt: `${d}T12:00:00` }))
		];
		const insights = generateInsights(expenses, { now }).filter(
			(i) => i.kind === 'weekend-pattern'
		);
		expect(insights).toHaveLength(1);
		expect(insights[0].message).toContain('weekends');
	});

	it('emits nothing when per-day spending is balanced', () => {
		// 90 across 9 weekend days and 210 across 21 weekdays → both 10/day.
		const expenses = [
			...weekendDates
				.slice(0, 5)
				.map((d) => makeExpense({ amount: 18, actionedAt: `${d}T12:00:00` })),
			...weekdayDates.map((d) => makeExpense({ amount: 35, actionedAt: `${d}T12:00:00` })),
			makeExpense({ amount: 70, actionedAt: '2026-08-10T12:00:00' })
		];
		const insights = generateInsights(expenses, { now }).filter(
			(i) => i.kind === 'weekend-pattern'
		);
		expect(insights).toEqual([]);
	});

	it('emits nothing with too few expenses in the window', () => {
		const expenses = weekendDates
			.slice(0, 3)
			.map((d) => makeExpense({ amount: 100, actionedAt: `${d}T12:00:00` }));
		const insights = generateInsights(expenses, { now }).filter(
			(i) => i.kind === 'weekend-pattern'
		);
		expect(insights).toEqual([]);
	});

	it('falls back to createdAt when actionedAt is missing', () => {
		const expenses = [
			...weekendDates.map((d) =>
				makeExpense({
					amount: 100,
					actionedAt: undefined as unknown as string,
					createdAt: `${d}T12:00:00`
				})
			),
			...weekdayDates.map((d) => makeExpense({ amount: 10, actionedAt: `${d}T12:00:00` }))
		];
		const insights = generateInsights(expenses, { now }).filter(
			(i) => i.kind === 'weekend-pattern'
		);
		expect(insights).toHaveLength(1);
		expect(insights[0].message).toContain('weekends');
	});
});

describe('top categories', () => {
	it('lists the top 3 categories in descending order with shares', () => {
		const expenses = [
			...monthlySpend('Food', '2026-08', [400]),
			...monthlySpend('Transport', '2026-08', [300]),
			...monthlySpend('Fun', '2026-08', [200]),
			...monthlySpend('Misc', '2026-08', [100])
		];
		const insights = generateInsights(expenses, { now }).filter((i) => i.kind === 'top-categories');
		expect(insights).toHaveLength(1);
		expect(insights[0].message).toMatch(/Food.*\(40%\), Transport.*\(30%\), Fun.*\(20%\)/);
		expect(insights[0].message).not.toContain('Misc');
	});

	it('emits nothing with fewer than two categories', () => {
		const expenses = monthlySpend('Food', '2026-08', [400, 300]);
		const insights = generateInsights(expenses, { now }).filter((i) => i.kind === 'top-categories');
		expect(insights).toEqual([]);
	});
});
