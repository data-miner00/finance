<script lang="ts">
	import type { Expense } from '$lib/services/types';
	import { appState } from '$lib/states.svelte';
	import DataTable from './table/index.svelte';
	import { columns } from './table/column';
	import { onMount } from 'svelte';

	import { Button, buttonVariants } from '$lib/components/ui/button';
	import * as Dialog from '$lib/components/ui/dialog/index.js';
	import { Input } from '$lib/components/ui/input/index.js';
	import { Label } from '$lib/components/ui/label/index.js';
	import { createExpense } from '$lib/services';

	let isDialogOpen = $state(false);
	let showCurrentMonthOnly = $state(true);

	let name = $state('');
	let amount = $state(0);
	let categoryName = $state<string | undefined>(undefined);
	let description = $state('');

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
	});
</script>

<div class="flex flex-col gap-4 py-4 md:gap-6 md:py-6">
	<div class="px-4 lg:px-6">
		<div class="max-w-4xl space-y-2">
			<h1 class="text-xl font-bold">Expenses</h1>
			<p class="text-sm text-muted-foreground">
				Track your spending with a sortable expense table, category filters, and summary totals.
			</p>
		</div>

		<div class="mt-4 flex flex-wrap items-center gap-2">
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
		</div>
	</div>

	<div class="px-4 lg:px-6">
		<DataTable data={filteredExpenses} {columns} />
	</div>
</div>

<Dialog.Root bind:open={isDialogOpen}>
	<form>
		<Dialog.Content class="sm:max-w-[425px]">
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
