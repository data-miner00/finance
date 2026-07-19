<script lang="ts">
	import { TrendingDownIcon } from '@lucide/svelte';
	import TrendingUpIcon from '@lucide/svelte/icons/trending-up';
	import { scaleBand } from 'd3-scale';
	import { BarChart } from 'layerchart';
	import { cubicInOut } from 'svelte/easing';

	import * as Card from '$lib/components/ui/card/index.js';
	import * as Chart from '$lib/components/ui/chart/index.js';

	type ChartData = {
		month: string;
		total: number;
	};

	type Props = {
		chartData: ChartData[];
	};

	let { chartData }: Props = $props();

	const chartConfig = {
		income: { label: 'Income', color: 'var(--chart-1)' }
	} satisfies Chart.ChartConfig;

	let firstMonth = $derived(chartData[0].month);
	let lastMonth = $derived(chartData[chartData.length - 1].month);
	let currentMonthIncome = $derived(chartData[chartData.length - 1].total);
	let lastMonthIncome = $derived(chartData[chartData.length - 2].total);
	let percentageChange = $derived(((currentMonthIncome - lastMonthIncome) / lastMonthIncome) * 100);
</script>

<Card.Root>
	<Card.Header>
		<Card.Title>Monthly Income</Card.Title>
		<Card.Description>{firstMonth} - {lastMonth}</Card.Description>
	</Card.Header>
	<Card.Content>
		<Chart.Container config={chartConfig}>
			<BarChart
				data={chartData}
				xScale={scaleBand().padding(0.25)}
				x="month"
				axis="x"
				series={[{ key: 'total', label: 'Income', color: chartConfig.income.color }]}
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
				<div class="flex items-center gap-2 leading-none font-medium">
					{#if percentageChange >= 0}
						Trending up by {Math.abs(percentageChange).toFixed(2)}% this month <TrendingUpIcon
							class="size-4"
						/>
					{:else}
						Winding down by {Math.abs(percentageChange).toFixed(2)}% this month <TrendingDownIcon
							class="size-4"
						/>
					{/if}
				</div>
				<div class="flex items-center gap-2 leading-none text-muted-foreground">
					Showing total income for the last 6 months
				</div>
			</div>
		</div>
	</Card.Footer>
</Card.Root>
