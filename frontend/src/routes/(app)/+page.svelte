<script lang="ts">
	import { onMount } from 'svelte';

	import ChartAreaInteractive from '$lib/components/chart-area-interactive.svelte';
	import data from '$lib/components/data';
	import DataTable from '$lib/components/data-table.svelte';
	import SectionCards from '$lib/components/section-cards.svelte';
	import { appState } from '$lib/states.svelte';

	onMount(() => {
		appState.pageTitle = 'Dashboard';
		appState.currentPage = 'dashboard';
	});

	let chartData = $derived(
		appState.expenses.map((e) => ({ date: new Date(e.actionedAt), expense: e.amount }))
	);
</script>

<div class="flex flex-col gap-4 py-4 md:gap-6 md:py-6">
	<SectionCards />
	<div class="px-4 lg:px-6">
		<ChartAreaInteractive {chartData} />
	</div>
	<DataTable {data} />
</div>
