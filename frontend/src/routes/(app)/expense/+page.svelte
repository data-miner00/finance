<script lang="ts">
	import { type DateValue, getLocalTimeZone, today } from '@internationalized/date';
	import { ArrowBigLeft, ArrowBigRight, ChevronDownIcon, RefreshCcw, X } from '@lucide/svelte';
	import { type DateRange } from 'bits-ui';
	import { formatDateRange } from 'little-date';
	import { onMount } from 'svelte';
	import { toast } from 'svelte-sonner';

	import { calculatePercentageChange, formatCurrency, isCurrentMonth, isLastMonth } from '$lib';
	import DataTable from '$lib/components/data-table-revamp.svelte';
	import StatCard from '$lib/components/stat-card.svelte';
	import { Button } from '$lib/components/ui/button';
	import * as ButtonGroup from '$lib/components/ui/button-group/index.js';
	import * as Popover from '$lib/components/ui/popover/index.js';
	import { RangeCalendar } from '$lib/components/ui/range-calendar/index.js';
	import * as Tabs from '$lib/components/ui/tabs/index.js';
	import type { Expense } from '$lib/services/types';
	import { appState } from '$lib/states.svelte';
	import type { DailyTotal, MonthlyTotal } from '$lib/types';

	import CategoryCost from './charts/category-cost.svelte';
	import CategoryCount from './charts/category-count.svelte';
	import DailySpending from './charts/daily-spending.svelte';
	import TotalByMonth from './charts/total-month.svelte';
	import { columns } from './data-table/column';

	const id = $props.id();

	function getTrendDescription(
		percentChange: number,
		period: string
	): { direction: 'up' | 'down'; text: string } {
		if (percentChange > 0) {
			return { direction: 'up', text: `Up ${Math.abs(percentChange).toFixed(1)}% ${period}` };
		} else if (percentChange < 0) {
			return { direction: 'down', text: `Down ${Math.abs(percentChange).toFixed(1)}% ${period}` };
		}
		return { direction: 'up', text: `No change ${period}` };
	}

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
	const lastMonthDate = new Date(now.getFullYear(), now.getMonth() - 1, 1);
	let averageSpendingPerDay = $derived(
		getAverageForMonth(appState.expenses, now.getFullYear(), now.getMonth() + 1)
	);
	let averageSpendingPerDayLastMonth = $derived(
		getAverageForMonth(appState.expenses, lastMonthDate.getFullYear(), lastMonthDate.getMonth() + 1)
	);
	let averageSpendingPerMonth = $derived(
		getMonthlyAverageForYear(appState.expenses, now.getFullYear())
	);
	let averageSpendingPerMonthLastYear = $derived(
		getMonthlyAverageForYear(appState.expenses, now.getFullYear() - 1)
	);
	let dailySpending = $derived(groupExpensesByDay(appState.expenses));

	// Derived: Total expense for this month
	let totalExpenseThisMonth = $derived(
		appState.expenses
			.filter((expense) => isCurrentMonth(expense.actionedAt ?? expense.createdAt))
			.reduce((sum, expense) => sum + expense.amount, 0)
	);

	// Derived: Total expense for last month
	let totalExpenseLastMonth = $derived(
		appState.expenses
			.filter((expense) => isLastMonth(expense.actionedAt ?? expense.createdAt))
			.reduce((sum, expense) => sum + expense.amount, 0)
	);
	let expenseChangePercent = $derived(
		calculatePercentageChange(totalExpenseThisMonth, totalExpenseLastMonth)
	);
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

	let value = $state<DateRange>({
		start: today(getLocalTimeZone()).set({ day: 0 }),
		end: today(getLocalTimeZone())
	});

	function resetToThisMonth() {
		value.start = today(getLocalTimeZone()).set({ day: 0 });
		value.end = today(getLocalTimeZone());

		toast.success('Reset date range to this month.');
	}

	function setPreviousMonth() {
		if (!value.start || !value.end) {
			return resetToThisMonth();
		}

		value.start = value.start.set({ day: 0 }).subtract({ months: 1 });
		value.end = value.end.set({ day: 0 }).subtract({ days: 1 });

		toast.success('Set date range to previous month.');
	}

	function setNextMonth() {
		if (!value.start || !value.end) {
			return resetToThisMonth();
		}

		value.start = value.start.set({ day: 0 }).add({ months: 1 });
		value.end = value.end.set({ day: 0 }).add({ months: 2 }).subtract({ days: 1 });

		toast.success('Set date range to previous month.');
	}

	let filteredExpensesByDateRange = $derived(
		appState.expenses.filter((x) => {
			var start = value.start?.toDate(getLocalTimeZone());
			var end = value.end?.toDate(getLocalTimeZone());

			// When interactively selecting the date range from the UI, the `end` will be temporarily undefined when the selection is pending
			if (!start || !end) return true;

			var actionedAt = new Date(x.actionedAt);
			return start <= actionedAt && actionedAt <= end;
		})
	);

	function formatRange(start: DateValue, end: DateValue) {
		return formatDateRange(start.toDate(getLocalTimeZone()), end.toDate(getLocalTimeZone()), {
			includeTime: false
		});
	}

	let categoryCountRaw = $derived(
		appState.categories.map((category) => {
			const count = filteredExpensesByDateRange.filter(
				(expense) => expense.categoryName === category
			).length;
			return { category, count };
		})
	);

	let categoryCountData = $derived.by(() => {
		const categoryData = categoryCountRaw.filter((x) => x.count != 0);

		categoryData.sort((a, b) => b.count - a.count);

		const topCategories = categoryData.slice(0, appState.settings.pieChartDisplayTop);
		const otherCount = categoryData
			.slice(appState.settings.pieChartDisplayTop)
			.reduce((acc, curr) => acc + curr.count, 0);
		return [
			...topCategories.map((cat, index) => ({ ...cat, color: `var(--chart-${index + 1})` })),
			...(otherCount ? [{ category: 'Other', count: otherCount, color: 'var(--color-other)' }] : [])
		];
	});

	let categoryCostData = $derived.by(() => {
		const categoryData = appState.categories.map((category) => {
			const cost = filteredExpensesByDateRange
				.filter((expense) => expense.categoryName === category)
				.reduce((prev, curr) => prev + curr.amount, 0);
			return { category, cost };
		});

		categoryData.sort((a, b) => b.cost - a.cost);

		const topCategories = categoryData
			.filter((x) => x.cost != 0)
			.slice(0, appState.settings.pieChartDisplayTop);
		const otherCount = categoryData
			.slice(appState.settings.pieChartDisplayTop)
			.reduce((acc, curr) => acc + curr.cost, 0);
		return [
			...topCategories.map((cat, index) => ({ ...cat, color: `var(--chart-${index + 1})` })),
			...(otherCount ? [{ category: 'Other', cost: otherCount, color: 'var(--color-other)' }] : [])
		];
	});
