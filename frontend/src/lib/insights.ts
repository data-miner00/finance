import { calculatePercentageChange, formatCurrency } from '$lib';
import type { Expense } from '$lib/services/types';

export type InsightTone = 'positive' | 'negative' | 'neutral';

export interface Insight {
	kind: 'category-change' | 'largest-expense' | 'weekend-pattern' | 'top-categories';
	tone: InsightTone;
	title: string;
	message: string;
	categoryName?: string;
}

export interface InsightOptions {
	now?: Date;
	currency?: string;
}

export const MIN_CATEGORY_SPEND = 50;
export const MIN_CHANGE_PERCENT = 20;
export const MAX_CATEGORY_CHANGE_INSIGHTS = 2;
export const LARGEST_EXPENSE_MIN_COUNT = 5;
export const LARGEST_EXPENSE_MIN_SHARE = 0.25;
export const WEEKEND_WINDOW_DAYS = 30;
export const WEEKEND_MIN_EXPENSES = 10;
export const WEEKEND_MIN_RATIO = 1.4;
export const TOP_CATEGORIES_COUNT = 3;

// The $lib isCurrentMonth/isLastMonth helpers hardcode new Date(); these take an
// injected `now` so insights are testable with a fixed date.
function expenseDate(expense: Expense): Date {
	return new Date(expense.actionedAt ?? expense.createdAt);
}

function isSameMonth(date: Date, other: Date): boolean {
	return date.getMonth() === other.getMonth() && date.getFullYear() === other.getFullYear();
}

function isWeekend(date: Date): boolean {
	return date.getDay() === 0 || date.getDay() === 6;
}

function sumByCategory(expenses: Expense[]): Map<string, number> {
	const totals = new Map<string, number>();
	for (const expense of expenses) {
		if (!expense.categoryName) continue;
		totals.set(expense.categoryName, (totals.get(expense.categoryName) ?? 0) + expense.amount);
	}
	return totals;
}

function categorySpendChangeInsights(expenses: Expense[], now: Date, currency: string): Insight[] {
	const lastMonth = new Date(now.getFullYear(), now.getMonth() - 1);
	const currentTotals = sumByCategory(expenses.filter((e) => isSameMonth(expenseDate(e), now)));
	const previousTotals = sumByCategory(
		expenses.filter((e) => isSameMonth(expenseDate(e), lastMonth))
	);

	const candidates: { categoryName: string; current: number; previous: number; change: number }[] =
		[];
	for (const [categoryName, current] of currentTotals) {
		const previous = previousTotals.get(categoryName) ?? 0;
		if (current < MIN_CATEGORY_SPEND || previous < MIN_CATEGORY_SPEND) continue;

		const change = calculatePercentageChange(current, previous);
		if (Math.abs(change) < MIN_CHANGE_PERCENT) continue;

		candidates.push({ categoryName, current, previous, change });
	}

	return candidates
		.sort((a, b) => Math.abs(b.change) - Math.abs(a.change))
		.slice(0, MAX_CATEGORY_CHANGE_INSIGHTS)
		.map(({ categoryName, current, previous, change }) => {
			const increased = change > 0;
			return {
				kind: 'category-change',
				tone: increased ? 'negative' : 'positive',
				title: `${categoryName} spending ${increased ? 'up' : 'down'}`,
				message: `Your ${categoryName} spending ${increased ? 'increased' : 'decreased'} ${Math.abs(change).toFixed(1)}% vs last month (${formatCurrency(previous, currency)} → ${formatCurrency(current, currency)}).`,
				categoryName
			};
		});
}

