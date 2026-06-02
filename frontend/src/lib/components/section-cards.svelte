<script lang="ts">
	import TrendingDownIcon from '@tabler/icons-svelte/icons/trending-down';
	import TrendingUpIcon from '@tabler/icons-svelte/icons/trending-up';
	import { Badge } from '$lib/components/ui/badge/index.js';
	import * as Card from '$lib/components/ui/card/index.js';
	import { appState } from '$lib/states.svelte';

	// Helper function to get current month and year
	function isCurrentMonth(dateString: string): boolean {
		const date = new Date(dateString);
		const now = new Date();
		return date.getMonth() === now.getMonth() && date.getFullYear() === now.getFullYear();
	}

	// Helper function to get last month
	function isLastMonth(dateString: string): boolean {
		const date = new Date(dateString);
		const now = new Date();
		const lastMonth = new Date(now.getFullYear(), now.getMonth() - 1);
		return (
			date.getMonth() === lastMonth.getMonth() && date.getFullYear() === lastMonth.getFullYear()
		);
	}

	// Helper function to calculate percentage change
	function calculatePercentageChange(current: number, previous: number): number {
		if (previous === 0) return 0;
		return ((current - previous) / previous) * 100;
	}

	// Derived: Total income for this month
	let totalIncomeThisMonth = $derived(
		appState.incomes
			.filter((income) => isCurrentMonth(income.createdAt))
			.reduce((sum, income) => sum + income.amount, 0)
	);

	// Derived: Total income for last month
	let totalIncomeLastMonth = $derived(
		appState.incomes
			.filter((income) => isLastMonth(income.createdAt))
			.reduce((sum, income) => sum + income.amount, 0)
	);

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

	// Derived: Total accumulated savings (sum of all account balances)
	let totalAccumulatedSavings = $derived(
		appState.accounts.reduce((sum, account) => sum + account.balance, 0)
	);

	// Derived: Total piggy bank amounts
	let totalPiggyBankAmount = $derived(appState.piggyBanks.reduce((sum, pb) => sum + pb.amount, 0));

	// Derived: Left to spend (income - expense - piggy bank)
	let leftToSpend = $derived(totalIncomeThisMonth - totalExpenseThisMonth);

	// Derived: Left to spend last month
	let leftToSpendLastMonth = $derived(totalIncomeLastMonth - totalExpenseLastMonth);

	// Derived: Calculate percentage changes
	let incomeChangePercent = $derived(
		calculatePercentageChange(totalIncomeThisMonth, totalIncomeLastMonth)
	);
	let expenseChangePercent = $derived(
		calculatePercentageChange(totalExpenseThisMonth, totalExpenseLastMonth)
	);
	let leftToSpendChangePercent = $derived(
		calculatePercentageChange(leftToSpend, leftToSpendLastMonth)
	);

	// Helper function to get trend description
	function getTrendDescription(percentChange: number): { icon: 'up' | 'down'; text: string } {
		if (percentChange > 0) {
			return { icon: 'up', text: `Up ${Math.abs(percentChange).toFixed(1)}% this month` };
		} else if (percentChange < 0) {
			return { icon: 'down', text: `Down ${Math.abs(percentChange).toFixed(1)}% this month` };
		}
		return { icon: 'up', text: 'No change this month' };
	}

	// Helper function to format currency
	function formatCurrency(amount: number): string {
		return new Intl.NumberFormat('en-MY', {
			style: 'currency',
			currency: 'MYR'
		}).format(amount);
	}
</script>

<div
	class="grid grid-cols-1 gap-4 px-4 *:data-[slot=card]:bg-gradient-to-t *:data-[slot=card]:from-primary/5 *:data-[slot=card]:to-card *:data-[slot=card]:shadow-xs lg:px-6 @xl/main:grid-cols-2 @5xl/main:grid-cols-4 dark:*:data-[slot=card]:bg-card"
