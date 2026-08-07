<script lang="ts">
	import { onMount } from 'svelte';

	import { calculatePercentageChange, formatCurrency } from '$lib';
	import DataTable from '$lib/components/data-table-revamp.svelte';
	import StatCard from '$lib/components/stat-card.svelte';
	import * as Tabs from '$lib/components/ui/tabs/index.js';
	import { appState } from '$lib/states.svelte';

	import CumulativeIncome from './charts/cumulative-income.svelte';
	import MonthlyDistribution from './charts/monthly-distribution.svelte';
	import MonthlyNetIncome from './charts/monthly-net-income.svelte';
	import SavingsRate from './charts/savings-rate.svelte';
	import { columns } from './data-table/column';

	onMount(() => {
		appState.pageTitle = 'Incomes';
		appState.currentPage = 'income';
	});

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

	function getTrendDescriptionPercentagePoints(
		pointChange: number,
		period: string
	): { direction: 'up' | 'down'; text: string } {
		if (pointChange > 0) {
			return { direction: 'up', text: `Up ${Math.abs(pointChange).toFixed(1)}pp ${period}` };
		} else if (pointChange < 0) {
			return { direction: 'down', text: `Down ${Math.abs(pointChange).toFixed(1)}pp ${period}` };
		}
		return { direction: 'up', text: `No change ${period}` };
	}

	type MonthlyBucket = { month: string; monthKey: string; total: number };

	function groupByMonth(items: { actionedAt: string; amount: number }[]): MonthlyBucket[] {
		const monthMap = new Map<string, number>();

		for (const item of items) {
			const date = new Date(item.actionedAt);
			const monthKey = `${date.getFullYear()}-${String(date.getMonth()).padStart(2, '0')}`;

			monthMap.set(monthKey, (monthMap.get(monthKey) ?? 0) + item.amount);
		}

		return Array.from(monthMap.entries())
			.sort(([a], [b]) => a.localeCompare(b))
			.map(([monthKey, total]) => {
				const [year, monthIndex] = monthKey.split('-').map(Number);
				const month = new Date(year, monthIndex, 1).toLocaleString('default', {
					month: 'long',
					year: 'numeric'
				});
				return { month, monthKey, total: Math.round(total * 100) / 100 };
			});
	}

	let monthlyIncome = $derived(groupByMonth(appState.incomes));
	let monthlyExpenses = $derived(groupByMonth(appState.expenses));

	// TODO: Last 6 month
	let monthlyNetIncome = $derived.by(() => {
		const monthKeys = new Set([
			...monthlyIncome.map((m) => m.monthKey),
			...monthlyExpenses.map((m) => m.monthKey)
		]);

		return Array.from(monthKeys)
			.sort()
			.map((monthKey) => {
				const income = monthlyIncome.find((m) => m.monthKey === monthKey);
				const expense = monthlyExpenses.find((m) => m.monthKey === monthKey);
				const month = (income ?? expense)!.month;
				const netIncome = (income?.total ?? 0) - (expense?.total ?? 0);
				return { month, netIncome };
			});
	});

	// TODO: Last 6 month
	let monthlySavingsRate = $derived.by(() =>
		monthlyIncome
			.filter((income) => income.total > 0)
			.map((income) => {
				const expense = monthlyExpenses.find((e) => e.monthKey === income.monthKey);
				const savingsRate = ((income.total - (expense?.total ?? 0)) / income.total) * 100;
				return { month: income.month, savingsRate: Math.max(0, Math.round(savingsRate * 10) / 10) };
			})
	);

	function monthKeyOf(date: Date): string {
		return `${date.getFullYear()}-${String(date.getMonth()).padStart(2, '0')}`;
	}

	function getMonthTotal(buckets: MonthlyBucket[], monthKey: string): number {
		return buckets.find((bucket) => bucket.monthKey === monthKey)?.total ?? 0;
	}

	function getMonthlyAverageForYear(items: { actionedAt: string; amount: number }[], year: number) {
		const filtered = items.filter((item) => new Date(item.actionedAt).getFullYear() === year);
		if (filtered.length === 0) return 0;

		const total = filtered.reduce((sum, item) => sum + item.amount, 0);
		return Math.round((total / 12) * 100) / 100;
	}

	const now = new Date();
	const currentMonthKey = monthKeyOf(now);
	const lastMonthKey = monthKeyOf(new Date(now.getFullYear(), now.getMonth() - 1, 1));

	// Average monthly income, this year vs last year
	let averageIncomePerMonth = $derived(
		getMonthlyAverageForYear(appState.incomes, now.getFullYear())
	);
	let averageIncomePerMonthLastYear = $derived(
		getMonthlyAverageForYear(appState.incomes, now.getFullYear() - 1)
	);

	// Total income, this month vs last month
	let totalIncomeThisMonth = $derived(getMonthTotal(monthlyIncome, currentMonthKey));
	let totalIncomeLastMonth = $derived(getMonthTotal(monthlyIncome, lastMonthKey));

	let totalExpenseThisMonth = $derived(getMonthTotal(monthlyExpenses, currentMonthKey));
	let totalExpenseLastMonth = $derived(getMonthTotal(monthlyExpenses, lastMonthKey));

	// Net income, this month vs last month
	let netIncomeThisMonth = $derived(totalIncomeThisMonth - totalExpenseThisMonth);
	let netIncomeLastMonth = $derived(totalIncomeLastMonth - totalExpenseLastMonth);

	// Savings rate, this month vs last month
	let savingsRateThisMonth = $derived(
		totalIncomeThisMonth > 0
			? Math.round(((totalIncomeThisMonth - totalExpenseThisMonth) / totalIncomeThisMonth) * 1000) /
					10
			: 0
	);
	let savingsRateLastMonth = $derived(
		totalIncomeLastMonth > 0
			? Math.round(((totalIncomeLastMonth - totalExpenseLastMonth) / totalIncomeLastMonth) * 1000) /
					10
			: 0
	);

	// Cumulative income for the current year (YTD running total)
	let cumulativeIncome = $derived.by(() => {
		const currentYearPrefix = `${now.getFullYear()}-`;
		let running = 0;
		return monthlyIncome
			.filter((bucket) => bucket.monthKey.startsWith(currentYearPrefix))
			.map((bucket) => {
				running += bucket.total;
				return { month: bucket.month, cumulative: Math.round(running * 100) / 100 };
			});
	});
