<script lang="ts">
	import ChartAreaInteractive from '$lib/components/chart-area-interactive.svelte';
	import DataTable from '$lib/components/data-table.svelte';
	import SectionCards from '$lib/components/section-cards.svelte';
	import data from '$lib/components/data';
	import { onMount } from 'svelte';
	import { appState } from '$lib/states.svelte';

	onMount(() => {
		appState.pageTitle = 'Dashboard';
	});
</script>

<div class="flex flex-col gap-4 py-4 md:gap-6 md:py-6">
	<SectionCards />
	<div class="px-4 lg:px-6">
		<ChartAreaInteractive />
	</div>
	<DataTable {data} />
</div>
