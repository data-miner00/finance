<script lang="ts">
	import { appState } from '$lib/states.svelte';
	import DataTable from './data-table/index.svelte';
	import { columns } from './data-table/column';
	import { onMount } from 'svelte';

	onMount(() => {
		appState.pageTitle = 'Incomes';
	});
</script>

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
		<DataTable data={appState.incomes} {columns} />
	</div>
</div>
