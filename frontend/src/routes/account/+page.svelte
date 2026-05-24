<script lang="ts">
	import DataTable from './data-table/index.svelte';
	import { appState } from '$lib/states.svelte';
	import { columns } from './data-table/column';
</script>

<div class="flex flex-col gap-4 py-4 md:gap-6 md:py-6">
	<div class="px-4 lg:px-6">
		<div class="max-w-4xl space-y-2">
			<h1 class="text-xl font-bold">Accounts</h1>
			<p class="text-sm text-muted-foreground">
				Manage your financial accounts and view their balances.
			</p>
		</div>
	</div>

	<div class="px-4 lg:px-6">
		<DataTable data={appState.accounts} {columns} />
	</div>
</div>
