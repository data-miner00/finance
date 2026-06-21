<script lang="ts">
	import { onMount } from 'svelte';
	import { Toaster } from 'svelte-sonner';

	import favicon from '$lib/assets/favicon.svg';
	import AppSidebar from '$lib/components/app-sidebar.svelte';
	import AddTransactionDialog from '$lib/components/custom/add-transaction-dialog.svelte';
	import SiteHeader from '$lib/components/site-header.svelte';
	import * as Sidebar from '$lib/components/ui/sidebar/index.js';
	import {
		getAccounts,
		getExpenses,
		getIncomes,
		getPeople,
		getPiggyBanks,
		getRecurringActions,
		getTaxes
	} from '$lib/services';
	import { appState } from '$lib/states.svelte';

	import './layout.css';

	let { children } = $props();

	onMount(async () => {
		appState.expenses = await getExpenses();
		appState.accounts = await getAccounts();
		appState.incomes = await getIncomes();
		appState.recurringActions = await getRecurringActions();
		appState.piggyBanks = await getPiggyBanks();
		appState.people = await getPeople();
		appState.taxes = await getTaxes();
		appState.categories = Array.from(
			new Set(appState.expenses.map((e) => e.categoryName || '').filter((c) => !!c))
		);
		appState.knownLocations = Array.from(
			new Set(appState.expenses.map((e) => e.location || '').filter((l) => !!l))
		);
	});
</script>

<svelte:head>
	<link rel="icon" href={favicon} />
	<title>{appState.pageTitle}</title>
</svelte:head>

<Toaster position="top-center" />

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
