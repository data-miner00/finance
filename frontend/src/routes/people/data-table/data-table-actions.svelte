<script lang="ts">
	import EllipsisIcon from '@lucide/svelte/icons/ellipsis';

	import * as AlertDialog from '$lib/components/ui/alert-dialog/index.js';
	import { Button, buttonVariants } from '$lib/components/ui/button/index.js';
	import * as Dialog from '$lib/components/ui/dialog/index.js';
	import * as DropdownMenu from '$lib/components/ui/dropdown-menu/index.js';
	import { Input } from '$lib/components/ui/input/index.js';
	import { Label } from '$lib/components/ui/label/index.js';
	import { deletePerson, updatePerson } from '$lib/services';
	import { appState } from '$lib/states.svelte';

	let { id }: { id: string } = $props();

	let isEditDialogOpen = $state(false);
	let isDeleteDialogOpen = $state(false);
	let name = $state('');
	let alias = $state('');
	let description = $state('');

	function openEditDialog() {
		const item = appState.people.find((i) => i.id === id);
		if (!item) return;
		name = item.name;
		alias = item.alias || '';
		description = item.description || '';
		isEditDialogOpen = true;
	}

	async function saveUpdate(event: Event) {
		event.preventDefault();
		await updatePerson(id, { name, alias, description });
		appState.people = appState.people.map((i) =>
			i.id === id ? { ...i, name, alias, description, updatedAt: new Date().toISOString() } : i
		);
		isEditDialogOpen = false;
	}

	async function confirmDelete() {
		await deletePerson(id);
		appState.people = appState.people.filter((i) => i.id !== id);
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
			<DropdownMenu.Item onclick={openEditDialog}>Edit</DropdownMenu.Item>

			<DropdownMenu.Item onclick={() => navigator.clipboard.writeText(id)}>
				Copy ID
			</DropdownMenu.Item>
		</DropdownMenu.Group>
		<DropdownMenu.Separator />
		<DropdownMenu.Item onclick={() => (isDeleteDialogOpen = true)} variant="destructive">
			Delete
		</DropdownMenu.Item>
	</DropdownMenu.Content>
</DropdownMenu.Root>

<Dialog.Root bind:open={isEditDialogOpen}>
	<form>
		<Dialog.Content class="sm:max-w-lg">
			<Dialog.Header>
				<Dialog.Title>Edit Person</Dialog.Title>
				<Dialog.Description>Update person details.</Dialog.Description>
			</Dialog.Header>
			<div class="grid gap-4">
				<div class="grid gap-3">
					<Label for="edit-name">Name</Label>
					<Input id="edit-name" name="name" bind:value={name} />
				</div>
				<div class="grid gap-3">
					<Label for="edit-alias">Alias</Label>
					<Input id="edit-alias" name="alias" bind:value={alias} />
				</div>
				<div class="grid gap-3">
					<Label for="edit-description">Description</Label>
					<Input id="edit-description" name="description" bind:value={description} />
				</div>
			</div>
			<Dialog.Footer>
				<Dialog.Close type="button" class={buttonVariants({ variant: 'outline' })}>
					Cancel
				</Dialog.Close>
				<Button type="submit" onclick={saveUpdate}>Update Person</Button>
			</Dialog.Footer>
		</Dialog.Content>
	</form>
</Dialog.Root>

<AlertDialog.Root bind:open={isDeleteDialogOpen}>
	<AlertDialog.Content>
		<AlertDialog.Header>
			<AlertDialog.Title>Delete person?</AlertDialog.Title>
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
