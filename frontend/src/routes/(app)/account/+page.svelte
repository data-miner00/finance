<script lang="ts">
	import { TrendingUpIcon } from '@lucide/svelte';
	import { onMount } from 'svelte';

	import { formatCurrency } from '$lib';
	import DataTable from '$lib/components/data-table-revamp.svelte';
	import { Badge } from '$lib/components/ui/badge/index.js';
	import * as Card from '$lib/components/ui/card/index.js';
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
	<div
		class="grid grid-cols-1 gap-4 px-4 *:data-[slot=card]:bg-gradient-to-t *:data-[slot=card]:from-primary/5 *:data-[slot=card]:to-card *:data-[slot=card]:shadow-xs lg:px-6 @xl/main:grid-cols-2 @5xl/main:grid-cols-4 dark:*:data-[slot=card]:bg-card"
	>
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
				<Card.Description>Credit Card Bills</Card.Description>
				<Card.Title class="text-2xl font-semibold tabular-nums @[250px]/card:text-3xl">
					{formatCurrency(totalCreditCardBills)}
				</Card.Title>
				<Card.Action>
					<Badge variant="outline">
						<TrendingUpIcon />
						Current Bill
					</Badge>
				</Card.Action>
			</Card.Header>
			<Card.Footer class="flex-col items-start gap-1.5 text-sm">
				<div class="line-clamp-1 flex gap-2 font-medium">
					Accumulated over time <TrendingUpIcon class="size-4" />
				</div>
				<div class="text-muted-foreground">Across credit cards</div>
			</Card.Footer>
		</Card.Root>
	</div>
	<div class="px-4 lg:px-6">
		<div class="max-w-4xl space-y-2">
			<h1 class="text-xl font-bold">Accounts</h1>
			<p class="text-sm text-muted-foreground">
				Manage your financial accounts and view their balances.
			</p>
		</div>
	</div>

	<div class="px-4 lg:px-6">
		<DataTable data={appState.accounts} {columns} controls={dataTableControls} />
	</div>
</div>
