<script lang="ts">
	import { TrendingDownIcon } from '@lucide/svelte';
	import TrendingUpIcon from '@lucide/svelte/icons/trending-up';
	import { scaleBand } from 'd3-scale';
	import { BarChart } from 'layerchart';

	import { formatCurrency } from '$lib';
	import * as Card from '$lib/components/ui/card/index.js';
	import * as Chart from '$lib/components/ui/chart/index.js';

	type ChartData = {
		month: string;
		netIncome: number;
	};

	type Props = {
		chartData: ChartData[];
	};

	let { chartData }: Props = $props();
	const chartConfig = {
		netIncome: { label: 'Net Income' }
	} satisfies Chart.ChartConfig;

	let firstMonth = $derived(chartData[0]?.month);
	let lastMonth = $derived(chartData[chartData.length - 1]?.month);
	let hasTrend = $derived(chartData.length >= 2);
	// Net income can cross zero, so a currency delta is more meaningful here than a % change
	let change = $derived(
		hasTrend
			? chartData[chartData.length - 1].netIncome - chartData[chartData.length - 2].netIncome
			: 0
	);
</script>

<Card.Root>
	<Card.Header>
		<Card.Title>Monthly Net Income</Card.Title>
		<Card.Description>{firstMonth} - {lastMonth}</Card.Description>
	</Card.Header>
	<Card.Content>
		<Chart.Container config={chartConfig}>
			<BarChart
				labels={{
					offset: 5,
					value: (d) => d.month,
					fill: (d) => {
						if (d.netIncome > 0) {
							return 'var(--chart-1)';
						} else if (d.netIncome < 0) {
							return 'var(--chart-2)';
						}
					}
				}}
				data={chartData}
				xScale={scaleBand().padding(0.25)}
				x="month"
				y="netIncome"
				yNice={4}
				yBaseline={0}
				cRange={['var(--chart-1)', 'var(--chart-2)']}
				c={(d) => (d.netIncome > 0 ? 'var(--chart-1)' : 'var(--chart-2)')}
				axis={false}
				props={{
					bars: { stroke: 'none', radius: 0 },
					highlight: { area: { fill: 'none' } },
					xAxis: { format: (d) => d.slice(0, 3) }
				}}
			>
				{#snippet tooltip()}
					<Chart.Tooltip hideLabel hideIndicator nameKey="netIncome" />
				{/snippet}
			</BarChart>
		</Chart.Container>
	</Card.Content>
	<Card.Footer>
		<div class="flex w-full items-start gap-2 text-sm">
			<div class="grid gap-2">
				<div class="flex items-center gap-2 leading-none font-medium">
					{#if !hasTrend}
						Net income (income minus expenses) by month
					{:else if change >= 0}
						Up {formatCurrency(Math.abs(change))} vs last month <TrendingUpIcon class="size-4" />
					{:else}
						Down {formatCurrency(Math.abs(change))} vs last month <TrendingDownIcon class="size-4" />
					{/if}
				</div>
				<div class="flex items-center gap-2 leading-none text-muted-foreground">
					Showing net income for {chartData.length}
					{chartData.length === 1 ? 'month' : 'months'}
				</div>
			</div>
		</div>
	</Card.Footer>
</Card.Root>
