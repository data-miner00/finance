<script lang="ts">
	import { toast } from 'svelte-sonner';

	import ConfirmAlertDialog from '$lib/components/custom/table-common/confirm-alert-dialog.svelte';
	import RowActionsMenu from '$lib/components/custom/table-common/row-actions-menu.svelte';
	import { Button, buttonVariants } from '$lib/components/ui/button/index.js';
	import * as Dialog from '$lib/components/ui/dialog/index.js';
	import { Input } from '$lib/components/ui/input/index.js';
	import { Label } from '$lib/components/ui/label/index.js';
	import * as Select from '$lib/components/ui/select/index.js';
	import { CURRENCIES, CURRENCY_LABELS, type CurrencyCode } from '$lib/constants/currencies';
	import { deleteIncome, updateIncome } from '$lib/services';
	import { appState } from '$lib/states.svelte';

	let { id }: { id: string } = $props();

	let isEditDialogOpen = $state(false);
	let isDeleteDialogOpen = $state(false);
	let name = $state('');
	let amount = $state(0);
	let currency = $state('');
	let accountId = $state('');

	function openEditDialog() {
		const item = appState.incomes.find((i) => i.id === id);
		if (!item) return;
		name = item.name;
		amount = item.amount;
		currency = item.currency;
		accountId = item.accountId ?? '';
		isEditDialogOpen = true;
	}

	async function saveIncome(event: Event) {
		event.preventDefault();
		try {
			const updated = await updateIncome(id, {
				name,
				amount,
				currency,
				accountId: accountId || undefined
			});
			appState.incomes = appState.incomes.map((i) => (i.id === id ? updated : i));
			isEditDialogOpen = false;
			toast.success('Income updated successfully.');
		} catch (error) {
			toast.error('Failed to update income. ' + error);
		}
	}

	async function confirmDelete() {
		try {
			await deleteIncome(id);
			appState.incomes = appState.incomes.filter((i) => i.id !== id);
			toast.success('Income deleted successfully.');
		} catch (error) {
			toast.error('Failed to delete income. ' + error);
		} finally {
			isDeleteDialogOpen = false;
		}
	}
</script>

<RowActionsMenu
	onEdit={openEditDialog}
	onDelete={() => (isDeleteDialogOpen = true)}
	editLabel="Edit income"
	deleteLabel="Delete income"
	copyId={id}
	copyLabel="Copy income ID"
/>

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
					<Label for="edit-currency">Currency</Label>
					<Select.Root type="single" name="currency" bind:value={currency}>
						<Select.Trigger id="edit-currency" class="w-full">
							{CURRENCY_LABELS[currency as CurrencyCode] ?? currency}
						</Select.Trigger>
						<Select.Content>
							{#each CURRENCIES as code (code)}
								<Select.Item value={code} label={CURRENCY_LABELS[code]}>
									{CURRENCY_LABELS[code]}
								</Select.Item>
							{/each}
						</Select.Content>
					</Select.Root>
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

<ConfirmAlertDialog
	bind:open={isDeleteDialogOpen}
	title="Delete income?"
	description="This action cannot be undone."
	onConfirm={confirmDelete}
/>
