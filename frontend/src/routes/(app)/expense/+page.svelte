<script lang="ts">
	import { ArrowBigLeft, ArrowBigRight, CalendarArrowDown, CalendarIcon } from '@lucide/svelte';
	import { onMount } from 'svelte';

	import { formatCurrency } from '$lib';
	import DataTable from '$lib/components/data-table-revamp.svelte';
	import { Button } from '$lib/components/ui/button';
	import * as ButtonGroup from '$lib/components/ui/button-group/index.js';
	import * as Card from '$lib/components/ui/card/index.js';
	import type { Expense } from '$lib/services/types';
	import { appState } from '$lib/states.svelte';
	import type { DailyTotal, MonthlyTotal } from '$lib/types';

	import CategoryCost from './charts/category-cost.svelte';
	import CategoryCount from './charts/category-count.svelte';
	import DailySpending from './charts/daily-spending.svelte';
	import TotalByMonth from './charts/total-month.svelte';
	import { columns } from './table/column';

	let showCurrentMonthOnly = $state(true);

	const timeRanges = [
		{ value: 'day', label: 'Day' },
		{ value: 'month', label: 'Month' },
		{ value: 'year', label: 'Year' },
		{ value: 'all', label: 'All' }
	];
	let timeRange = $state<'day' | 'month' | 'year' | 'all'>('day');
	const timeRangeLabel = $derived(
		timeRanges.find((tr) => tr.value === timeRange)?.label ?? 'Select a time range'
	);
	const isCurrentMonthExpense = (expense: Expense) => {
		const created = new Date(expense.createdAt);
		const today = new Date();
		return created.getFullYear() === today.getFullYear() && created.getMonth() === today.getMonth();
	};

	let filteredExpenses = $derived(
		showCurrentMonthOnly ? appState.expenses.filter(isCurrentMonthExpense) : appState.expenses
	);

	onMount(() => {
		appState.pageTitle = 'Expenses';
		appState.currentPage = 'expense';
	});

	function groupExpensesByMonth(expenses: Expense[]): MonthlyTotal[] {
		const monthMap = new Map<string, number>();

		for (const expense of expenses) {
			const date = new Date(expense.actionedAt || expense.createdAt);
			const month = date.toLocaleString('default', { month: 'long' }); // e.g. "January"

			monthMap.set(month, (monthMap.get(month) ?? 0) + expense.amount);
		}

		return Array.from(monthMap.entries())
			.reverse()
			.map(([month, total]) => ({
				month,
				total: Math.round(total * 100) / 100 // avoid floating point drift
			}));
	}

	function groupExpensesByDay(expenses: Expense[]): DailyTotal[] {
		const dayMap = new Map<Date, number>();

		const today = new Date();
		const dateInt = today.getDate();

		const mappedExpenses = expenses.map((expense) => ({
			effectiveDate: new Date(expense.actionedAt ?? expense.createdAt),
			total: expense.amount
		}));

		// last 7 days
		for (let i = dateInt; i > dateInt - 7; --i) {
			today.setDate(today.getDate() - 1);
			const currentMonth = today.getMonth();
			const currentDate = today.getDate();
			const totalAmount = mappedExpenses
				.filter((expense) => {
					return (
						expense.effectiveDate.getDate() == currentDate &&
						expense.effectiveDate.getMonth() == currentMonth
					);
				})
				.reduce((prev, curr) => prev + curr.total, 0);

			dayMap.set(new Date(today.getFullYear(), currentMonth, currentDate), totalAmount);
		}

		return Array.from(dayMap.entries())
			.reverse()
			.map(
				([day, total]): DailyTotal => ({
					day,
					total: Math.round(total * 100) / 100
				})
			);
	}

	const now = new Date();
	let averageSpendingPerDay = $derived(
		getAverageForMonth(appState.expenses, now.getFullYear(), now.getMonth() + 1)
	);
	let averageSpendingPerMonth = $derived(
		getMonthlyAverageForYear(appState.expenses, now.getFullYear())
	);
	let dailySpending = $derived(groupExpensesByDay(appState.expenses));

	function getAverageForMonth(expenses: Expense[], year: number, month: number): number {
		const filtered = expenses.filter((expense) => {
			const date = new Date(expense.actionedAt || expense.createdAt);
			return date.getFullYear() === year && date.getMonth() === month - 1; // month is 1-based
		});

		if (filtered.length === 0) return 0;

		const target = new Date(year, month - 1, 0);
		const total = filtered.reduce((sum, expense) => sum + expense.amount, 0);

		return Math.round((total / target.getDate()) * 100) / 100;
	}

	function getMonthlyAverageForYear(expenses: Expense[], year: number): number {
		const filtered = expenses.filter((expense) => {
			const date = new Date(expense.actionedAt || expense.createdAt);
			return date.getFullYear() === year;
		});

		if (filtered.length === 0) return 0;

		const total = filtered.reduce((sum, expense) => sum + expense.amount, 0);

		return Math.round((total / 12) * 100) / 100;
	}
	let monthlyExpense = $derived(groupExpensesByMonth(appState.expenses));

	const thisMonthFirstDay = new Date(new Date().getFullYear(), new Date().getMonth(), 1);
	const thisMonthLastDay = new Date(new Date().getFullYear(), new Date().getMonth() + 1, 0);
