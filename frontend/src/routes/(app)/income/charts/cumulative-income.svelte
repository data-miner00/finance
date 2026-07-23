<script lang="ts">
	import { scaleBand } from 'd3-scale';
	import { curveNatural } from 'd3-shape';
	import { LineChart } from 'layerchart';

	import { formatCurrency } from '$lib';
	import * as Card from '$lib/components/ui/card/index.js';
	import * as Chart from '$lib/components/ui/chart/index.js';

	type ChartData = {
		month: string;
		cumulative: number;
	};

	type Props = {
		chartData: ChartData[];
	};

	let { chartData }: Props = $props();

	const chartConfig = {
		cumulative: { label: 'Cumulative Income', color: 'var(--chart-1)' }
	} satisfies Chart.ChartConfig;

	let firstMonth = $derived(chartData[0]?.month);
	let lastMonth = $derived(chartData[chartData.length - 1]?.month);
	let ytdTotal = $derived(chartData[chartData.length - 1]?.cumulative ?? 0);
</script>

<Card.Root>
	<Card.Header>
		<Card.Title>Cumulative Income (YTD)</Card.Title>
		<Card.Description>{firstMonth} - {lastMonth}</Card.Description>
	</Card.Header>
	<Card.Content>
		<Chart.Container config={chartConfig}>
			<LineChart
				data={chartData}
				x="month"
				xScale={scaleBand().padding(0.25)}
				axis="x"
				series={[
					{ key: 'cumulative', label: 'Cumulative Income', color: chartConfig.cumulative.color }
				]}
				props={{
					spline: { curve: curveNatural, motion: 'tween', strokeWidth: 2 },
					xAxis: { format: (d) => d.slice(0, 3) },
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
					{formatCurrency(ytdTotal)} earned year-to-date
				</div>
				<div class="flex items-center gap-2 leading-none text-muted-foreground">
					Running total across {chartData.length}
					{chartData.length === 1 ? 'month' : 'months'}
				</div>
			</div>
		</div>
	</Card.Footer>
</Card.Root>
