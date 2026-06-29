<script lang="ts">
	import { PlusIcon } from '@lucide/svelte';
	import { onMount } from 'svelte';
	import { toast } from 'svelte-sonner';

	import { formatCurrency, getDaysInMonth } from '$lib';
	import DataTable from '$lib/components/data-table-revamp.svelte';
	import { Badge } from '$lib/components/ui/badge/index.js';
	import { Button, buttonVariants } from '$lib/components/ui/button';
	import * as Card from '$lib/components/ui/card/index.js';
	import * as Dialog from '$lib/components/ui/dialog/index.js';
	import { Input } from '$lib/components/ui/input/index.js';
	import { Label } from '$lib/components/ui/label/index.js';
	import * as Select from '$lib/components/ui/select/index.js';
	import { createExpense, exportAllExpense, importExpenses } from '$lib/services';
	import type { Expense } from '$lib/services/types';
	import { appState } from '$lib/states.svelte';
	import type { DailyTotal, MonthlyTotal } from '$lib/types';

	import CategoryCost from './charts/category-cost.svelte';
	import CategoryCount from './charts/category-count.svelte';
	import DailySpending from './charts/daily-spending.svelte';
	import TotalByMonth from './charts/total-month.svelte';
	import { columns } from './table/column';

	let isDialogOpen = $state(false);
	let showCurrentMonthOnly = $state(true);

	let name = $state('');
	let amount = $state(0);
	let categoryName = $state<string | undefined>(undefined);
	let description = $state('');

	const timeRanges = [
		{ value: 'day', label: 'Day' },
		{ value: 'month', label: 'Month' },
		{ value: 'year', label: 'Year' },
		{ value: 'all', label: 'All' }
	];
	let timeRange = $state<'day' | 'month' | 'year' | 'all'>('day');
	const timeRangeLabel = $derived(
		timeRanges.find((tr) => tr.value === timeRange)?.label ?? 'Select a time range'
	);
	const isCurrentMonthExpense = (expense: Expense) => {
		const created = new Date(expense.createdAt);
		const today = new Date();
		return created.getFullYear() === today.getFullYear() && created.getMonth() === today.getMonth();
	};

	let filteredExpenses = $derived(
		showCurrentMonthOnly ? appState.expenses.filter(isCurrentMonthExpense) : appState.expenses
	);

	async function addExpense() {
		const expense = await createExpense({
			name,
			amount,
			categoryName,
			description: description || undefined
		});

		appState.expenses.push(expense);
		isDialogOpen = false;
		name = '';
		amount = 0;
		categoryName = undefined;
		description = '';
	}

	onMount(() => {
		appState.pageTitle = 'Expenses';
		appState.currentPage = 'expense';
	});

	function groupExpensesByMonth(expenses: Expense[]): MonthlyTotal[] {
		const monthMap = new Map<string, number>();

		for (const expense of expenses) {
			const date = new Date(expense.actionedAt || expense.createdAt);
			const month = date.toLocaleString('default', { month: 'long' }); // e.g. "January"

			monthMap.set(month, (monthMap.get(month) ?? 0) + expense.amount);
		}

		return Array.from(monthMap.entries())
			.reverse()
			.map(([month, total]) => ({
				month,
				total: Math.round(total * 100) / 100 // avoid floating point drift
			}));
	}

	function groupExpensesByDay(expenses: Expense[]): DailyTotal[] {
		const dayMap = new Map<Date, number>();

		const today = new Date();
		const dateInt = today.getDate();

		const mappedExpenses = expenses.map((expense) => ({
			effectiveDate: new Date(expense.actionedAt ?? expense.createdAt),
			total: expense.amount
		}));

		// last 7 days
		for (let i = dateInt; i > dateInt - 7; --i) {
			today.setDate(today.getDate() - 1);
			const currentMonth = today.getMonth();
			const currentDate = today.getDate();
			const totalAmount = mappedExpenses
				.filter((expense) => {
					return (
						expense.effectiveDate.getDate() == currentDate &&
						expense.effectiveDate.getMonth() == currentMonth
					);
				})
				.reduce((prev, curr) => prev + curr.total, 0);

			dayMap.set(new Date(today.getFullYear(), currentMonth, currentDate), totalAmount);
		}

		return Array.from(dayMap.entries())
			.reverse()
			.map(
				([day, total]): DailyTotal => ({
					day,
					total: Math.round(total * 100) / 100
				})
			);
	}

	const now = new Date();
	let averageSpendingPerDay = $derived(
		getAverageForMonth(appState.expenses, now.getFullYear(), now.getMonth() + 1)
	);
	let averageSpendingPerMonth = $derived(
		getMonthlyAverageForYear(appState.expenses, now.getFullYear())
	);
	let dailySpending = $derived(groupExpensesByDay(appState.expenses));

	function getAverageForMonth(expenses: Expense[], year: number, month: number): number {
		const filtered = expenses.filter((expense) => {
			const date = new Date(expense.actionedAt || expense.createdAt);
			return date.getFullYear() === year && date.getMonth() === month - 1; // month is 1-based
		});

		if (filtered.length === 0) return 0;

		const target = new Date(year, month - 1, 0);
		const total = filtered.reduce((sum, expense) => sum + expense.amount, 0);

		return Math.round((total / target.getDate()) * 100) / 100;
	}

	function getMonthlyAverageForYear(expenses: Expense[], year: number): number {
		const filtered = expenses.filter((expense) => {
			const date = new Date(expense.actionedAt || expense.createdAt);
			return date.getFullYear() === year;
		});

		if (filtered.length === 0) return 0;

		const total = filtered.reduce((sum, expense) => sum + expense.amount, 0);

		return Math.round((total / 12) * 100) / 100;
	}
	let monthlyExpense = $derived(groupExpensesByMonth(appState.expenses));

	async function exportAllData() {
		await exportAllExpense();
		toast.success('Successfully exported expenses.');
	}

	let files: FileList | undefined = $state();

	async function importAllData() {
		if (!files?.[0]) return;
		const file = files[0];

		try {
			await importExpenses(file);
			toast.success('Imported successfully.');
		} catch (e) {
			toast.error('Failed to import. ' + e);
		}
	}