</script>

{#snippet dataTableControls()}
	<div></div>
{/snippet}

<div class="flex flex-col gap-4 py-4 md:gap-6 md:py-6">
	<div class="px-4 lg:px-6">
		<div class="max-w-4xl space-y-2">
			<h1 class="text-xl font-bold">Incomes</h1>
			<p class="text-sm text-muted-foreground">
				Track your income with a sortable income table, category filters, and summary totals.
			</p>
		</div>
	</div>

	<div
		class="grid grid-cols-2 gap-4 px-4 *:data-[slot=card]:bg-gradient-to-t *:data-[slot=card]:from-primary/5 *:data-[slot=card]:to-card *:data-[slot=card]:shadow-xs lg:grid-cols-4 lg:px-6 dark:*:data-[slot=card]:bg-card"
	>
		<StatCard
			description="Average monthly income"
			value={formatCurrency(averageIncomePerMonth)}
			trendDirection={getTrendDescription(
				calculatePercentageChange(averageIncomePerMonth, averageIncomePerMonthLastYear),
				'this year'
			).direction}
			badgeText="{calculatePercentageChange(averageIncomePerMonth, averageIncomePerMonthLastYear) >
			0
				? '+'
				: ''}{calculatePercentageChange(
				averageIncomePerMonth,
				averageIncomePerMonthLastYear
			).toFixed(1)}%"
			footerText={getTrendDescription(
				calculatePercentageChange(averageIncomePerMonth, averageIncomePerMonthLastYear),
				'this year'
			).text}
			footerSubText="vs {formatCurrency(averageIncomePerMonthLastYear)} last year"
		/>

		<StatCard
			description="Total income this month"
			value={formatCurrency(totalIncomeThisMonth)}
			trendDirection={getTrendDescription(
				calculatePercentageChange(totalIncomeThisMonth, totalIncomeLastMonth),
				'this month'
			).direction}
			badgeText="{calculatePercentageChange(totalIncomeThisMonth, totalIncomeLastMonth) > 0
				? '+'
				: ''}{calculatePercentageChange(totalIncomeThisMonth, totalIncomeLastMonth).toFixed(1)}%"
			footerText={getTrendDescription(
				calculatePercentageChange(totalIncomeThisMonth, totalIncomeLastMonth),
				'this month'
			).text}
			footerSubText="vs {formatCurrency(totalIncomeLastMonth)} last month"
		/>

		<StatCard
			description="Net income this month"
			value={formatCurrency(netIncomeThisMonth)}
			trendDirection={getTrendDescription(
				calculatePercentageChange(netIncomeThisMonth, netIncomeLastMonth),
				'this month'
			).direction}
			badgeText="{calculatePercentageChange(netIncomeThisMonth, netIncomeLastMonth) > 0
				? '+'
				: ''}{calculatePercentageChange(netIncomeThisMonth, netIncomeLastMonth).toFixed(1)}%"
			footerText={getTrendDescription(
				calculatePercentageChange(netIncomeThisMonth, netIncomeLastMonth),
				'this month'
			).text}
			footerSubText="vs {formatCurrency(netIncomeLastMonth)} last month"
		/>

		<StatCard
			description="Savings rate this month"
			value="{savingsRateThisMonth.toFixed(1)}%"
			trendDirection={getTrendDescriptionPercentagePoints(
				savingsRateThisMonth - savingsRateLastMonth,
				'this month'
			).direction}
			badgeText="{savingsRateThisMonth - savingsRateLastMonth > 0 ? '+' : ''}{(
				savingsRateThisMonth - savingsRateLastMonth
			).toFixed(1)}pp"
			footerText={getTrendDescriptionPercentagePoints(
				savingsRateThisMonth - savingsRateLastMonth,
				'this month'
			).text}
			footerSubText="vs {savingsRateLastMonth.toFixed(1)}% last month"
		/>
	</div>

	<Tabs.Root value="records" class="gap-4">
		<div class="px-4 lg:px-6">
			<Tabs.List>
				<Tabs.Trigger value="records">Records</Tabs.Trigger>
				<Tabs.Trigger value="visualizations">Visualizations</Tabs.Trigger>
			</Tabs.List>
		</div>

		<Tabs.Content value="records" class="flex flex-col gap-4">
			<div class="px-4 lg:px-6">
				<DataTable data={appState.incomes} {columns} controls={dataTableControls} />
			</div>
		</Tabs.Content>

		<Tabs.Content value="visualizations">
			<div class="grid grid-cols-1 gap-4 px-4 lg:grid-cols-2 lg:px-6">
				<MonthlyDistribution chartData={monthlyIncome} />
				<MonthlyNetIncome chartData={monthlyNetIncome} />
				<SavingsRate chartData={monthlySavingsRate} />
				<CumulativeIncome chartData={cumulativeIncome} />
			</div>
		</Tabs.Content>
	</Tabs.Root>
</div>
