<script lang="ts">
	import { toast } from 'svelte-sonner';

	import ConfirmAlertDialog from '$lib/components/custom/table-common/confirm-alert-dialog.svelte';
	import RowActionsMenu from '$lib/components/custom/table-common/row-actions-menu.svelte';
	import { Button, buttonVariants } from '$lib/components/ui/button/index.js';
	import * as Dialog from '$lib/components/ui/dialog/index.js';
	import { Input } from '$lib/components/ui/input/index.js';
	import { Label } from '$lib/components/ui/label/index.js';
	import * as Select from '$lib/components/ui/select/index.js';
	import { type AccountType, deleteAccount, updateAccount } from '$lib/services';
	import { appState } from '$lib/states.svelte';

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
		try {
			const updated = await updateAccount(id, { name, accountType, balance });
			appState.accounts = appState.accounts.map((account) =>
				account.id === id ? updated : account
			);
			isEditDialogOpen = false;
			toast.success('Account updated successfully.');
		} catch (error) {
			toast.error('Failed to update account. ' + error);
		}
	}

	async function confirmDelete() {
		try {
			await deleteAccount(id);
			appState.accounts = appState.accounts.filter((account) => account.id !== id);
			toast.success('Account deleted successfully.');
		} catch (error) {
			toast.error('Failed to delete account. ' + error);
		} finally {
			isDeleteDialogOpen = false;
		}
	}
</script>

<RowActionsMenu
	onEdit={openEditDialog}
	onDelete={() => (isDeleteDialogOpen = true)}
	editLabel="Edit account"
	deleteLabel="Delete account"
	copyId={id}
	copyLabel="Copy account ID"
/>

<Dialog.Root bind:open={isEditDialogOpen}>
	<form>
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
					<Label for="edit-type">Type</Label>
					<Select.Root
						type="single"
						value={accountType.toString()}
						onValueChange={(value) => (accountType = parseInt(value))}
					>
						<Select.Trigger
							size="sm"
							class="flex w-full **:data-[slot=select-value]:block **:data-[slot=select-value]:truncate @[767px]/card:hidden"
							aria-label="Select a value"
						>
							<span data-slot="select-value">
								{accountType === 0
									? 'Savings'
									: accountType === 1
										? 'E-Wallet'
										: accountType === 2
											? 'Cash'
											: accountType === 3
												? 'App'
												: 'Credit Card'}
							</span>
						</Select.Trigger>
						<Select.Content class="rounded-xl">
							<Select.Item value="0" class="rounded-lg">Savings</Select.Item>
							<Select.Item value="1" class="rounded-lg">E-Wallet</Select.Item>
							<Select.Item value="2" class="rounded-lg">Cash</Select.Item>
							<Select.Item value="3" class="rounded-lg">App</Select.Item>
							<Select.Item value="4" class="rounded-lg">Credit Card</Select.Item>
						</Select.Content>
					</Select.Root>
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
				<Button type="submit" onclick={saveAccount}>Update Account</Button>
			</Dialog.Footer>
		</Dialog.Content>
	</form>
</Dialog.Root>

<ConfirmAlertDialog
	bind:open={isDeleteDialogOpen}
	title="Delete account?"
	description="This action cannot be undone. The account will be removed permanently."
	onConfirm={confirmDelete}
/>