</script>

<div class="flex gap-4 p-6">
	<CategoryCount
		chartData={appState.categories.map((category, index) => {
			const count = appState.expenses.filter((expense) => expense.categoryName === category).length;
			const color = `var(--chart-${index + 1})`;
			return { category, count, color };
		})}
	/>
	<CategoryCost
		chartData={appState.categories.map((category, index) => {
			const cost = appState.expenses
				.filter((expense) => expense.categoryName === category)
				.reduce((prev, curr) => prev + curr.amount, 0);
			const color = `var(--chart-${index + 1})`;
			return { category, cost, color };
		})}
	/>
	<TotalByMonth chartData={monthlyExpense} />

	<DailySpending chartData={dailySpending} />
</div>

<div class="flex gap-4 p-6">
	<Card.Root class="w-[200px]">
		<Card.Header>
			<Card.Description>Average daily spendings</Card.Description>
			<Card.Title class="text-2xl font-semibold tabular-nums @[250px]/card:text-3xl">
				{formatCurrency(averageSpendingPerDay)}
			</Card.Title>
		</Card.Header>
	</Card.Root>

	<Card.Root class="w-[200px]">
		<Card.Header>
			<Card.Description>Average monthly spendings</Card.Description>
			<Card.Title class="text-2xl font-semibold tabular-nums @[250px]/card:text-3xl">
				{formatCurrency(averageSpendingPerMonth)}
			</Card.Title>
		</Card.Header>
	</Card.Root>

	<Card.Root class="w-[200px]">
		<Card.Header>
			<Card.Description>Total expenses recorded</Card.Description>
			<Card.Title class="text-2xl font-semibold tabular-nums @[250px]/card:text-3xl">
				{appState.expenses.length}
			</Card.Title>
		</Card.Header>
	</Card.Root>

	<Card.Root class="w-[200px]">
		<Card.Header>
			<Card.Description>Expenses logged this month</Card.Description>
			<Card.Title class="text-2xl font-semibold tabular-nums @[250px]/card:text-3xl">
				{appState.expenses.filter(isCurrentMonthExpense).length}
			</Card.Title>
		</Card.Header>
	</Card.Root>
</div>

{#snippet dataTableControls()}
	<div class="flex items-center justify-between gap-2">
		<Button size="sm" onclick={() => (isDialogOpen = true)}>
			<PlusIcon />
			<span class="hidden lg:inline">Add Expense</span>
		</Button>

		<Button
			variant={showCurrentMonthOnly ? 'default' : 'outline'}
			size="sm"
			onclick={() => (showCurrentMonthOnly = true)}
		>
			Current month
		</Button>
		<Button
			variant={!showCurrentMonthOnly ? 'default' : 'outline'}
			size="sm"
			onclick={() => (showCurrentMonthOnly = false)}
		>
			Show older expenses
		</Button>
		<Button variant="outline" size="sm" onclick={exportAllData}>Export</Button>
		<Input
			bind:files
			type="file"
			placeholder="Import"
			name="importExpenses"
			onchange={importAllData}
		/>
	</div>
{/snippet}

<div class="flex flex-col gap-4 py-4 md:gap-6 md:py-6">
	<div class="px-4 lg:px-6">
		<div class="max-w-4xl space-y-2">
			<h1 class="text-xl font-bold">Expenses</h1>
			<p class="text-sm text-muted-foreground">
				Track your spending with a sortable expense table, category filters, and summary totals.
			</p>
		</div>
	</div>

	<div class="px-4 lg:px-6">
		<DataTable data={filteredExpenses} {columns} controls={dataTableControls} />
	</div>
</div>

<Dialog.Root bind:open={isDialogOpen}>
	<form>
		<Dialog.Content class="sm:max-w-106.25">
			<Dialog.Header>
				<Dialog.Title>Add Expense</Dialog.Title>
				<Dialog.Description>Fill in the details to create a new expense.</Dialog.Description>
			</Dialog.Header>
			<div class="grid gap-4">
				<div class="grid gap-3">
					<Label for="name-1">Name</Label>
					<Input id="name-1" name="name" placeholder="e.g. Grocery" bind:value={name} />
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
				<Button type="submit" onclick={addExpense}>Create Expense</Button>
			</Dialog.Footer>
		</Dialog.Content>
	</form>
</Dialog.Root>
