<script lang="ts">
	import { toast } from 'svelte-sonner';

	import ConfirmAlertDialog from '$lib/components/custom/table-common/confirm-alert-dialog.svelte';
	import { Button, buttonVariants } from '$lib/components/ui/button/index.js';
	import * as Dialog from '$lib/components/ui/dialog/index.js';
	import { Label } from '$lib/components/ui/label/index.js';
	import * as Select from '$lib/components/ui/select/index.js';
	import { refreshNotifications } from '$lib/notifications';
	import { deleteExpense, updateExpense } from '$lib/services';
	import type { Expense, UpdateExpenseRequest } from '$lib/services/types';
	import { appState } from '$lib/states.svelte';

	type Props = { selectedIds: string[]; onCleared: () => void };

	let { selectedIds, onCleared }: Props = $props();

	let isDeleteDialogOpen = $state(false);
	let isRecategorizeDialogOpen = $state(false);
	let recategorizeCategoryName = $state('');
	let isSubmitting = $state(false);

	function toUpdateRequest(expense: Expense, categoryName: string): UpdateExpenseRequest {
		return {
			categoryName,
			accountId: expense.accountId ?? undefined,
			name: expense.name,
			description: expense.description ?? undefined,
			amount: expense.amount,
			currency: expense.currency,
			location: expense.location ?? undefined,
			receiptImage: expense.receiptImage ?? undefined,
			agentName: expense.agentName,
			actionedAt: expense.actionedAt
		};
	}

	function reportOutcome(action: string, succeeded: number, failed: number) {
		if (failed === 0) {
			toast.success(`${action} ${succeeded} expense${succeeded === 1 ? '' : 's'}.`);
		} else if (succeeded === 0) {
			toast.error(`Failed to ${action.toLowerCase()} ${failed} expense${failed === 1 ? '' : 's'}.`);
		} else {
			toast.warning(
				`${action} ${succeeded} expense${succeeded === 1 ? '' : 's'}, ${failed} failed.`
			);
		}
	}

	async function confirmBulkDelete() {
		isSubmitting = true;
		const results = await Promise.allSettled(selectedIds.map((id) => deleteExpense(id)));

		const deletedIds = new Set(
			selectedIds.filter((_, index) => results[index].status === 'fulfilled')
		);
		appState.expenses = appState.expenses.filter((e) => !deletedIds.has(e.id));

		isSubmitting = false;
		isDeleteDialogOpen = false;
		reportOutcome('Deleted', deletedIds.size, selectedIds.length - deletedIds.size);
		if (deletedIds.size > 0) onCleared();
	}

	async function confirmBulkRecategorize() {
		if (!recategorizeCategoryName) {
			toast.error('Select a category first.');
			return;
		}

		isSubmitting = true;
		const results = await Promise.allSettled(
			selectedIds.map((id) => {
				const expense = appState.expenses.find((e) => e.id === id);
				if (!expense) return Promise.reject(new Error('Expense not found.'));
				return updateExpense(id, toUpdateRequest(expense, recategorizeCategoryName));
			})
		);

		let succeeded = 0;
		for (const result of results) {
			if (result.status === 'fulfilled') {
				succeeded++;
				const updated = result.value;
				appState.expenses = appState.expenses.map((e) => (e.id === updated.id ? updated : e));
			}
		}

		await refreshNotifications();

		isSubmitting = false;
		isRecategorizeDialogOpen = false;
		reportOutcome('Recategorized', succeeded, selectedIds.length - succeeded);
		if (succeeded > 0) onCleared();
	}

	function openRecategorizeDialog() {
		recategorizeCategoryName = '';
		isRecategorizeDialogOpen = true;
	}

	let selectedCountLabel = $derived(
		`${selectedIds.length} expense${selectedIds.length === 1 ? '' : 's'}`
	);
</script>

{#if selectedIds.length > 0}
	<div class="mb-4 flex items-center gap-3 rounded-md border bg-muted/50 px-4 py-2">
		<span class="text-sm font-medium">
			{selectedIds.length} selected
		</span>
		<Button variant="outline" size="sm" onclick={openRecategorizeDialog}>Recategorize</Button>
		<Button variant="outline" size="sm" onclick={() => (isDeleteDialogOpen = true)}>Delete</Button>
		<Button variant="ghost" size="sm" onclick={onCleared}>Clear selection</Button>
	</div>
{/if}

<Dialog.Root bind:open={isRecategorizeDialogOpen}>
	<Dialog.Content class="sm:max-w-sm">
		<Dialog.Header>
			<Dialog.Title>Recategorize {selectedCountLabel}</Dialog.Title>
			<Dialog.Description>Assign an existing category to all selected expenses.</Dialog.Description>
		</Dialog.Header>
		<div class="grid gap-3">
			<Label for="bulk-recategorize-category">Category</Label>
			<Select.Root type="single" name="categoryName" bind:value={recategorizeCategoryName}>
				<Select.Trigger id="bulk-recategorize-category" class="w-full">
					{recategorizeCategoryName || 'Select a category...'}
				</Select.Trigger>
				<Select.Content>
					{#each appState.categories as category (category.id)}
						<Select.Item value={category.name} label={category.name}>
							{category.name}
						</Select.Item>
					{/each}
				</Select.Content>
			</Select.Root>
		</div>
		<Dialog.Footer>
			<Dialog.Close type="button" class={buttonVariants({ variant: 'outline' })}>
				Cancel
			</Dialog.Close>
			<Button type="button" disabled={isSubmitting} onclick={confirmBulkRecategorize}>
				Recategorize
			</Button>
		</Dialog.Footer>
	</Dialog.Content>
</Dialog.Root>

<ConfirmAlertDialog
	bind:open={isDeleteDialogOpen}
	title="Delete {selectedCountLabel}?"
	description="This action cannot be undone."
	onConfirm={confirmBulkDelete}
/>
