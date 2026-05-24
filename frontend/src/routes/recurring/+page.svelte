<script lang="ts">
	import { appState } from '$lib/states.svelte';
	import DataTable from './data-table/index.svelte';
	import { columns } from './data-table/column';
	import { onMount } from 'svelte';

	onMount(() => {
		appState.pageTitle = 'Recurring Actions';
	});
</script>

<div class="flex flex-col gap-4 py-4 md:gap-6 md:py-6">
	<div class="px-4 lg:px-6">
		<div class="max-w-4xl space-y-2">
			<h1 class="text-xl font-bold">Recurring Actions</h1>
			<p class="text-sm text-muted-foreground">
				Manage your recurring actions and view their details.
			</p>
		</div>
	</div>

	<div class="px-4 lg:px-6">
		<DataTable data={appState.recurringActions} {columns} />
	</div>
</div>
