<script lang="ts">
	import ConfirmAlertDialog from '$lib/components/custom/table-common/confirm-alert-dialog.svelte';
	import RowActionsMenu from '$lib/components/custom/table-common/row-actions-menu.svelte';
	import { Button, buttonVariants } from '$lib/components/ui/button/index.js';
	import * as Dialog from '$lib/components/ui/dialog/index.js';
	import { Input } from '$lib/components/ui/input/index.js';
	import { Label } from '$lib/components/ui/label/index.js';
	import { deleteTax, updateTax } from '$lib/services';
	import { appState } from '$lib/states.svelte';

	let { id }: { id: string } = $props();

	let isEditDialogOpen = $state(false);
	let isDeleteDialogOpen = $state(false);
	let name = $state('');
	let amount = $state(0);

	function openEditDialog() {
		const item = appState.taxes.find((i) => i.id === id);
		if (!item) return;
		name = item.name;
		amount = item.amount;
		isEditDialogOpen = true;
	}

	async function saveTax(event: Event) {
		event.preventDefault();
		const updated = await updateTax(id, { name, amount });
		appState.taxes = appState.taxes.map((i) => (i.id === id ? updated : i));
		isEditDialogOpen = false;
	}

	async function confirmDelete() {
		await deleteTax(id);
		appState.taxes = appState.taxes.filter((i) => i.id !== id);
		isDeleteDialogOpen = false;
	}
</script>

<RowActionsMenu onEdit={openEditDialog} onDelete={() => (isDeleteDialogOpen = true)} copyId={id} />

<Dialog.Root bind:open={isEditDialogOpen}>
	<form>
		<Dialog.Content class="sm:max-w-lg">
			<Dialog.Header>
				<Dialog.Title>Edit Tax</Dialog.Title>
				<Dialog.Description>Update tax name and amount.</Dialog.Description>
			</Dialog.Header>
			<div class="grid gap-4">
				<div class="grid gap-3">
					<Label for="edit-name">Name</Label>
					<Input id="edit-name" name="name" bind:value={name} />
				</div>
				<div class="grid gap-3">
					<Label for="edit-amount">Amount</Label>
					<Input id="edit-amount" name="amount" type="number" step="0.01" bind:value={amount} />
				</div>
			</div>
			<Dialog.Footer>
				<Dialog.Close type="button" class={buttonVariants({ variant: 'outline' })}>
					Cancel
				</Dialog.Close>
				<Button type="submit" onclick={saveTax}>Update Tax</Button>
			</Dialog.Footer>
		</Dialog.Content>
	</form>
</Dialog.Root>

<ConfirmAlertDialog
	bind:open={isDeleteDialogOpen}
	title="Delete tax record?"
	description="This action cannot be undone."
	onConfirm={confirmDelete}
/>
