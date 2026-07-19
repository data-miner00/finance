<script lang="ts">
	import { PlusIcon } from '@lucide/svelte';
	import { onMount } from 'svelte';

	import DataTable from '$lib/components/data-table-revamp.svelte';
	import { Button, buttonVariants } from '$lib/components/ui/button';
	import * as Dialog from '$lib/components/ui/dialog/index.js';
	import { Input } from '$lib/components/ui/input/index.js';
	import { Label } from '$lib/components/ui/label/index.js';
	import { createIncome } from '$lib/services';
	import { appState } from '$lib/states.svelte';

	import MonthlyDistribution from './charts/monthly-distribution.svelte';
	import MonthlyNetIncome from './charts/monthly-net-income.svelte';
	import SavingsRate from './charts/savings-rate.svelte';
	import { columns } from './data-table/column';

	let isDialogOpen = $state(false);

	let name = $state('');
	let amount = $state(0);
	let description = $state('');

	async function addIncome() {
		const income = await createIncome({
			name,
			amount,
			description: description || undefined
		});

		appState.incomes = [...appState.incomes, income];
		isDialogOpen = false;
		name = '';
		amount = 0;
		description = '';
	}

	onMount(() => {
		appState.pageTitle = 'Incomes';
		appState.currentPage = 'income';
	});

	type MonthlyBucket = { month: string; monthKey: string; total: number };

	function groupByMonth(items: { actionedAt: string; amount: number }[]): MonthlyBucket[] {
		const monthMap = new Map<string, number>();

		for (const item of items) {
			const date = new Date(item.actionedAt);
			const monthKey = `${date.getFullYear()}-${String(date.getMonth()).padStart(2, '0')}`;

			monthMap.set(monthKey, (monthMap.get(monthKey) ?? 0) + item.amount);
		}

		return Array.from(monthMap.entries())
			.sort(([a], [b]) => a.localeCompare(b))
			.map(([monthKey, total]) => {
				const [year, monthIndex] = monthKey.split('-').map(Number);
				const month = new Date(year, monthIndex, 1).toLocaleString('default', {
					month: 'long',
					year: 'numeric'
				});
				return { month, monthKey, total: Math.round(total * 100) / 100 };
			});
	}

	let monthlyIncome = $derived(groupByMonth(appState.incomes));
	let monthlyExpenses = $derived(groupByMonth(appState.expenses));

	// TODO: Last 6 month
	let monthlyNetIncome = $derived.by(() => {
		const monthKeys = new Set([
			...monthlyIncome.map((m) => m.monthKey),
			...monthlyExpenses.map((m) => m.monthKey)
		]);

		return Array.from(monthKeys)
			.sort()
			.map((monthKey) => {
				const income = monthlyIncome.find((m) => m.monthKey === monthKey);
				const expense = monthlyExpenses.find((m) => m.monthKey === monthKey);
				const month = (income ?? expense)!.month;
				const netIncome = (income?.total ?? 0) - (expense?.total ?? 0);
				return { month, netIncome };
			});
	});

	// TODO: Last 6 month
	let monthlySavingsRate = $derived.by(() =>
		monthlyIncome
			.filter((income) => income.total > 0)
			.map((income) => {
				const expense = monthlyExpenses.find((e) => e.monthKey === income.monthKey);
				const savingsRate = ((income.total - (expense?.total ?? 0)) / income.total) * 100;
				return { month: income.month, savingsRate: Math.max(0, Math.round(savingsRate * 10) / 10) };
			})
	);
</script>

{#snippet dataTableControls()}
	<Button size="sm" onclick={() => (isDialogOpen = true)}>
		<PlusIcon />
		Create Income
	</Button>
{/snippet}

<div class="flex gap-4 p-6">
	<MonthlyDistribution chartData={monthlyIncome} />

	<MonthlyNetIncome chartData={monthlyNetIncome} />

	<SavingsRate chartData={monthlySavingsRate} />
</div>

<div class="flex flex-col gap-4 py-4 md:gap-6 md:py-6">
	<div class="px-4 lg:px-6">
		<div class="max-w-4xl space-y-2">
			<h1 class="text-xl font-bold">Incomes</h1>
			<p class="text-sm text-muted-foreground">
				Track your income with a sortable income table, category filters, and summary totals.
			</p>
		</div>
	</div>

	<div class="px-4 lg:px-6">
		<DataTable data={appState.incomes} {columns} controls={dataTableControls} />
	</div>
</div>

<Dialog.Root bind:open={isDialogOpen}>
	<form>
		<Dialog.Content class="sm:max-w-106.25">
			<Dialog.Header>
				<Dialog.Title>Add Income</Dialog.Title>
				<Dialog.Description>Fill in the details to create a new income.</Dialog.Description>
			</Dialog.Header>
			<div class="grid gap-4">
				<div class="grid gap-3">
					<Label for="name-1">Name</Label>
					<Input id="name-1" name="name" placeholder="e.g. Salary" bind:value={name} />
				</div>
				<div class="grid gap-3">
					<Label for="amount-1">Amount</Label>
					<Input
						id="amount-1"
						name="amount"
						placeholder="0.00"
						type="number"
						step="0.01"
						bind:value={amount}
					/>
				</div>
				<div class="grid gap-3">
					<Label for="description-1">Description</Label>
					<Input
						id="description-1"
						name="description"
						placeholder="Optional description"
						bind:value={description}
					/>
				</div>
			</div>
			<Dialog.Footer>
				<Dialog.Close type="button" class={buttonVariants({ variant: 'outline' })}>
					Cancel
				</Dialog.Close>
				<Button type="submit" onclick={addIncome}>Create Income</Button>
			</Dialog.Footer>
		</Dialog.Content>
	</form>
</Dialog.Root>
