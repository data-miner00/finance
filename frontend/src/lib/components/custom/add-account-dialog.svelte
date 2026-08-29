<script lang="ts">
	import { toast } from 'svelte-sonner';

	import { Button, buttonVariants } from '$lib/components/ui/button';
	import * as Dialog from '$lib/components/ui/dialog/index.js';
	import { Input } from '$lib/components/ui/input/index.js';
	import { Label } from '$lib/components/ui/label/index.js';
	import * as Select from '$lib/components/ui/select/index.js';
	import { CURRENCIES, CURRENCY_LABELS, type CurrencyCode } from '$lib/constants/currencies';
	import { type AccountType, createAccount } from '$lib/services';
	import { appState } from '$lib/states.svelte';

	let name = $state('');
	let balance = $state(0);
	let accountType = $state<AccountType>(0);
	let currency = $state(appState.settings.defaultCurrency);
	let annualSpendTarget = $state<number | null>(null);

	async function addAccount() {
		try {
			const account = await createAccount({
				name,
				accountType,
				balance,
				currency,
				annualSpendTarget: annualSpendTarget || undefined
			});

			appState.accounts = [...appState.accounts, account];
			name = '';
			balance = 0;
			accountType = 0;
			currency = appState.settings.defaultCurrency;
			annualSpendTarget = null;
			appState.openAddDialog = null;

			toast.success('Account created successfully.');
		} catch (error) {
			toast.error('Failed to create account. ' + error);
		}
	}
</script>

<Dialog.Root
	bind:open={
		() => appState.openAddDialog === 'account',
		(v) => (appState.openAddDialog = v ? 'account' : null)
	}
>
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
						<Select.Trigger size="sm" class="w-full">
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
				<div class="grid gap-3">
					<Label for="currency-1">Currency</Label>
					<Select.Root type="single" name="currency" bind:value={currency}>
						<Select.Trigger id="currency-1" class="w-full">
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
					<Label for="annual-spend-target-1">Annual Spend Target (Optional)</Label>
					<Input
						id="annual-spend-target-1"
						name="annualSpendTarget"
						placeholder="e.g. 12000.00"
						type="number"
						step="0.01"
						min="0"
						bind:value={annualSpendTarget}
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
