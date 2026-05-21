<script lang="ts">
	import { appState } from '$lib/states.svelte';
	import DataTable from './table/index.svelte';
	import { columns } from './table/column';
	import { onMount } from 'svelte';

	onMount(() => {
		appState.pageTitle = 'Expenses';
	});
</script>

<div class="flex flex-col gap-4 py-4 md:gap-6 md:py-6">
	<div class="px-4 lg:px-6">
		<div class="max-w-4xl space-y-2">
			<h1 class="text-xl font-bold">Expenses</h1>
			<p class="text-sm text-muted-foreground">
				Track your spending with a sortable expense table, category filters, and summary totals.
			</p>
		</div>
	</div>

	<div class="px-4 lg:px-6">
		<DataTable data={appState.expenses} {columns} />
	</div>
</div>
