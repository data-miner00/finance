<script lang="ts">
	import { toast } from 'svelte-sonner';

	import ConfirmAlertDialog from '$lib/components/custom/table-common/confirm-alert-dialog.svelte';
	import RowActionsMenu from '$lib/components/custom/table-common/row-actions-menu.svelte';
	import { Button, buttonVariants } from '$lib/components/ui/button/index.js';
	import * as Dialog from '$lib/components/ui/dialog/index.js';
	import { Input } from '$lib/components/ui/input/index.js';
	import { Label } from '$lib/components/ui/label/index.js';
	import { updateCategory } from '$lib/services';
	import { appState } from '$lib/states.svelte';

	let { id }: { id: string } = $props();

	let isEditDialogOpen = $state(false);
	let isRemoveDialogOpen = $state(false);
	let amount = $state(0);

	function openEditDialog() {
		const category = appState.categories.find((item) => item.id === id);
		if (!category) return;

		amount = category.budgetAmount ?? 0;
		isEditDialogOpen = true;
	}

	async function saveBudget(event: Event) {
		event.preventDefault();
		const category = appState.categories.find((item) => item.id === id);
		if (!category) return;

		try {
			const updated = await updateCategory(id, {
				name: category.name,
				color: category.color,
				icon: category.icon,
				budgetAmount: amount
			});
			appState.categories = appState.categories.map((c) => (c.id === id ? updated : c));
			isEditDialogOpen = false;
			toast.success('Budget updated successfully.');
		} catch {
			toast.error('Failed to update budget.');
		}
	}

	async function confirmRemove() {
		const category = appState.categories.find((item) => item.id === id);
		if (!category) return;

		try {
			const updated = await updateCategory(id, {
				name: category.name,
				color: category.color,
				icon: category.icon,
				budgetAmount: null
			});
			appState.categories = appState.categories.map((c) => (c.id === id ? updated : c));
			isRemoveDialogOpen = false;
			toast.success('Budget removed.');
		} catch {
			isRemoveDialogOpen = false;
			toast.error('Failed to remove budget.');
		}
	}
</script>

<RowActionsMenu
	onEdit={openEditDialog}
	onDelete={() => (isRemoveDialogOpen = true)}
	deleteLabel="Remove Budget"
/>

<Dialog.Root bind:open={isEditDialogOpen}>
	<form>
		<Dialog.Content class="sm:max-w-lg">
			<Dialog.Header>
				<Dialog.Title>Edit Budget</Dialog.Title>
				<Dialog.Description>Update the monthly budget amount for this category.</Dialog.Description>
			</Dialog.Header>
			<div class="grid gap-4">
				<div class="grid gap-3">
					<Label for="edit-budget-amount">Amount</Label>
					<Input
						id="edit-budget-amount"
						name="amount"
						type="number"
						step="0.01"
						placeholder="0.00"
						bind:value={amount}
					/>
				</div>
			</div>
			<Dialog.Footer>
				<Dialog.Close type="button" class={buttonVariants({ variant: 'outline' })}>
					Cancel
				</Dialog.Close>
				<Button type="submit" onclick={saveBudget}>Update Budget</Button>
			</Dialog.Footer>
		</Dialog.Content>
	</form>
</Dialog.Root>

<ConfirmAlertDialog
	bind:open={isRemoveDialogOpen}
	title="Remove budget?"
	description="This clears the monthly budget for this category. The category itself is not deleted."
	confirmLabel="Remove"
	onConfirm={confirmRemove}
/>
