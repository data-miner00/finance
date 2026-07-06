<script lang="ts">
	import { CheckIcon, ChevronsUpDownIcon } from '@lucide/svelte';
	import EllipsisIcon from '@lucide/svelte/icons/ellipsis';
	import { tick } from 'svelte';
	import { toast } from 'svelte-sonner';

	import * as AlertDialog from '$lib/components/ui/alert-dialog/index.js';
	import { Button, buttonVariants } from '$lib/components/ui/button/index.js';
	import * as Command from '$lib/components/ui/command/index.js';
	import * as Dialog from '$lib/components/ui/dialog/index.js';
	import * as DropdownMenu from '$lib/components/ui/dropdown-menu/index.js';
	import { Input } from '$lib/components/ui/input/index.js';
	import { Label } from '$lib/components/ui/label/index.js';
	import * as Popover from '$lib/components/ui/popover/index.js';
	import { deleteExpense, updateExpense } from '$lib/services';
	import { appState } from '$lib/states.svelte';
	import { cn } from '$lib/utils';

	let { id }: { id: string } = $props();

	let isEditDialogOpen = $state(false);
	let isDeleteDialogOpen = $state(false);
	let name = $state('');
	let categoryName = $state<string>();
	let amount = $state(0);
	let actionedAt = $state('');
	let location = $state<string>();
	let description = $state<string>();
	let agentName = $state<string>();

	function openEditDialog() {
		const item = appState.expenses.find((e) => e.id === id);
		if (!item) return;
		name = item.name;
		amount = item.amount;
		categoryName = item.categoryName;
		actionedAt = item.actionedAt;
		location = item.location || undefined;
		description = item.description || undefined;
		agentName = item.agentName || undefined;
		isEditDialogOpen = true;
	}

	async function saveExpense(event: Event) {
		event.preventDefault();
		await updateExpense(id, {
			name,
			amount,
			categoryName,
			actionedAt,
			location,
			description,
			agentName
		});
		appState.expenses = appState.expenses.map((e) =>
			e.id === id
				? {
						...e,
						name,
						amount,
						categoryName,
						actionedAt,
						location,
						description,
						agentName,
						updatedAt: new Date().toISOString()
					}
				: e
		);
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

	let locationComboOpen = $state(false);
	let locationTriggerRef = $state<HTMLButtonElement>(null!);

	function closeAndFocusLocationTrigger() {
		locationComboOpen = false;
		tick().then(() => {
			locationTriggerRef.focus();
		});
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
								<Command.Input placeholder="Category..." bind:value={categoryName} />
								<Command.List>
									<Command.Empty>{categoryName}</Command.Empty>
									<Command.Group value="frameworks">
										{#each appState.categories as category (category)}
											<Command.Item
												value={category}
												onSelect={() => {
													categoryName = category;
													closeAndFocusCategoryTrigger();
												}}
											>
												<CheckIcon class={cn(categoryName !== category && 'text-transparent')} />
												{category}
											</Command.Item>
										{/each}
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
					<Input
						id="agentName-1"
						name="agentName"
						bind:value={agentName}
						placeholder="e.g. John Doe"
					/>
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

<AlertDialog.Root bind:open={isDeleteDialogOpen}>
	<AlertDialog.Content>
		<AlertDialog.Header>
			<AlertDialog.Title>Delete expense?</AlertDialog.Title>
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
