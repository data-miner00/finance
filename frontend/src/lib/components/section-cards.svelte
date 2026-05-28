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

	// Derived: Total income for this month
	let totalIncomeThisMonth = $derived(
		appState.incomes
			.filter((income) => isCurrentMonth(income.createdAt))
			.reduce((sum, income) => sum + income.amount, 0)
	);

	// Derived: Total expense for this month
	let totalExpenseThisMonth = $derived(
		appState.expenses
			.filter((expense) => isCurrentMonth(expense.actionedAt))
			.reduce((sum, expense) => sum + expense.amount, 0)
	);

	// Derived: Total accumulated savings (sum of all account balances)
	let totalAccumulatedSavings = $derived(
		appState.accounts.reduce((sum, account) => sum + account.balance, 0)
	);

	// Derived: Total piggy bank amounts
	let totalPiggyBankAmount = $derived(appState.piggyBanks.reduce((sum, pb) => sum + pb.amount, 0));

	// Derived: Left to spend (income - expense - piggy bank)
	let leftToSpend = $derived(totalIncomeThisMonth - totalExpenseThisMonth - totalPiggyBankAmount);

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
					<TrendingUpIcon />
					+12.5%
				</Badge>
			</Card.Action>
		</Card.Header>
		<Card.Footer class="flex-col items-start gap-1.5 text-sm">
			<div class="line-clamp-1 flex gap-2 font-medium">
				Trending up this month <TrendingUpIcon class="size-4" />
			</div>
			<div class="text-muted-foreground">Income for this month</div>
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
					<TrendingDownIcon />
					-20%
				</Badge>
			</Card.Action>
		</Card.Header>
		<Card.Footer class="flex-col items-start gap-1.5 text-sm">
			<div class="line-clamp-1 flex gap-2 font-medium">
				Down 20% this period <TrendingDownIcon class="size-4" />
			</div>
			<div class="text-muted-foreground">Spending for this period</div>
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
					+12.5%
				</Badge>
			</Card.Action>
		</Card.Header>
		<Card.Footer class="flex-col items-start gap-1.5 text-sm">
			<div class="line-clamp-1 flex gap-2 font-medium">
				Slowly accumulating <TrendingUpIcon class="size-4" />
			</div>
			<div class="text-muted-foreground">Total savings accumulated</div>
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
					<TrendingUpIcon />
					+4.5%
				</Badge>
			</Card.Action>
		</Card.Header>
		<Card.Footer class="flex-col items-start gap-1.5 text-sm">
			<div class="line-clamp-1 flex gap-2 font-medium">
				Steady performance increase <TrendingUpIcon class="size-4" />
			</div>
			<div class="text-muted-foreground">Total amount left to spend this month</div>
		</Card.Footer>
	</Card.Root>
</div>
