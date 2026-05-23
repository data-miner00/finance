<script lang="ts">
	import './layout.css';
	import favicon from '$lib/assets/favicon.svg';
	import * as Sidebar from '$lib/components/ui/sidebar/index.js';
	import AppSidebar from '$lib/components/app-sidebar.svelte';
	import SiteHeader from '$lib/components/site-header.svelte';
	import AddTransactionDialog from '$lib/components/custom/add-transaction-dialog.svelte';
	import { appState } from '$lib/states.svelte';
	import { onMount } from 'svelte';
	import {
		getAccounts,
		getExpenses,
		getIncomes,
		getPiggyBanks,
		getRecurringActions
	} from '$lib/services';

	let { children } = $props();

	onMount(async () => {
		appState.expenses = await getExpenses();
		appState.accounts = await getAccounts();
		appState.incomes = await getIncomes();
		appState.recurringActions = await getRecurringActions();
		appState.piggyBanks = await getPiggyBanks();
	});
</script>

<svelte:head><link rel="icon" href={favicon} /></svelte:head>

<Sidebar.Provider
	style="--sidebar-width: calc(var(--spacing) * 72); --header-height: calc(var(--spacing) * 12);"
>
	<AppSidebar variant="inset" />
	<Sidebar.Inset>
		<SiteHeader />
		<div class="flex flex-1 flex-col">
			<div class="@container/main flex flex-1 flex-col gap-2">
				{@render children()}
			</div>
		</div>
	</Sidebar.Inset>
</Sidebar.Provider>

<AddTransactionDialog bind:open={appState.isAddTransactionDialogOpen} />
