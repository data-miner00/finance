<script lang="ts">
	import { onMount } from 'svelte';

	import DataTable from '$lib/components/data-table-revamp.svelte';
	import { appState } from '$lib/states.svelte';

	import { columns } from './data-table/column';

	onMount(() => {
		appState.pageTitle = 'Taxes';
		appState.currentPage = 'tax';
	});
</script>

{#snippet dataTableControls()}
	<div></div>
{/snippet}

<div class="flex flex-col gap-4 py-4 md:gap-6 md:py-6">
	<div class="px-4 lg:px-6">
		<div class="max-w-4xl space-y-2">
			<h1 class="text-xl font-bold">Taxes</h1>
			<p class="text-sm text-muted-foreground">
				Track your taxes with a sortable tax table, category filters, and summary totals.
			</p>
		</div>
	</div>

	<div class="px-4 lg:px-6">
		<DataTable data={appState.taxes} {columns} controls={dataTableControls} />
	</div>
</div>
