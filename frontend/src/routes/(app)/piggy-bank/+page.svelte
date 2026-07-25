<script lang="ts">
	import { PlusIcon } from '@lucide/svelte';
	import { onMount } from 'svelte';

	import DataTable from '$lib/components/data-table-revamp.svelte';
	import { appState } from '$lib/states.svelte';

	import { columns } from './data-table/column';

	onMount(() => {
		appState.pageTitle = 'Piggy Banks';
		appState.currentPage = 'piggyBank';
	});
</script>

{#snippet dataTableControls()}
	<div></div>
{/snippet}

<div class="flex flex-col gap-4 py-4 md:gap-6 md:py-6">
	<div class="px-4 lg:px-6">
		<div class="max-w-4xl space-y-2">
			<h1 class="text-xl font-bold">Piggy Banks</h1>
			<p class="text-sm text-muted-foreground">
				Track your piggy banks with a sortable table, category filters, and summary totals.
			</p>
		</div>
	</div>

	<div class="px-4 lg:px-6">
		<DataTable data={appState.piggyBanks} {columns} controls={dataTableControls} />
	</div>
</div>