</script>

<div class="flex flex-col gap-4 py-4 md:gap-6 md:py-6">
	<div class="px-4 lg:px-6">
		<div class="max-w-4xl space-y-2">
			<h1 class="text-xl font-bold">Expenses</h1>
			<p class="text-sm text-muted-foreground">
				Track your spending with a sortable expense table, category filters, and summary totals.
			</p>
		</div>
	</div>

	<div
		class="grid grid-cols-2 gap-4 px-4 *:data-[slot=card]:bg-linear-to-t *:data-[slot=card]:from-primary/5 *:data-[slot=card]:to-card *:data-[slot=card]:shadow-xs lg:grid-cols-4 lg:px-6 dark:*:data-[slot=card]:bg-card"
	>
		<StatCard
			description="Spending"
			value={formatCurrency(totalExpenseThisMonth)}
			trendDirection={getTrendDescription(expenseChangePercent, 'this month').direction}
			badgeText="{expenseChangePercent > 0 ? '+' : ''}{expenseChangePercent.toFixed(1)}%"
			footerText={getTrendDescription(expenseChangePercent, 'last month').text}
			footerSubText="vs {formatCurrency(totalExpenseLastMonth)} last month"
		/>

		<StatCard
			description="Average daily spendings"
			value={formatCurrency(averageSpendingPerDay)}
			trendDirection={getTrendDescription(
				calculatePercentageChange(averageSpendingPerDay, averageSpendingPerDayLastMonth),
				'this month'
			).direction}
			badgeText="{calculatePercentageChange(averageSpendingPerDay, averageSpendingPerDayLastMonth) >
			0
				? '+'
				: ''}{calculatePercentageChange(
				averageSpendingPerDay,
				averageSpendingPerDayLastMonth
			).toFixed(1)}%"
			footerText={getTrendDescription(
				calculatePercentageChange(averageSpendingPerDay, averageSpendingPerDayLastMonth),
				'this month'
			).text}
			footerSubText="vs {formatCurrency(averageSpendingPerDayLastMonth)} last month"
		/>

		<StatCard
			description="Average monthly spendings"
			value={formatCurrency(averageSpendingPerMonth)}
			trendDirection={getTrendDescription(
				calculatePercentageChange(averageSpendingPerMonth, averageSpendingPerMonthLastYear),
				'this year'
			).direction}
			badgeText="{calculatePercentageChange(
				averageSpendingPerMonth,
				averageSpendingPerMonthLastYear
			) > 0
				? '+'
				: ''}{calculatePercentageChange(
				averageSpendingPerMonth,
				averageSpendingPerMonthLastYear
			).toFixed(1)}%"
			footerText={getTrendDescription(
				calculatePercentageChange(averageSpendingPerMonth, averageSpendingPerMonthLastYear),
				'this year'
			).text}
			footerSubText="vs {formatCurrency(averageSpendingPerMonthLastYear)} last year"
		/>

		<StatCard
			description="Total expenses recorded"
			value={appState.expenses.length.toString()}
			trendDirection="up"
			badgeText="All-time"
			footerText="Accumulated over time"
			footerSubText="Across all recorded expenses"
		/>

		<StatCard
			description="Expenses logged this month"
			value={appState.expenses
				.filter((expense) => isCurrentMonth(expense.actionedAt))
				.length.toString()}
			trendDirection={getTrendDescription(
				calculatePercentageChange(
					appState.expenses.filter((expense) => isCurrentMonth(expense.actionedAt)).length,
					appState.expenses.filter((expense) => isLastMonth(expense.actionedAt)).length
				),
				'this month'
			).direction}
			badgeText="{calculatePercentageChange(
				appState.expenses.filter((expense) => isCurrentMonth(expense.actionedAt)).length,
				appState.expenses.filter((expense) => isLastMonth(expense.actionedAt)).length
			) > 0
				? '+'
				: ''}{calculatePercentageChange(
				appState.expenses.filter((expense) => isCurrentMonth(expense.actionedAt)).length,
				appState.expenses.filter((expense) => isLastMonth(expense.actionedAt)).length
			).toFixed(1)}%"
			footerText={getTrendDescription(
				calculatePercentageChange(
					appState.expenses.filter((expense) => isCurrentMonth(expense.actionedAt)).length,
					appState.expenses.filter((expense) => isLastMonth(expense.actionedAt)).length
				),
				'this month'
			).text}
			footerSubText="vs {appState.expenses.filter((expense) => isLastMonth(expense.actionedAt))
				.length} logged last month"
		/>
	</div>

	<Tabs.Root value="records" class="gap-4">
		<div class="flex items-center justify-between px-4 lg:px-6">
			<Tabs.List>
				<Tabs.Trigger value="records">Records</Tabs.Trigger>
				<Tabs.Trigger value="visualizations">Visualizations</Tabs.Trigger>
			</Tabs.List>
			<div class="flex items-center gap-2">
				<ButtonGroup.Root>
					<Button variant="outline" size="icon" onclick={setPreviousMonth}>
						<ArrowBigLeft />
					</Button>
					<Popover.Root>
						<Popover.Trigger id="{id}-dates">
							{#snippet child({ props })}
								<Button {...props} variant="outline" class="w-56 justify-between font-normal">
									{#if value?.start && value?.end}
										{formatRange(value.start, value.end)}
									{:else}
										Select date
									{/if}
									<ChevronDownIcon />
								</Button>
							{/snippet}
						</Popover.Trigger>
						<Popover.Content class="w-auto overflow-hidden p-0" align="start">
							<RangeCalendar bind:value captionLayout="dropdown" />
						</Popover.Content>
					</Popover.Root>
					<Button variant="outline" size="icon" onclick={setNextMonth}>
						<ArrowBigRight />
					</Button>
					<Button variant="outline" size="icon" onclick={resetToThisMonth}>
						<RefreshCcw />
					</Button>
				</ButtonGroup.Root>
			</div>
		</div>

		<Tabs.Content value="records" class="flex flex-col gap-4">
			<div class="px-4 lg:px-6">
				<DataTable data={filteredExpensesByDateRange} {columns} />
			</div>
		</Tabs.Content>

		<Tabs.Content value="visualizations">
			<div class="grid grid-cols-1 gap-4 px-4 lg:grid-cols-2 lg:px-6">
				<CategoryCount chartData={categoryCountData} />
				<CategoryCost chartData={categoryCostData} />
				<TotalByMonth chartData={monthlyExpense} />
				<DailySpending chartData={dailySpending} />
			</div>
		</Tabs.Content>
	</Tabs.Root>
</div>
