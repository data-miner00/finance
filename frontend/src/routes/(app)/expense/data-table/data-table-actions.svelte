<script lang="ts">
	import { CheckIcon, ChevronsUpDownIcon, PlusIcon } from '@lucide/svelte';
	import { tick } from 'svelte';
	import { toast } from 'svelte-sonner';

	import ConfirmAlertDialog from '$lib/components/custom/table-common/confirm-alert-dialog.svelte';
	import RowActionsMenu from '$lib/components/custom/table-common/row-actions-menu.svelte';
	import { Button, buttonVariants } from '$lib/components/ui/button/index.js';
	import * as Command from '$lib/components/ui/command/index.js';
	import * as Dialog from '$lib/components/ui/dialog/index.js';
	import { Input } from '$lib/components/ui/input/index.js';
	import { Label } from '$lib/components/ui/label/index.js';
	import * as Popover from '$lib/components/ui/popover/index.js';
	import * as Select from '$lib/components/ui/select/index.js';
	import { CURRENCIES, CURRENCY_LABELS, type CurrencyCode } from '$lib/constants/currencies';
	import { refreshNotifications } from '$lib/notifications';
	import { categoryNameExists, createCategory, deleteExpense, updateExpense } from '$lib/services';
	import { appState } from '$lib/states.svelte';
	import { cn } from '$lib/utils';

	let { id }: { id: string } = $props();

	let isEditDialogOpen = $state(false);
	let isDeleteDialogOpen = $state(false);
	let name = $state('');
	let categoryName = $state('');
	let categorySearch = $state('');
	let amount = $state(0);
	let currency = $state('');
	let actionedAt = $state('');
	let location = $state('');
	let description = $state<string>();
	let agentName = $state('');
	let accountId = $state('');

	function openEditDialog() {
		const item = appState.expenses.find((e) => e.id === id);
		if (!item) return;
		name = item.name;
		amount = item.amount;
		currency = item.currency;
		categoryName = item.categoryName || '';
		actionedAt = item.actionedAt;
		location = item.location || '';
		description = item.description || undefined;
		agentName = item.agentName || '';
		accountId = item.accountId ?? '';
		isEditDialogOpen = true;
	}

	async function saveExpense(event: Event) {
		event.preventDefault();
		const updated = await updateExpense(id, {
			name,
			amount,
			currency,
			categoryName,
			actionedAt,
			location,
			description,
			agentName,
			accountId: accountId || undefined
		});
		appState.expenses = appState.expenses.map((e) => (e.id === id ? updated : e));
		refreshNotifications();
		toast.success('Expense updated successfully.');
		isEditDialogOpen = false;
	}

	async function confirmDelete() {
		await deleteExpense(id);
		appState.expenses = appState.expenses.filter((e) => e.id !== id);
		isDeleteDialogOpen = false;
		toast.success('Expense deleted successfully.');
	}

	let categoryComboOpen = $state(false);
	let categoryTriggerRef = $state<HTMLButtonElement>(null!);

	function closeAndFocusCategoryTrigger() {
		categoryComboOpen = false;
		tick().then(() => {
			categoryTriggerRef.focus();
		});
	}

	$effect(() => {
		if (categoryComboOpen) {
			categorySearch = categoryName;
		}
	});

	async function createNewCategory() {
		const trimmedName = categorySearch.trim();
		if (!trimmedName) {
			toast.error('Category name is required.');
			return;
		}

		if (categoryNameExists(appState.categories, trimmedName)) {
			toast.error(`A category named "${trimmedName}" already exists.`);
			return;
		}

		try {
			const created = await createCategory({ name: trimmedName });
			appState.categories = [...appState.categories, created];
			categoryName = created.name;
			closeAndFocusCategoryTrigger();
		} catch (error) {
			const isDuplicate = error instanceof Error && error.message.includes('409');
			toast.error(
				isDuplicate
					? `A category named "${trimmedName}" already exists.`
					: 'Failed to create category.'
			);
		}
	}

	let locationComboOpen = $state(false);
	let locationTriggerRef = $state<HTMLButtonElement>(null!);

	function closeAndFocusLocationTrigger() {
		locationComboOpen = false;
		tick().then(() => {
			locationTriggerRef.focus();
		});
	}

	let agentComboOpen = $state(false);
	let agentTriggerRef = $state<HTMLButtonElement>(null!);

	function closeAndFocusAgentTrigger() {
		agentComboOpen = false;
		tick().then(() => {
			agentTriggerRef.focus();
		});
	}
</script>

<RowActionsMenu onEdit={openEditDialog} onDelete={() => (isDeleteDialogOpen = true)} copyId={id} />

