<script lang="ts">
	import TrendingDownIcon from '@lucide/svelte/icons/trending-down';
	import TrendingUpIcon from '@lucide/svelte/icons/trending-up';
	import { scaleBand } from 'd3-scale';
	import { BarChart } from 'layerchart';
	import { cubicInOut } from 'svelte/easing';

	import { formatCurrency } from '$lib';
	import * as Card from '$lib/components/ui/card/index.js';
	import * as Chart from '$lib/components/ui/chart/index.js';

	type Props = { chartData: { month: string; total: number }[] };

	let { chartData }: Props = $props();

	const chartConfig = Object.fromEntries(
		chartData.slice(0, 5).map((d) => [d.month, { label: d.month, color: 'var(--chart-1)' }])
	) satisfies Chart.ChartConfig;

	let monthRange = $derived(
		chartData.length
			? chartData.length === 1
				? chartData[0].month
				: `${chartData[0].month} - ${chartData[chartData.length - 1].month}`
			: 'No data'
	);
	let trendDelta = $derived(
		chartData.length >= 2
			? chartData[chartData.length - 1].total - chartData[chartData.length - 2].total
			: 0
	);
</script>

<Card.Root>
	<Card.Header>
		<Card.Title>Total by Months</Card.Title>
		<Card.Description>{monthRange}</Card.Description>
	</Card.Header>
	<Card.Content>
		<Chart.Container config={chartConfig}>
			<BarChart
				data={chartData}
				xScale={scaleBand().padding(0.25)}
				x="month"
				axis="x"
				series={[{ key: 'total', label: 'Total', color: 'var(--chart-1)' }]}
				props={{
					bars: {
						stroke: 'none',
						rounded: 'all',
						radius: 8,
						motion: { type: 'tween', duration: 500, easing: cubicInOut }
					},
					highlight: { area: { fill: 'none' } },
					xAxis: { format: (d) => d.slice(0, 3) }
				}}
			>
				{#snippet tooltip()}
					<Chart.Tooltip hideLabel />
				{/snippet}
			</BarChart>
		</Chart.Container>
	</Card.Content>
	<Card.Footer>
		<div class="flex w-full items-start gap-2 text-sm">
			<div class="grid gap-2">
				{#if chartData.length >= 2}
					<div class="flex items-center gap-2 leading-none font-medium">
						{#if trendDelta >= 0}
							Up {formatCurrency(trendDelta)} from the previous month <TrendingUpIcon
								class="size-4"
							/>
						{:else}
							Down {formatCurrency(Math.abs(trendDelta))} from the previous month <TrendingDownIcon
								class="size-4"
							/>
						{/if}
					</div>
				{/if}
				<div class="flex items-center gap-2 leading-none text-muted-foreground">
					Showing total expenses by month
				</div>
			</div>
		</div>
	</Card.Footer>
</Card.Root>
