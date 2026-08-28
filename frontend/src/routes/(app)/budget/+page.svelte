<script lang="ts">
	import PlusIcon from '@tabler/icons-svelte/icons/plus';
	import { onMount } from 'svelte';
	import { toast } from 'svelte-sonner';

	import { formatCurrency } from '$lib';
	import { computeBudgetRows } from '$lib/budget';
	import DataTable from '$lib/components/data-table-revamp.svelte';
	import StatCard from '$lib/components/stat-card.svelte';
	import { Button, buttonVariants } from '$lib/components/ui/button/index.js';
	import * as Dialog from '$lib/components/ui/dialog/index.js';
	import { Input } from '$lib/components/ui/input/index.js';
	import { Label } from '$lib/components/ui/label/index.js';
	import * as Select from '$lib/components/ui/select/index.js';
	import { updateCategory } from '$lib/services';
	import { appState } from '$lib/states.svelte';

	import { type BudgetRow, columns } from './data-table/column';

	onMount(() => {
		appState.pageTitle = 'Budget';
		appState.currentPage = 'budget';
	});

	let isSetBudgetDialogOpen = $state(false);
	let newBudgetCategoryId = $state('');
	let newBudgetAmount = $state(0);

	let unbudgetedCategories = $derived(appState.categories.filter((c) => c.budgetAmount == null));

	let budgetRows = $derived<BudgetRow[]>(computeBudgetRows(appState.categories, appState.expenses));

	let totalBudgeted = $derived(budgetRows.reduce((sum, row) => sum + row.budgetAmount, 0));
	let totalSpent = $derived(budgetRows.reduce((sum, row) => sum + row.spent, 0));
	let totalRemaining = $derived(totalBudgeted - totalSpent);

	function openSetBudgetDialog() {
		newBudgetCategoryId = '';
		newBudgetAmount = 0;
		isSetBudgetDialogOpen = true;
	}

	async function setBudget(event: Event) {
		event.preventDefault();
		const category = appState.categories.find((c) => c.id === newBudgetCategoryId);
		if (!category) {
			toast.error('Select a category.');
			return;
		}

		try {
			const updated = await updateCategory(category.id, {
				name: category.name,
				color: category.color,
				icon: category.icon,
				budgetAmount: newBudgetAmount
			});
			appState.categories = appState.categories.map((c) => (c.id === category.id ? updated : c));
			isSetBudgetDialogOpen = false;
			toast.success(`Budget set for "${category.name}".`);
		} catch {
			toast.error('Failed to set budget.');
		}
	}
</script>

{#snippet dataTableControls()}
	<Button size="sm" onclick={openSetBudgetDialog} disabled={unbudgetedCategories.length === 0}>
		<PlusIcon /> Set Budget
	</Button>
{/snippet}

<div class="flex flex-col gap-4 py-4 md:gap-6 md:py-6">
	<div class="px-4 lg:px-6">
		<div class="max-w-4xl space-y-2">
			<h1 class="text-xl font-bold">Budget</h1>
			<p class="text-sm text-muted-foreground">
				Set a monthly spending limit per category and track actual spend against it.
			</p>
		</div>
	</div>

	<div
		class="grid grid-cols-1 gap-4 px-4 *:data-[slot=card]:bg-linear-to-t *:data-[slot=card]:from-primary/5 *:data-[slot=card]:to-card *:data-[slot=card]:shadow-xs lg:px-6 @xl/main:grid-cols-2 @5xl/main:grid-cols-3 dark:*:data-[slot=card]:bg-card"
	>
		<StatCard
			description="Total Budgeted"
			value={formatCurrency(totalBudgeted)}
			badgeText="Monthly"
			trendDirection="up"
			footerText="Across budgeted categories"
			footerSubText="Recurs every month"
		/>

		<StatCard
			description="Total Spent This Month"
			value={formatCurrency(totalSpent)}
			badgeText="Actual"
			trendDirection={totalSpent > totalBudgeted ? 'down' : 'up'}
			footerText="Actual spend so far"
			footerSubText="Budgeted categories only"
		/>

		<StatCard
			description="Remaining"
			value={formatCurrency(totalRemaining)}
			badgeText="Remaining"
			trendDirection={totalRemaining < 0 ? 'down' : 'up'}
			footerText={totalRemaining < 0 ? 'Over budget' : 'Left to spend'}
			footerSubText="This month"
		/>
	</div>

	<div class="px-4 lg:px-6">
		<DataTable data={budgetRows} {columns} controls={dataTableControls} isPaginated={false} />
	</div>
</div>

<Dialog.Root bind:open={isSetBudgetDialogOpen}>
	<form>
		<Dialog.Content class="sm:max-w-lg">
			<Dialog.Header>
				<Dialog.Title>Set Budget</Dialog.Title>
				<Dialog.Description>Choose a category and its monthly budget amount.</Dialog.Description>
			</Dialog.Header>
			<div class="grid gap-4">
				<div class="grid gap-3">
					<Label for="budget-category">Category</Label>
					<Select.Root type="single" name="categoryId" bind:value={newBudgetCategoryId}>
						<Select.Trigger id="budget-category" class="w-full">
							{unbudgetedCategories.find((c) => c.id === newBudgetCategoryId)?.name ??
								'Select a category...'}
						</Select.Trigger>
						<Select.Content>
							{#each unbudgetedCategories as category (category.id)}
								<Select.Item value={category.id} label={category.name}>
									{category.name}
								</Select.Item>
							{/each}
						</Select.Content>
					</Select.Root>
				</div>
				<div class="grid gap-3">
					<Label for="budget-amount">Amount</Label>
					<Input
						id="budget-amount"
						name="amount"
						type="number"
						step="0.01"
						placeholder="0.00"
						bind:value={newBudgetAmount}
					/>
				</div>
			</div>
			<Dialog.Footer>
				<Dialog.Close type="button" class={buttonVariants({ variant: 'outline' })}>
					Cancel
				</Dialog.Close>
				<Button type="submit" onclick={setBudget} disabled={!newBudgetCategoryId}>
					Set Budget
				</Button>
			</Dialog.Footer>
		</Dialog.Content>
	</form>
</Dialog.Root>
