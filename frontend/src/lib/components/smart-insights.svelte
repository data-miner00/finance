<script lang="ts">
	import BulbIcon from '@tabler/icons-svelte/icons/bulb';
	import TrendingDownIcon from '@tabler/icons-svelte/icons/trending-down';
	import TrendingUpIcon from '@tabler/icons-svelte/icons/trending-up';

	import { getCategoryIcon } from '$lib/category-icons';
	import { Badge } from '$lib/components/ui/badge/index.js';
	import * as Card from '$lib/components/ui/card/index.js';
	import { type Insight, generateInsights } from '$lib/insights';
	import { appState } from '$lib/states.svelte';

	let insights = $derived(
		generateInsights(appState.expenses, { currency: appState.settings.defaultCurrency })
	);

	function insightIcon(insight: Insight) {
		if (insight.categoryName) {
			const category = appState.categories.find((c) => c.name === insight.categoryName);
			return getCategoryIcon(category?.icon);
		}
		return BulbIcon;
	}
</script>

{#if insights.length > 0}
	<div class="px-4 lg:px-6">
		<Card.Root>
			<Card.Header>
				<Card.Title>Smart Insights</Card.Title>
				<Card.Description>What your spending says this month</Card.Description>
			</Card.Header>
			<Card.Content class="flex flex-col gap-3">
				{#each insights as insight (insight.kind + (insight.categoryName ?? ''))}
					{@const Icon = insightIcon(insight)}
					<div class="flex items-start gap-3">
						<div class="mt-0.5 rounded-md bg-muted p-1.5 text-muted-foreground">
							<Icon class="size-4" />
						</div>
						<div class="flex flex-col gap-0.5">
							<div class="flex items-center gap-2 font-medium">
								{insight.title}
								{#if insight.tone === 'negative'}
									<Badge variant="destructive" class="px-1.5">
										<TrendingUpIcon />
									</Badge>
								{:else if insight.tone === 'positive'}
									<Badge variant="outline" class="px-1.5">
										<TrendingDownIcon class="text-green-500 dark:text-green-400" />
									</Badge>
								{/if}
							</div>
							<p class="text-sm text-muted-foreground">{insight.message}</p>
						</div>
					</div>
				{/each}
			</Card.Content>
		</Card.Root>
	</div>
{/if}
