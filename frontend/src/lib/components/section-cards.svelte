<script lang="ts">
	import { calculatePercentageChange, isCurrentMonth, isLastMonth } from '$lib';
	import StatCard from '$lib/components/stat-card.svelte';
	import { AccountType } from '$lib/services';
	import { appState } from '$lib/states.svelte';

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
		appState.accounts
			.filter((account) => account.type !== AccountType.CreditCard)
			.reduce((sum, account) => sum + account.balance, 0)
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
	<StatCard
		description="Income"
		value={formatCurrency(totalIncomeThisMonth)}
		trendDirection={getTrendDescription(incomeChangePercent).icon}
		badgeText="{incomeChangePercent > 0 ? '+' : ''}{incomeChangePercent.toFixed(1)}%"
		footerText={getTrendDescription(incomeChangePercent).text}
		footerSubText="vs {formatCurrency(totalIncomeLastMonth)} last month"
	/>
	<StatCard
		description="Spending"
		value={formatCurrency(totalExpenseThisMonth)}
		trendDirection={getTrendDescription(expenseChangePercent).icon}
		badgeText="{expenseChangePercent > 0 ? '+' : ''}{expenseChangePercent.toFixed(1)}%"
		footerText={getTrendDescription(expenseChangePercent).text}
		footerSubText="vs {formatCurrency(totalExpenseLastMonth)} last month"
	/>
	<StatCard
		description="Savings"
		value={formatCurrency(totalAccumulatedSavings)}
		trendDirection="up"
		badgeText="Current Total"
		footerText="Accumulated over time"
		footerSubText="Across all accounts"
	/>
	<StatCard
		description="Left to Spend"
		value={formatCurrency(leftToSpend)}
		trendDirection={getTrendDescription(leftToSpendChangePercent).icon}
		badgeText="{leftToSpendChangePercent > 0 ? '+' : ''}{leftToSpendChangePercent.toFixed(1)}%"
		footerText={getTrendDescription(leftToSpendChangePercent).text}
		footerSubText="vs {formatCurrency(leftToSpendLastMonth)} last month"
	/>
</div>
