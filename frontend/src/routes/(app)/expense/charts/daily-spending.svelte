<script lang="ts">
	import TrendingDownIcon from '@lucide/svelte/icons/trending-down';
	import TrendingUpIcon from '@lucide/svelte/icons/trending-up';
	import { scaleUtc } from 'd3-scale';
	import { curveNatural } from 'd3-shape';
	import { LineChart } from 'layerchart';

	import { formatCurrency } from '$lib';
	import * as Card from '$lib/components/ui/card/index.js';
	import * as Chart from '$lib/components/ui/chart/index.js';

	type DailyTotal = {
		day: Date;
		total: number;
	};

	type Props = {
		chartData: DailyTotal[];
	};

	let { chartData }: Props = $props();

	const chartConfig = {
		total: { label: 'Expense', color: 'var(--chart-1)' }
	} satisfies Chart.ChartConfig;

	let average = $derived(
		chartData.length
			? chartData.reduce((sum, d) => sum + d.total, 0) / chartData.length
			: 0
	);
	let trendDelta = $derived(
		chartData.length >= 2
			? chartData[chartData.length - 1].total - chartData[chartData.length - 2].total
			: 0
	);
</script>

<Card.Root>
	<Card.Header>
		<Card.Title>Daily Spendings</Card.Title>
		<Card.Description>Showing daily totals for the last {chartData.length} days</Card.Description>
	</Card.Header>
	<Card.Content>
		<Chart.Container config={chartConfig}>
			<LineChart
				data={chartData}
				x="day"
				xScale={scaleUtc()}
				axis="x"
				series={[
					{
						key: 'total',
						label: 'Expense',
						color: chartConfig.total.color
					}
				]}
				props={{
					spline: { curve: curveNatural, motion: 'tween', strokeWidth: 2 },
					xAxis: {
						format: (v: Date) => v.toLocaleDateString('en-US', { month: 'short', day: 'numeric' })
					},
					highlight: { points: { r: 4 } }
				}}
			>
				{#snippet tooltip()}
					<Chart.Tooltip hideLabel />
				{/snippet}
			</LineChart>
		</Chart.Container>
	</Card.Content>
	<Card.Footer>
		<div class="flex w-full items-start gap-2 text-sm">
			<div class="grid gap-2">
				<div class="flex items-center gap-2 leading-none font-medium">
					{#if trendDelta >= 0}
						Up {formatCurrency(trendDelta)} from the previous day <TrendingUpIcon class="size-4" />
					{:else}
						Down {formatCurrency(Math.abs(trendDelta))} from the previous day <TrendingDownIcon
							class="size-4"
						/>
					{/if}
				</div>
				<div class="flex items-center gap-2 leading-none text-muted-foreground">
					Averaging {formatCurrency(average)} per day
				</div>
			</div>
		</div>
	</Card.Footer>
</Card.Root>
