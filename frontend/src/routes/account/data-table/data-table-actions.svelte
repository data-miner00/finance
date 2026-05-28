<script lang="ts">
	import EllipsisIcon from '@lucide/svelte/icons/ellipsis';
	import { Button, buttonVariants } from '$lib/components/ui/button/index.js';
	import * as DropdownMenu from '$lib/components/ui/dropdown-menu/index.js';
	import * as AlertDialog from '$lib/components/ui/alert-dialog/index.js';
	import * as Dialog from '$lib/components/ui/dialog/index.js';
	import { Input } from '$lib/components/ui/input/index.js';
	import { Label } from '$lib/components/ui/label/index.js';
	import { appState } from '$lib/states.svelte';
	import { deleteAccount, updateAccount, type AccountType } from '$lib/services';

	let { id }: { id: string } = $props();

	let isEditDialogOpen = $state(false);
	let isDeleteDialogOpen = $state(false);
	let name = $state('');
	let balance = $state(0);
	let accountType = $state<AccountType>(0);

	function openEditDialog() {
		const account = appState.accounts.find((item) => item.id === id);
		if (!account) return;

		name = account.name;
		balance = account.balance;
		accountType = account.type;
		isEditDialogOpen = true;
	}

	async function saveAccount(event: Event) {
		event.preventDefault();
		await updateAccount(id, { name, accountType, balance });
		appState.accounts = appState.accounts.map((account) =>
			account.id === id
				? { ...account, name, balance, updatedAt: new Date().toISOString() }
				: account
		);
		isEditDialogOpen = false;
	}

	async function confirmDelete() {
		await deleteAccount(id);
		appState.accounts = appState.accounts.filter((account) => account.id !== id);
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
			<DropdownMenu.Item onclick={openEditDialog}>Edit account</DropdownMenu.Item>
			<DropdownMenu.Item onclick={() => (isDeleteDialogOpen = true)}>
				Delete account
			</DropdownMenu.Item>
			<DropdownMenu.Item onclick={() => navigator.clipboard.writeText(id)}>
				Copy account ID
			</DropdownMenu.Item>
		</DropdownMenu.Group>
		<DropdownMenu.Separator />
		<DropdownMenu.Item>View account details</DropdownMenu.Item>
	</DropdownMenu.Content>
</DropdownMenu.Root>

<Dialog.Root bind:open={isEditDialogOpen}>
	<form onsubmit={saveAccount}>
		<Dialog.Content class="sm:max-w-lg">
			<Dialog.Header>
				<Dialog.Title>Edit Account</Dialog.Title>
				<Dialog.Description>Update account name and balance.</Dialog.Description>
			</Dialog.Header>
			<div class="grid gap-4">
				<div class="grid gap-3">
					<Label for="edit-name">Name</Label>
					<Input id="edit-name" name="name" placeholder="e.g. Public Bank" bind:value={name} />
				</div>
				<div class="grid gap-3">
					<Label for="edit-balance">Balance</Label>
					<Input
						id="edit-balance"
						name="balance"
						placeholder="0.00"
						type="number"
						step="0.01"
						bind:value={balance}
					/>
				</div>
			</div>
			<Dialog.Footer>
				<Dialog.Close type="button" class={buttonVariants({ variant: 'outline' })}>
					Cancel
				</Dialog.Close>
				<Button type="submit">Update Account</Button>
			</Dialog.Footer>
		</Dialog.Content>
	</form>
</Dialog.Root>

<AlertDialog.Root bind:open={isDeleteDialogOpen}>
	<AlertDialog.Content>
		<AlertDialog.Header>
			<AlertDialog.Title>Delete account?</AlertDialog.Title>
			<AlertDialog.Description>
				This action cannot be undone. The account will be removed permanently.
			</AlertDialog.Description>
		</AlertDialog.Header>
		<AlertDialog.Footer>
			<AlertDialog.Cancel type="button" class={buttonVariants({ variant: 'outline' })}>
				Cancel
			</AlertDialog.Cancel>
			<AlertDialog.Action onclick={confirmDelete}>Delete</AlertDialog.Action>
		</AlertDialog.Footer>
	</AlertDialog.Content>
</AlertDialog.Root>
