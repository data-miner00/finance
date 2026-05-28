<script lang="ts">
	import EllipsisIcon from '@lucide/svelte/icons/ellipsis';
	import { Button, buttonVariants } from '$lib/components/ui/button/index.js';
	import * as DropdownMenu from '$lib/components/ui/dropdown-menu/index.js';
	import * as AlertDialog from '$lib/components/ui/alert-dialog/index.js';
	import * as Dialog from '$lib/components/ui/dialog/index.js';
	import { Input } from '$lib/components/ui/input/index.js';
	import { Label } from '$lib/components/ui/label/index.js';
	import { appState } from '$lib/states.svelte';
	import { deleteRecurringAction, updateRecurringAction } from '$lib/services';

	let { id }: { id: string } = $props();

	let isEditDialogOpen = $state(false);
	let isDeleteDialogOpen = $state(false);
	let name = $state('');
	let amount = $state(0);

	function openEditDialog() {
		const item = appState.recurringActions.find((r) => r.id === id);
		if (!item) return;
		name = item.name;
		amount = item.amount;
		isEditDialogOpen = true;
	}

	async function saveRecurring(event: Event) {
		event.preventDefault();
		await updateRecurringAction(id, { name, amount, isActive: true, recurringAt: '', type: 0 });
		appState.recurringActions = appState.recurringActions.map((r) =>
			r.id === id ? { ...r, name, amount, updatedAt: new Date().toISOString() } : r
		);
		isEditDialogOpen = false;
	}

	async function confirmDelete() {
		await deleteRecurringAction(id);
		appState.recurringActions = appState.recurringActions.filter((r) => r.id !== id);
		isDeleteDialogOpen = false;
	}
</script>

<DropdownMenu.Root>
	<DropdownMenu.Trigger>
		{#snippet child({ props })}
			<Button {...props} variant="ghost" size="icon" class="relative size-8 p-0">
				<span class="sr-only">Open menu</span>
				<EllipsisIcon />
			</Button>
		{/snippet}
	</DropdownMenu.Trigger>
	<DropdownMenu.Content>
		<DropdownMenu.Group>
			<DropdownMenu.Label>Actions</DropdownMenu.Label>
			<DropdownMenu.Item onclick={openEditDialog}>Edit recurring</DropdownMenu.Item>
			<DropdownMenu.Item onclick={() => (isDeleteDialogOpen = true)}>
				Delete recurring
			</DropdownMenu.Item>
			<DropdownMenu.Item onclick={() => navigator.clipboard.writeText(id)}>
				Copy recurring ID
			</DropdownMenu.Item>
		</DropdownMenu.Group>
		<DropdownMenu.Separator />
		<DropdownMenu.Item>View recurring details</DropdownMenu.Item>
	</DropdownMenu.Content>
</DropdownMenu.Root>

<Dialog.Root bind:open={isEditDialogOpen}>
	<form onsubmit={saveRecurring}>
		<Dialog.Content class="sm:max-w-lg">
			<Dialog.Header>
				<Dialog.Title>Edit Recurring</Dialog.Title>
				<Dialog.Description>Update name and amount.</Dialog.Description>
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
				<Button type="submit">Update Recurring</Button>
			</Dialog.Footer>
		</Dialog.Content>
	</form>
</Dialog.Root>

<AlertDialog.Root bind:open={isDeleteDialogOpen}>
	<AlertDialog.Content>
		<AlertDialog.Header>
			<AlertDialog.Title>Delete recurring?</AlertDialog.Title>
			<AlertDialog.Description>This action cannot be undone.</AlertDialog.Description>
		</AlertDialog.Header>
		<AlertDialog.Footer>
			<AlertDialog.Cancel type="button" class={buttonVariants({ variant: 'outline' })}>
				Cancel
			</AlertDialog.Cancel>
			<AlertDialog.Action onclick={confirmDelete}>Delete</AlertDialog.Action>
		</AlertDialog.Footer>
	</AlertDialog.Content>
</AlertDialog.Root>
