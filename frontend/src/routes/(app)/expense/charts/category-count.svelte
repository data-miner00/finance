<script lang="ts">
	import TrendingUpIcon from '@lucide/svelte/icons/trending-up';
	import { Arc, PieChart, Text } from 'layerchart';

	import * as Card from '$lib/components/ui/card/index.js';
	import * as Chart from '$lib/components/ui/chart/index.js';

	type Props = { chartData: { category: string; count: number; color: string }[] };

	let { chartData }: Props = $props();

	const chartConfig = Object.fromEntries(
		chartData
			.slice(0, 5)
			.map((d) => [d.category, { label: d.category, color: d.color }])
			.concat([
				[
					'other',
					{
						label: 'Other',
						color: 'var(--color-other)'
					}
				]
			])
	) satisfies Chart.ChartConfig;
</script>

<Card.Root class="flex flex-col">
	<Card.Header class="items-center">
		<Card.Title>Category Distribution</Card.Title>
		<Card.Description>January - June 2024</Card.Description>
	</Card.Header>
	<Card.Content class="flex-1">
		<Chart.Container config={chartConfig} class="mx-auto aspect-square max-h-[250px]">
			<PieChart
				data={chartData}
				key="category"
				value="count"
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
		<div class="flex items-center gap-2 leading-none font-medium">
			Trending up by 5.2% this month <TrendingUpIcon class="size-4" />
		</div>
		<div class="leading-none text-muted-foreground">
			Showing total visitors for the last 6 months
		</div>
	</Card.Footer>
</Card.Root>
