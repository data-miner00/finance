<script lang="ts">
	import ConfirmAlertDialog from '$lib/components/custom/table-common/confirm-alert-dialog.svelte';
	import RowActionsMenu from '$lib/components/custom/table-common/row-actions-menu.svelte';
	import { Button, buttonVariants } from '$lib/components/ui/button/index.js';
	import * as Dialog from '$lib/components/ui/dialog/index.js';
	import { Input } from '$lib/components/ui/input/index.js';
	import { Label } from '$lib/components/ui/label/index.js';
	import { deletePiggyBank, updatePiggyBank } from '$lib/services';
	import { appState } from '$lib/states.svelte';

	let { id }: { id: string } = $props();

	let isEditDialogOpen = $state(false);
	let isDeleteDialogOpen = $state(false);
	let name = $state('');
	let amount = $state(0);
	let target = $state(0);

	function openEditDialog() {
		const item = appState.piggyBanks.find((p) => p.id === id);
		if (!item) return;
		name = item.name;
		amount = item.amount;
		target = item.target;
		isEditDialogOpen = true;
	}

	async function savePiggy(event: Event) {
		event.preventDefault();
		const updated = await updatePiggyBank(id, { name, amount, target });
		appState.piggyBanks = appState.piggyBanks.map((p) => (p.id === id ? updated : p));
		isEditDialogOpen = false;
	}

	async function confirmDelete() {
		await deletePiggyBank(id);
		appState.piggyBanks = appState.piggyBanks.filter((p) => p.id !== id);
		isDeleteDialogOpen = false;
	}
</script>

<RowActionsMenu
	onEdit={openEditDialog}
	onDelete={() => (isDeleteDialogOpen = true)}
	editLabel="Edit piggy bank"
	deleteLabel="Delete piggy bank"
	copyId={id}
	copyLabel="Copy piggy bank ID"
/>

<Dialog.Root bind:open={isEditDialogOpen}>
	<form>
		<Dialog.Content class="sm:max-w-lg">
			<Dialog.Header>
				<Dialog.Title>Edit Piggy Bank</Dialog.Title>
				<Dialog.Description>Update name, amount, and target.</Dialog.Description>
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
				<div class="grid gap-3">
					<Label for="edit-target">Target</Label>
					<Input id="edit-target" name="target" type="number" step="0.01" bind:value={target} />
				</div>
			</div>
			<Dialog.Footer>
				<Dialog.Close type="button" class={buttonVariants({ variant: 'outline' })}>
					Cancel
				</Dialog.Close>
				<Button type="submit" onclick={savePiggy}>Update Piggy Bank</Button>
			</Dialog.Footer>
		</Dialog.Content>
	</form>
</Dialog.Root>

<ConfirmAlertDialog
	bind:open={isDeleteDialogOpen}
	title="Delete piggy bank?"
	description="This action cannot be undone."
	onConfirm={confirmDelete}
/>
