<script lang="ts">
	import DataTable from './data-table/index.svelte';
	import { appState } from '$lib/states.svelte';
	import { columns } from './data-table/column';

	import { Button, buttonVariants } from '$lib/components/ui/button';
	import * as Dialog from '$lib/components/ui/dialog/index.js';
	import { Input } from '$lib/components/ui/input/index.js';
	import { Label } from '$lib/components/ui/label/index.js';
	import { createAccount, type AccountType } from '$lib/services';

	let isDialogOpen = $state(false);

	let name = $state('');
	let balance = $state(0);
	let accountType = $state<AccountType>(0);

	async function addAccount() {
		await createAccount({ name, accountType, balance });

		appState.accounts.push({
			name,
			type: accountType,
			balance,
			id: crypto.randomUUID(),
			createdAt: new Date().toISOString(),
			updatedAt: new Date().toISOString()
		});
		isDialogOpen = false;
	}
</script>

<div class="flex flex-col gap-4 py-4 md:gap-6 md:py-6">
	<div class="px-4 lg:px-6">
		<div class="max-w-4xl space-y-2">
			<h1 class="text-xl font-bold">Accounts</h1>
			<p class="text-sm text-muted-foreground">
				Manage your financial accounts and view their balances.
			</p>
		</div>

		<Button onclick={() => (isDialogOpen = true)}>Create Account</Button>
	</div>

	<div class="px-4 lg:px-6">
		<DataTable data={appState.accounts} {columns} />
	</div>
</div>

<Dialog.Root bind:open={isDialogOpen}>
	<form>
		<Dialog.Content class="sm:max-w-[425px]">
			<Dialog.Header>
				<Dialog.Title>Add Account</Dialog.Title>
				<Dialog.Description>Fill in the details to create a new account.</Dialog.Description>
			</Dialog.Header>
			<div class="grid gap-4">
				<div class="grid gap-3">
					<Label for="name-1">Name</Label>
					<Input id="name-1" name="name" placeholder="e.g. Public Bank" bind:value={name} />
				</div>
				<div class="grid gap-3">
					<Label for="balance-1">Balance</Label>
					<Input
						id="balance-1"
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
				<Button type="submit" onclick={addAccount}>Create Account</Button>
			</Dialog.Footer>
		</Dialog.Content>
	</form>
</Dialog.Root>