function largestExpenseInsight(expenses: Expense[], now: Date, currency: string): Insight[] {
	const monthly = expenses.filter((e) => isSameMonth(expenseDate(e), now));
	if (monthly.length < LARGEST_EXPENSE_MIN_COUNT) return [];

	const total = monthly.reduce((sum, e) => sum + e.amount, 0);
	if (total <= 0) return [];

	const largest = monthly.reduce((max, e) => (e.amount > max.amount ? e : max));
	const share = largest.amount / total;
	if (share < LARGEST_EXPENSE_MIN_SHARE) return [];

	return [
		{
			kind: 'largest-expense',
			tone: 'neutral',
			title: 'Largest expense',
			message: `Your largest expense this month was "${largest.name}" at ${formatCurrency(largest.amount, currency)} — ${(share * 100).toFixed(0)}% of your total spend.`,
			categoryName: largest.categoryName
		}
	];
}

function weekendSpendingInsight(expenses: Expense[], now: Date, currency: string): Insight[] {
	const windowStart = new Date(now);
	windowStart.setDate(windowStart.getDate() - WEEKEND_WINDOW_DAYS);

	const windowed = expenses.filter((e) => {
		const date = expenseDate(e);
		return date >= windowStart && date <= now;
	});
	if (windowed.length < WEEKEND_MIN_EXPENSES) return [];

	let weekendDays = 0;
	let weekdayDays = 0;
	for (let offset = 0; offset < WEEKEND_WINDOW_DAYS; offset++) {
		const day = new Date(now);
		day.setDate(day.getDate() - offset);
		if (isWeekend(day)) weekendDays++;
		else weekdayDays++;
	}

	const weekendTotal = windowed
		.filter((e) => isWeekend(expenseDate(e)))
		.reduce((sum, e) => sum + e.amount, 0);
	const weekdayTotal = windowed
		.filter((e) => !isWeekend(expenseDate(e)))
		.reduce((sum, e) => sum + e.amount, 0);

	const weekendAverage = weekendTotal / weekendDays;
	const weekdayAverage = weekdayTotal / weekdayDays;
	if (weekendAverage <= 0 || weekdayAverage <= 0) return [];

	const higher = Math.max(weekendAverage, weekdayAverage);
	const lower = Math.min(weekendAverage, weekdayAverage);
	const ratio = higher / lower;
	if (ratio < WEEKEND_MIN_RATIO) return [];

	const label = weekendAverage > weekdayAverage ? 'weekends' : 'weekdays';
	return [
		{
			kind: 'weekend-pattern',
			tone: 'neutral',
			title: `You spend more on ${label}`,
			message: `You spend ${ratio.toFixed(1)}x more per day on ${label} (${formatCurrency(higher, currency)}/day vs ${formatCurrency(lower, currency)}/day over the last ${WEEKEND_WINDOW_DAYS} days).`
		}
	];
}

function topCategoriesInsight(expenses: Expense[], now: Date, currency: string): Insight[] {
	const totals = sumByCategory(expenses.filter((e) => isSameMonth(expenseDate(e), now)));
	if (totals.size < 2) return [];

	const total = [...totals.values()].reduce((sum, amount) => sum + amount, 0);
	if (total <= 0) return [];

	const top = [...totals.entries()]
		.sort((a, b) => b[1] - a[1])
		.slice(0, TOP_CATEGORIES_COUNT)
		.map(
			([categoryName, amount]) =>
				`${categoryName} ${formatCurrency(amount, currency)} (${((amount / total) * 100).toFixed(0)}%)`
		);

	return [
		{
			kind: 'top-categories',
			tone: 'neutral',
			title: `Top ${top.length} categories`,
			message: `Top categories this month: ${top.join(', ')}.`
		}
	];
}

export function generateInsights(expenses: Expense[], options: InsightOptions = {}): Insight[] {
	const now = options.now ?? new Date();
	const currency = options.currency ?? 'MYR';

	return [
		...categorySpendChangeInsights(expenses, now, currency),
		...largestExpenseInsight(expenses, now, currency),
		...weekendSpendingInsight(expenses, now, currency),
		...topCategoriesInsight(expenses, now, currency)
	];
}
