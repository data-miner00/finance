<script lang="ts">
	import { Arc, PieChart, Text } from 'layerchart';

	import { formatCurrency } from '$lib';
	import * as Card from '$lib/components/ui/card/index.js';
	import * as Chart from '$lib/components/ui/chart/index.js';

	type Props = { chartData: { category: string; cost: number; color: string }[] };

	let { chartData }: Props = $props();

	const chartConfig = Object.fromEntries(
		chartData.map((d) => [d.category, { label: d.category, color: d.color }])
	) satisfies Chart.ChartConfig;

	let totalCost = $derived(chartData.reduce((sum, d) => sum + d.cost, 0));
	let topCategory = $derived.by(() =>
		chartData.length ? chartData.reduce((max, d) => (d.cost > max.cost ? d : max)) : undefined
	);
	let topShare = $derived(
		topCategory && totalCost > 0 ? (topCategory.cost / totalCost) * 100 : 0
	);
</script>

<Card.Root class="flex flex-col">
	<Card.Header class="items-center">
		<Card.Title>Category Cost Distribution</Card.Title>
		<Card.Description>All-time cost breakdown by category</Card.Description>
	</Card.Header>
	<Card.Content class="flex-1">
		<Chart.Container config={chartConfig} class="mx-auto aspect-square max-h-[250px]">
			<PieChart
				data={chartData}
				key="category"
				value="cost"
				cRange={chartData.map((d) => d.color)}
				c="color"
				props={{
					pie: {
						motion: 'tween'
					}
				}}
			>
				{#snippet tooltip()}
					<Chart.Tooltip hideLabel />
				{/snippet}
				{#snippet arc({ props, visibleData, index })}
					{@const category = visibleData[index].category}
					<Arc {...props}>
						{#snippet children({ getArcTextProps })}
							<Text
								value={category}
								{...getArcTextProps('centroid')}
								font-size="12"
								class="fill-background capitalize"
							/>
						{/snippet}
					</Arc>
				{/snippet}
			</PieChart>
		</Chart.Container>
	</Card.Content>
	<Card.Footer class="flex-col gap-2 text-sm">
		{#if topCategory}
			<div class="flex items-center gap-2 leading-none font-medium">
				{topCategory.category} leads at {topShare.toFixed(1)}% of total spend
			</div>
		{/if}
		<div class="leading-none text-muted-foreground">
			Based on {formatCurrency(totalCost)} in all-time recorded expenses
		</div>
	</Card.Footer>
</Card.Root>
