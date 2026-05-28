<script lang="ts">
	import EllipsisIcon from '@lucide/svelte/icons/ellipsis';
	import { Button, buttonVariants } from '$lib/components/ui/button/index.js';
	import * as DropdownMenu from '$lib/components/ui/dropdown-menu/index.js';
	import * as AlertDialog from '$lib/components/ui/alert-dialog/index.js';
	import * as Dialog from '$lib/components/ui/dialog/index.js';
	import { Input } from '$lib/components/ui/input/index.js';
	import { Label } from '$lib/components/ui/label/index.js';
	import { appState } from '$lib/states.svelte';
	import { deletePiggyBank, updatePiggyBank } from '$lib/services';

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
		await updatePiggyBank(id, { name, amount, target });
		appState.piggyBanks = appState.piggyBanks.map((p) =>
			p.id === id ? { ...p, name, amount, target, updatedAt: new Date().toISOString() } : p
		);
		isEditDialogOpen = false;
	}

	async function confirmDelete() {
		await deletePiggyBank(id);
		appState.piggyBanks = appState.piggyBanks.filter((p) => p.id !== id);
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
			<DropdownMenu.Item onclick={openEditDialog}>Edit piggy bank</DropdownMenu.Item>
			<DropdownMenu.Item onclick={() => (isDeleteDialogOpen = true)}>
				Delete piggy bank
			</DropdownMenu.Item>
			<DropdownMenu.Item onclick={() => navigator.clipboard.writeText(id)}>
				Copy piggy bank ID
			</DropdownMenu.Item>
		</DropdownMenu.Group>
		<DropdownMenu.Separator />
		<DropdownMenu.Item>View piggy bank details</DropdownMenu.Item>
	</DropdownMenu.Content>
</DropdownMenu.Root>

<Dialog.Root bind:open={isEditDialogOpen}>
	<form onsubmit={savePiggy}>
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
				<Button type="submit">Update Piggy Bank</Button>
			</Dialog.Footer>
		</Dialog.Content>
	</form>
</Dialog.Root>

<AlertDialog.Root bind:open={isDeleteDialogOpen}>
	<AlertDialog.Content>
		<AlertDialog.Header>
			<AlertDialog.Title>Delete piggy bank?</AlertDialog.Title>
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
