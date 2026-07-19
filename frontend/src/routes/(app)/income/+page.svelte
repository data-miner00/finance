<script lang="ts">
	import { onMount } from 'svelte';

	import DataTable from '$lib/components/data-table-revamp.svelte';
	import { appState } from '$lib/states.svelte';

	import MonthlyDistribution from './charts/monthly-distribution.svelte';
	import MonthlyNetIncome from './charts/monthly-net-income.svelte';
	import SavingsRate from './charts/savings-rate.svelte';
	import { columns } from './data-table/column';

	onMount(() => {
		appState.pageTitle = 'Incomes';
		appState.currentPage = 'income';
	});

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
</script>

{#snippet dataTableControls()}
	<div></div>
{/snippet}

<div class="flex gap-4 p-6">
	<MonthlyDistribution chartData={monthlyIncome} />
	<MonthlyNetIncome chartData={monthlyNetIncome} />
	<SavingsRate chartData={monthlySavingsRate} />
</div>

<div class="flex flex-col gap-4 py-4 md:gap-6 md:py-6">
	<div class="px-4 lg:px-6">
		<div class="max-w-4xl space-y-2">
			<h1 class="text-xl font-bold">Incomes</h1>
			<p class="text-sm text-muted-foreground">
				Track your income with a sortable income table, category filters, and summary totals.
			</p>
		</div>
	</div>

	<div class="px-4 lg:px-6">
		<DataTable data={appState.incomes} {columns} controls={dataTableControls} />
	</div>
</div>
