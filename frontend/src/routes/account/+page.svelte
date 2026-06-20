<script lang="ts">
	import { onMount } from 'svelte';

	import DataTable from '$lib/components/data-table-revamp.svelte';
	import { Button, buttonVariants } from '$lib/components/ui/button';
	import * as Dialog from '$lib/components/ui/dialog/index.js';
	import { Input } from '$lib/components/ui/input/index.js';
	import { Label } from '$lib/components/ui/label/index.js';
	import * as Select from '$lib/components/ui/select/index.js';
	import { type AccountType, createAccount } from '$lib/services';
	import { appState } from '$lib/states.svelte';

	import { columns } from './data-table/column';

	let isDialogOpen = $state(false);

	let name = $state('');
	let balance = $state(0);
	let accountType = $state<AccountType>(0);

	onMount(() => {
		appState.pageTitle = 'Accounts';
	});

	async function addAccount() {
		const account = await createAccount({ name, accountType, balance });

		appState.accounts = [...appState.accounts, account];
		name = '';
		balance = 0;
		accountType = 0;
		isDialogOpen = false;
	}
</script>

{#snippet dataTableControls()}
	<Button size="sm" onclick={() => (isDialogOpen = true)}>Create Account</Button>
{/snippet}

<div class="flex flex-col gap-4 py-4 md:gap-6 md:py-6">
	<div class="px-4 lg:px-6">
		<div class="max-w-4xl space-y-2">
			<h1 class="text-xl font-bold">Accounts</h1>
			<p class="text-sm text-muted-foreground">
				Manage your financial accounts and view their balances.
			</p>
		</div>
	</div>

	<div class="px-4 lg:px-6">
		<DataTable data={appState.accounts} {columns} controls={dataTableControls} />
	</div>
</div>

<Dialog.Root bind:open={isDialogOpen}>
	<form>
		<Dialog.Content class="sm:max-w-106.25">
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
					<Label for="type-1">Type</Label>
					<Select.Root
						name="type-1"
						type="single"
						onValueChange={(value: string) => (accountType = parseInt(value) as AccountType)}
					>
						<Select.Trigger size="sm" class={'w-full' + buttonVariants({ variant: 'outline' })}>
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
						<Select.Content>
							<Select.Item value="0">Savings</Select.Item>
							<Select.Item value="1">E-Wallet</Select.Item>
							<Select.Item value="2">Cash</Select.Item>
							<Select.Item value="3">App</Select.Item>
							<Select.Item value="4">Credit Card</Select.Item>
						</Select.Content>
					</Select.Root>
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
