<script lang="ts">
	import { onMount } from 'svelte';

	import { formatCurrency } from '$lib';
	import DataTable from '$lib/components/data-table-revamp.svelte';
	import StatCard from '$lib/components/stat-card.svelte';
	import { AccountType } from '$lib/services';
	import { appState } from '$lib/states.svelte';

	import { columns } from './data-table/column';

	onMount(() => {
		appState.pageTitle = 'Accounts';
		appState.currentPage = 'account';
	});

	let totalAccumulatedSavings = $derived(
		appState.accounts
			.filter((account) => account.type !== AccountType.CreditCard)
			.reduce((sum, account) => sum + account.balance, 0)
	);

	let totalCreditCardBills = $derived(
		appState.accounts
			.filter((account) => account.type === AccountType.CreditCard)
			.reduce((sum, account) => sum + account.balance, 0)
	);
</script>

{#snippet dataTableControls()}
	<div></div>
{/snippet}

<div class="flex flex-col gap-4 py-4 md:gap-6 md:py-6">
	<div class="px-4 lg:px-6">
		<div class="max-w-4xl space-y-2">
			<h1 class="text-xl font-bold">Accounts</h1>
			<p class="text-sm text-muted-foreground">
				Manage your financial accounts and view their balances.
			</p>
		</div>
	</div>

	<div
		class="grid grid-cols-1 gap-4 px-4 *:data-[slot=card]:bg-gradient-to-t *:data-[slot=card]:from-primary/5 *:data-[slot=card]:to-card *:data-[slot=card]:shadow-xs lg:px-6 @xl/main:grid-cols-2 @5xl/main:grid-cols-4 dark:*:data-[slot=card]:bg-card"
	>
		<StatCard
			description="Savings"
			value={formatCurrency(totalAccumulatedSavings)}
			badgeText="Current Total"
			trendDirection="up"
			footerText="Accumulated over time"
			footerSubText="Across all accounts"
		/>

		<StatCard
			description="Credit Card Bills"
			value={formatCurrency(totalCreditCardBills)}
			badgeText="Current Bill"
			trendDirection="up"
			footerText="Accumulated over time"
			footerSubText="Across credit cards"
		/>

		<StatCard
			description="Total Accounts"
			value={appState.accounts.length + ' Accounts'}
			badgeText="Accounts"
			trendDirection="up"
			footerText="Count YTD"
			footerSubText="Across all accounts"
		/>
	</div>

	<div class="px-4 lg:px-6">
		<DataTable data={appState.accounts} {columns} controls={dataTableControls} />
	</div>
</div>