<Dialog.Root bind:open={isEditDialogOpen}>
	<form>
		<Dialog.Content class="sm:max-w-lg">
			<Dialog.Header>
				<Dialog.Title>Edit Expense</Dialog.Title>
				<Dialog.Description>Update expense details.</Dialog.Description>
			</Dialog.Header>
			<div class="grid gap-4">
				<div class="grid gap-3">
					<Label for="edit-name">Name</Label>
					<Input id="edit-name" name="name" placeholder="e.g., Texas Chicken" bind:value={name} />
				</div>
				<div class="grid gap-3">
					<Label for="category-1">Category</Label>
					<Popover.Root bind:open={categoryComboOpen}>
						<Popover.Trigger bind:ref={categoryTriggerRef}>
							{#snippet child({ props })}
								<Button
									{...props}
									variant="outline"
									class="justify-between"
									role="combobox"
									aria-expanded={categoryComboOpen}
								>
									{categoryName || 'Select or add a new category...'}
									<ChevronsUpDownIcon class="opacity-50" />
								</Button>
							{/snippet}
						</Popover.Trigger>
						<Popover.Content class="p-0">
							<Command.Root>
								<Command.Input placeholder="Category..." bind:value={categorySearch} />
								<Command.List>
									<Command.Empty>No matching category.</Command.Empty>
									<Command.Group value="categories">
										{#each appState.categories as category (category.id)}
											<Command.Item
												value={category.name}
												onSelect={() => {
													categoryName = category.name;
													closeAndFocusCategoryTrigger();
												}}
											>
												<CheckIcon
													class={cn(categoryName !== category.name && 'text-transparent')}
												/>
												{category.name}
											</Command.Item>
										{/each}
										{#if categorySearch.trim() && !appState.categories.some((category) => category.name.toLowerCase() === categorySearch
														.trim()
														.toLowerCase())}
											<Command.Item value={`create-${categorySearch}`} onSelect={createNewCategory}>
												<PlusIcon class="opacity-70" />
												Create "{categorySearch.trim()}"
											</Command.Item>
										{/if}
									</Command.Group>
								</Command.List>
							</Command.Root>
						</Popover.Content>
					</Popover.Root>
				</div>
				<div class="grid gap-3">
					<Label for="description-1">Description</Label>
					<Input
						id="description-1"
						name="description"
						bind:value={description}
						placeholder="e.g. It was delicious!"
					/>
				</div>
				<div class="grid gap-3">
					<Label for="edit-amount">Amount</Label>
					<Input
						id="edit-amount"
						name="amount"
						type="number"
						placeholder="e.g., 10.00"
						step="0.01"
						bind:value={amount}
					/>
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
				<div class="grid gap-3">
					<Label for="location-1">Location</Label>
					<Popover.Root bind:open={locationComboOpen}>
						<Popover.Trigger bind:ref={locationTriggerRef}>
							{#snippet child({ props })}
								<Button
									{...props}
									variant="outline"
									class="justify-between"
									role="combobox"
									aria-expanded={locationComboOpen}
								>
									{location || 'Select or add a new location'}
									<ChevronsUpDownIcon class="opacity-50" />
								</Button>
							{/snippet}
						</Popover.Trigger>
						<Popover.Content class="p-0">
							<Command.Root>
								<Command.Input placeholder="e.g. Melaka Supermarket" bind:value={location} />
								<Command.List>
									<Command.Empty>{location}</Command.Empty>
									<Command.Group value="frameworks">
										{#each appState.knownLocations as locationSuggestion (locationSuggestion)}
											<Command.Item
												value={locationSuggestion}
												onSelect={() => {
													location = locationSuggestion;
													closeAndFocusLocationTrigger();
												}}
											>
												<CheckIcon
													class={cn(location !== locationSuggestion && 'text-transparent')}
												/>
												{locationSuggestion}
											</Command.Item>
										{/each}
									</Command.Group>
								</Command.List>
							</Command.Root>
						</Popover.Content>
					</Popover.Root>
				</div>
				<div class="grid gap-3">
					<Label for="agentName-1">Agent</Label>
					<Popover.Root bind:open={agentComboOpen}>
						<Popover.Trigger bind:ref={agentTriggerRef}>
							{#snippet child({ props })}
								<Button
									{...props}
									variant="outline"
									class="justify-between"
									role="combobox"
									aria-expanded={agentComboOpen}
								>
									{agentName || 'Select or add a new agent...'}
									<ChevronsUpDownIcon class="opacity-50" />
								</Button>
							{/snippet}
						</Popover.Trigger>
						<Popover.Content class="p-0">
							<Command.Root>
								<Command.Input placeholder="e.g. John Doe" bind:value={agentName} />
								<Command.List>
									<Command.Empty>{agentName}</Command.Empty>
									<Command.Group value="frameworks">
										{#each appState.knownAgents as agent (agent)}
											<Command.Item
												value={agent}
												onSelect={() => {
													agentName = agent;
													closeAndFocusAgentTrigger();
												}}
											>
												<CheckIcon class={cn(agentName !== agent && 'text-transparent')} />
												{agent}
											</Command.Item>
										{/each}
									</Command.Group>
								</Command.List>
							</Command.Root>
						</Popover.Content>
					</Popover.Root>
				</div>
			</div>
			<Dialog.Footer>
				<Dialog.Close type="button" class={buttonVariants({ variant: 'outline' })}>
					Cancel
				</Dialog.Close>
				<Button type="submit" onclick={saveExpense}>Update Expense</Button>
			</Dialog.Footer>
		</Dialog.Content>
	</form>
</Dialog.Root>

<ConfirmAlertDialog
	bind:open={isDeleteDialogOpen}
	title="Delete expense?"
	description="This action cannot be undone."
	onConfirm={confirmDelete}
/>