</script>

{#snippet dataTableControls()}
	<div class="flex items-center justify-between gap-2">
		<Button
			variant={showCurrentMonthOnly ? 'default' : 'outline'}
			size="sm"
			onclick={() => (showCurrentMonthOnly = true)}
		>
			Current month
		</Button>
		<Button
			variant={!showCurrentMonthOnly ? 'default' : 'outline'}
			size="sm"
			onclick={() => (showCurrentMonthOnly = false)}
		>
			Show older expenses
		</Button>
	</div>
{/snippet}

<div class="flex flex-col gap-4 py-4 md:gap-6 md:py-6">
	<div class="flex items-center justify-between px-4 lg:px-6">
		<div class="max-w-4xl space-y-2">
			<h1 class="text-xl font-bold">Expenses</h1>
			<p class="text-sm text-muted-foreground">
				Track your spending with a sortable expense table, category filters, and summary totals.
			</p>
		</div>

		<div class="flex items-center gap-2">
			<ButtonGroup.Root>
				<Button variant="outline" size="icon">
					<ArrowBigLeft />
				</Button>
				<Button variant="outline">
					<CalendarIcon />
					{thisMonthFirstDay.toLocaleDateString()} - {thisMonthLastDay.toLocaleDateString()}
				</Button>
				<Button variant="outline" size="icon">
					<ArrowBigRight />
				</Button>
			</ButtonGroup.Root>

			<ButtonGroup.Root>
				<Button
					size="icon"
					variant={showCurrentMonthOnly ? 'default' : 'outline'}
					onclick={() => (showCurrentMonthOnly = true)}
				>
					<CalendarArrowDown />
				</Button>
				<Button
					variant={!showCurrentMonthOnly ? 'default' : 'outline'}
					onclick={() => (showCurrentMonthOnly = false)}
				>
					<CalendarIcon />
					Show All
				</Button>
			</ButtonGroup.Root>
		</div>
	</div>

	<div class="flex gap-4 p-6">
		<CategoryCount
			chartData={appState.categories.map((category, index) => {
				const count = appState.expenses.filter(
					(expense) => expense.categoryName === category
				).length;
				const color = `var(--chart-${index + 1})`;
				return { category, count, color };
			})}
		/>
		<CategoryCost
			chartData={appState.categories.map((category, index) => {
				const cost = appState.expenses
					.filter((expense) => expense.categoryName === category)
					.reduce((prev, curr) => prev + curr.amount, 0);
				const color = `var(--chart-${index + 1})`;
				return { category, cost, color };
			})}
		/>
		<TotalByMonth chartData={monthlyExpense} />

		<DailySpending chartData={dailySpending} />
	</div>

	<div class="flex gap-4 p-6">
		<Card.Root class="w-[200px]">
			<Card.Header>
				<Card.Description>Average daily spendings</Card.Description>
				<Card.Title class="text-2xl font-semibold tabular-nums @[250px]/card:text-3xl">
					{formatCurrency(averageSpendingPerDay)}
				</Card.Title>
			</Card.Header>
		</Card.Root>

		<Card.Root class="w-[200px]">
			<Card.Header>
				<Card.Description>Average monthly spendings</Card.Description>
				<Card.Title class="text-2xl font-semibold tabular-nums @[250px]/card:text-3xl">
					{formatCurrency(averageSpendingPerMonth)}
				</Card.Title>
			</Card.Header>
		</Card.Root>

		<Card.Root class="w-[200px]">
			<Card.Header>
				<Card.Description>Total expenses recorded</Card.Description>
				<Card.Title class="text-2xl font-semibold tabular-nums @[250px]/card:text-3xl">
					{appState.expenses.length}
				</Card.Title>
			</Card.Header>
		</Card.Root>

		<Card.Root class="w-[200px]">
			<Card.Header>
				<Card.Description>Expenses logged this month</Card.Description>
				<Card.Title class="text-2xl font-semibold tabular-nums @[250px]/card:text-3xl">
					{appState.expenses.filter(isCurrentMonthExpense).length}
				</Card.Title>
			</Card.Header>
		</Card.Root>
	</div>

	<div class="px-4 lg:px-6">
		<h2 class="text-lg font-semibold">Recent Expenses</h2>
	</div>

	<div class="px-4 lg:px-6">
		<DataTable data={filteredExpenses} {columns} controls={dataTableControls} />
	</div>
</div>
