<script lang="ts">
	import { onMount } from 'svelte';

	import DataTable from '$lib/components/data-table-revamp.svelte';
	import StatCard from '$lib/components/stat-card.svelte';
	import { appState } from '$lib/states.svelte';

	import { columns } from './data-table/column';

	onMount(() => {
		appState.pageTitle = 'Categories';
		appState.currentPage = 'category';
	});

	let usedCategoryNames = $derived(
		new Set(appState.expenses.map((e) => e.categoryName).filter((name) => !!name))
	);

	let unusedCategoryCount = $derived(
		appState.categories.filter((c) => !usedCategoryNames.has(c.name)).length
	);
</script>

<div class="flex flex-col gap-4 py-4 md:gap-6 md:py-6">
	<div class="px-4 lg:px-6">
		<div class="max-w-4xl space-y-2">
			<h1 class="text-xl font-bold">Categories</h1>
			<p class="text-sm text-muted-foreground">
				Manage expense categories — rename, recolor, merge, or remove them.
			</p>
		</div>
	</div>

	<div
		class="grid grid-cols-1 gap-4 px-4 *:data-[slot=card]:bg-linear-to-t *:data-[slot=card]:from-primary/5 *:data-[slot=card]:to-card *:data-[slot=card]:shadow-xs lg:px-6 @xl/main:grid-cols-2 @5xl/main:grid-cols-4 dark:*:data-[slot=card]:bg-card"
	>
		<StatCard
			description="Total Categories"
			value={appState.categories.length + ' Categories'}
			badgeText="Categories"
			trendDirection="up"
			footerText="Count YTD"
			footerSubText="Across all expenses"
		/>

		<StatCard
			description="Unused Categories"
			value={unusedCategoryCount + ' Unused'}
			badgeText="Unused"
			trendDirection="up"
			footerText="No expenses assigned"
			footerSubText="Candidates for cleanup"
		/>
	</div>

	<div class="px-4 lg:px-6">
		<DataTable data={appState.categories} {columns} />
	</div>
</div>