>
	<Card.Root class="@container/card">
		<Card.Header>
			<Card.Description>Income</Card.Description>
			<Card.Title class="text-2xl font-semibold tabular-nums @[250px]/card:text-3xl">
				{formatCurrency(totalIncomeThisMonth)}
			</Card.Title>
			<Card.Action>
				<Badge variant="outline">
					{#if getTrendDescription(incomeChangePercent).icon === 'up'}
						<TrendingUpIcon />
					{:else}
						<TrendingDownIcon />
					{/if}
					{incomeChangePercent > 0 ? '+' : ''}{incomeChangePercent.toFixed(1)}%
				</Badge>
			</Card.Action>
		</Card.Header>
		<Card.Footer class="flex-col items-start gap-1.5 text-sm">
			<div class="line-clamp-1 flex gap-2 font-medium">
				{getTrendDescription(incomeChangePercent).text}
				{#if getTrendDescription(incomeChangePercent).icon === 'up'}
					<TrendingUpIcon class="size-4" />
				{:else}
					<TrendingDownIcon class="size-4" />
				{/if}
			</div>
			<div class="text-muted-foreground">vs {formatCurrency(totalIncomeLastMonth)} last month</div>
		</Card.Footer>
	</Card.Root>
	<Card.Root class="@container/card">
		<Card.Header>
			<Card.Description>Spending</Card.Description>
			<Card.Title class="text-2xl font-semibold tabular-nums @[250px]/card:text-3xl">
				{formatCurrency(totalExpenseThisMonth)}
			</Card.Title>
			<Card.Action>
				<Badge variant="outline">
					{#if getTrendDescription(expenseChangePercent).icon === 'up'}
						<TrendingUpIcon />
					{:else}
						<TrendingDownIcon />
					{/if}
					{expenseChangePercent > 0 ? '+' : ''}{expenseChangePercent.toFixed(1)}%
				</Badge>
			</Card.Action>
		</Card.Header>
		<Card.Footer class="flex-col items-start gap-1.5 text-sm">
			<div class="line-clamp-1 flex gap-2 font-medium">
				{getTrendDescription(expenseChangePercent).text}
				{#if getTrendDescription(expenseChangePercent).icon === 'up'}
					<TrendingUpIcon class="size-4" />
				{:else}
					<TrendingDownIcon class="size-4" />
				{/if}
			</div>
			<div class="text-muted-foreground">vs {formatCurrency(totalExpenseLastMonth)} last month</div>
		</Card.Footer>
	</Card.Root>
	<Card.Root class="@container/card">
		<Card.Header>
			<Card.Description>Savings</Card.Description>
			<Card.Title class="text-2xl font-semibold tabular-nums @[250px]/card:text-3xl">
				{formatCurrency(totalAccumulatedSavings)}
			</Card.Title>
			<Card.Action>
				<Badge variant="outline">
					<TrendingUpIcon />
					Current Total
				</Badge>
			</Card.Action>
		</Card.Header>
		<Card.Footer class="flex-col items-start gap-1.5 text-sm">
			<div class="line-clamp-1 flex gap-2 font-medium">
				Accumulated over time <TrendingUpIcon class="size-4" />
			</div>
			<div class="text-muted-foreground">Across all accounts</div>
		</Card.Footer>
	</Card.Root>
	<Card.Root class="@container/card">
		<Card.Header>
			<Card.Description>Left to Spend</Card.Description>
			<Card.Title class="text-2xl font-semibold tabular-nums @[250px]/card:text-3xl">
				{formatCurrency(leftToSpend)}
			</Card.Title>
			<Card.Action>
				<Badge variant="outline">
					{#if getTrendDescription(leftToSpendChangePercent).icon === 'up'}
						<TrendingUpIcon />
					{:else}
						<TrendingDownIcon />
					{/if}
					{leftToSpendChangePercent > 0 ? '+' : ''}{leftToSpendChangePercent.toFixed(1)}%
				</Badge>
			</Card.Action>
		</Card.Header>
		<Card.Footer class="flex-col items-start gap-1.5 text-sm">
			<div class="line-clamp-1 flex gap-2 font-medium">
				{getTrendDescription(leftToSpendChangePercent).text}
				{#if getTrendDescription(leftToSpendChangePercent).icon === 'up'}
					<TrendingUpIcon class="size-4" />
				{:else}
					<TrendingDownIcon class="size-4" />
				{/if}
			</div>
			<div class="text-muted-foreground">vs {formatCurrency(leftToSpendLastMonth)} last month</div>
		</Card.Footer>
	</Card.Root>
</div>
