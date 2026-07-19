<script lang="ts">
	import TrendingUpIcon from '@lucide/svelte/icons/trending-up';
	import { scaleBand } from 'd3-scale';
	import { BarChart } from 'layerchart';

	import * as Card from '$lib/components/ui/card/index.js';
	import * as Chart from '$lib/components/ui/chart/index.js';

	type ChartData = {
		month: string;
		savingsRate: number;
	};

	type Props = {
		chartData: ChartData[];
	};

	let { chartData }: Props = $props();
	const chartConfig = {
		savingsRate: { label: 'Savings Rate' }
	} satisfies Chart.ChartConfig;

	let averageRate = $derived(
		chartData.length ? chartData.reduce((sum, d) => sum + d.savingsRate, 0) / chartData.length : 0
	);
</script>

<Card.Root>
	<Card.Header>
		<Card.Title>Savings Rate</Card.Title>
		<Card.Description>Percentage of income left over each month</Card.Description>
	</Card.Header>
	<Card.Content>
		<Chart.Container config={chartConfig}>
			<BarChart
				labels={{
					offset: 5,
					value: (d) => `${d.savingsRate.toFixed(0)}%`,
					fill: (d) => (d.savingsRate >= 0 ? 'var(--chart-1)' : 'var(--chart-2)')
				}}
				data={chartData}
				xScale={scaleBand().padding(0.25)}
				x="month"
				y="savingsRate"
				yNice={4}
				yBaseline={0}
				cRange={['var(--chart-1)', 'var(--chart-2)']}
				c={(d) => (d.savingsRate >= 0 ? 'var(--chart-1)' : 'var(--chart-2)')}
				axis={false}
				props={{
					bars: { stroke: 'none', radius: 0 },
					highlight: { area: { fill: 'none' } },
					xAxis: { format: (d) => d.slice(0, 3) }
				}}
			>
				{#snippet tooltip()}
					<Chart.Tooltip hideLabel hideIndicator nameKey="savingsRate" />
				{/snippet}
			</BarChart>
		</Chart.Container>
	</Card.Content>
	<Card.Footer>
		<div class="flex w-full items-start gap-2 text-sm">
			<div class="grid gap-2">
				<div class="flex items-center gap-2 leading-none font-medium">
					Averaging {averageRate.toFixed(1)}% saved across shown months <TrendingUpIcon
						class="size-4"
					/>
				</div>
				<div class="flex items-center gap-2 leading-none text-muted-foreground">
					(Income − Expense) ÷ Income for each month
				</div>
			</div>
		</div>
	</Card.Footer>
</Card.Root>
