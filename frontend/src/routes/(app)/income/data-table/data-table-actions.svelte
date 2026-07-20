<script lang="ts">
	import EllipsisIcon from '@lucide/svelte/icons/ellipsis';

	import * as AlertDialog from '$lib/components/ui/alert-dialog/index.js';
	import { Button, buttonVariants } from '$lib/components/ui/button/index.js';
	import * as Dialog from '$lib/components/ui/dialog/index.js';
	import * as DropdownMenu from '$lib/components/ui/dropdown-menu/index.js';
	import { Input } from '$lib/components/ui/input/index.js';
	import { Label } from '$lib/components/ui/label/index.js';
	import * as Select from '$lib/components/ui/select/index.js';
	import { copyText } from '$lib';
	import { deleteIncome, updateIncome } from '$lib/services';
	import { appState } from '$lib/states.svelte';

	let { id }: { id: string } = $props();

	let isEditDialogOpen = $state(false);
	let isDeleteDialogOpen = $state(false);
	let name = $state('');
	let amount = $state(0);
	let accountId = $state('');

	function openEditDialog() {
		const item = appState.incomes.find((i) => i.id === id);
		if (!item) return;
		name = item.name;
		amount = item.amount;
		accountId = item.accountId ?? '';
		isEditDialogOpen = true;
	}

	async function saveIncome(event: Event) {
		event.preventDefault();
		const accountName = appState.accounts.find((a) => a.id === accountId)?.name;
		await updateIncome(id, { name, amount, accountId: accountId || undefined });
		appState.incomes = appState.incomes.map((i) =>
			i.id === id
				? {
						...i,
						name,
						amount,
						accountId: accountId || null,
						accountName,
						updatedAt: new Date().toISOString()
					}
				: i
		);
		isEditDialogOpen = false;
	}

	async function confirmDelete() {
		await deleteIncome(id);
		appState.incomes = appState.incomes.filter((i) => i.id !== id);
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
			<DropdownMenu.Item onclick={openEditDialog}>Edit income</DropdownMenu.Item>

			<DropdownMenu.Item onclick={() => copyText(id, 'ID copied to clipboard')}>
				Copy income ID
			</DropdownMenu.Item>
		</DropdownMenu.Group>
		<DropdownMenu.Separator />
		<DropdownMenu.Item onclick={() => (isDeleteDialogOpen = true)} variant="destructive">
			Delete income
		</DropdownMenu.Item>
	</DropdownMenu.Content>
</DropdownMenu.Root>

<Dialog.Root bind:open={isEditDialogOpen}>
	<form>
		<Dialog.Content class="sm:max-w-lg">
			<Dialog.Header>
				<Dialog.Title>Edit Income</Dialog.Title>
				<Dialog.Description>Update income name and amount.</Dialog.Description>
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
					<Label for="edit-account">Account</Label>
					<Select.Root type="single" name="accountId" bind:value={accountId}>
						<Select.Trigger id="edit-account" class="w-full">
							{appState.accounts.find((a) => a.id === accountId)?.name ?? 'No account'}
						</Select.Trigger>
						<Select.Content>
							<Select.Item value="" label="No account">No account</Select.Item>
							{#each appState.accounts as account (account.id)}
								<Select.Item value={account.id} label={account.name}>
									{account.name}
								</Select.Item>
							{/each}
						</Select.Content>
					</Select.Root>
				</div>
			</div>
			<Dialog.Footer>
				<Dialog.Close type="button" class={buttonVariants({ variant: 'outline' })}>
					Cancel
				</Dialog.Close>
				<Button type="submit" onclick={saveIncome}>Update Income</Button>
			</Dialog.Footer>
		</Dialog.Content>
	</form>
</Dialog.Root>

<AlertDialog.Root bind:open={isDeleteDialogOpen}>
	<AlertDialog.Content>
		<AlertDialog.Header>
			<AlertDialog.Title>Delete income?</AlertDialog.Title>
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
