<script lang="ts">
	import EllipsisIcon from '@lucide/svelte/icons/ellipsis';
	import { Button, buttonVariants } from '$lib/components/ui/button/index.js';
	import * as DropdownMenu from '$lib/components/ui/dropdown-menu/index.js';
	import * as AlertDialog from '$lib/components/ui/alert-dialog/index.js';
	import * as Dialog from '$lib/components/ui/dialog/index.js';
	import { Input } from '$lib/components/ui/input/index.js';
	import { Label } from '$lib/components/ui/label/index.js';
	import { appState } from '$lib/states.svelte';
	import { deleteRecurringAction, updateRecurringAction } from '$lib/services';
	import * as Select from '$lib/components/ui/select/index.js';

	let { id }: { id: string } = $props();

	let isEditDialogOpen = $state(false);
	let isDeleteDialogOpen = $state(false);
	let name = $state('');
	let amount = $state(0);
	let startAt = $state('');
	let recurrenceType = $state('Monthly');
	let intervalValue = $state(1);
	let dayOfMonth = $state<number | null>(null);
	let description = $state('');
	let isActive = $state(true);

	function openEditDialog() {
		const item = appState.recurringActions.find((r) => r.id === id);
		if (!item) return;
		name = item.name;
		amount = item.amount;
		startAt = item.startAt;
		recurrenceType = item.recurrenceType;
		intervalValue = item.intervalValue;
		dayOfMonth = item.dayOfMonth || null;
		description = item.description || '';
		isActive = item.isActive;
		isEditDialogOpen = true;
	}

	async function saveRecurring(event: Event) {
		event.preventDefault();
		await updateRecurringAction(id, {
			name,
			amount,
			startAt,
			recurrenceType,
			intervalValue,
			dayOfMonth,
			description,
			isActive
		});
		appState.recurringActions = appState.recurringActions.map((r) =>
			r.id === id
				? {
						...r,
						name,
						amount,
						startAt,
						recurrenceType,
						intervalValue,
						dayOfMonth,
						description,
						isActive,
						updatedAt: new Date().toISOString()
					}
				: r
		);
		isEditDialogOpen = false;
	}

	async function confirmDelete() {
		await deleteRecurringAction(id);
		appState.recurringActions = appState.recurringActions.filter((r) => r.id !== id);
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
			<DropdownMenu.Item onclick={openEditDialog}>Edit recurring</DropdownMenu.Item>
			<DropdownMenu.Item onclick={() => (isDeleteDialogOpen = true)}>
				Delete recurring
			</DropdownMenu.Item>
			<DropdownMenu.Item onclick={() => navigator.clipboard.writeText(id)}>
				Copy recurring ID
			</DropdownMenu.Item>
		</DropdownMenu.Group>
		<DropdownMenu.Separator />
		<DropdownMenu.Item>View recurring details</DropdownMenu.Item>
	</DropdownMenu.Content>
</DropdownMenu.Root>

<Dialog.Root bind:open={isEditDialogOpen}>
	<form onsubmit={saveRecurring}>
		<Dialog.Content class="sm:max-w-lg">
			<Dialog.Header>
				<Dialog.Title>Edit Recurring</Dialog.Title>
				<Dialog.Description>Update the details for this recurring action.</Dialog.Description>
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
					<Label for="edit-startAt">Start Date</Label>
					<Input id="edit-startAt" name="startAt" type="date" bind:value={startAt} />
				</div>
				<div class="grid gap-3">
					<Label for="edit-recurrenceType">Recurrence Type</Label>
					<Select.Root type="single" name="recurrenceType" bind:value={recurrenceType}>
						<Select.Trigger id="edit-recurrenceType" class="w-full">
							{recurrenceType}
						</Select.Trigger>
						<Select.Content>
							<Select.Item value="Daily" label="Daily">Daily</Select.Item>
							<Select.Item value="Weekly" label="Weekly">Weekly</Select.Item>
							<Select.Item value="Monthly" label="Monthly">Monthly</Select.Item>
							<Select.Item value="Yearly" label="Yearly">Yearly</Select.Item>
						</Select.Content>
					</Select.Root>
				</div>
				<div class="grid gap-3">
					<Label for="edit-intervalValue">Interval</Label>
					<Input
						id="edit-intervalValue"
						name="intervalValue"
						type="number"
						min="1"
						bind:value={intervalValue}
					/>
				</div>
				<div class="grid gap-3">
					<Label for="edit-dayOfMonth">Day of Month (optional)</Label>
					<Input
						id="edit-dayOfMonth"
						name="dayOfMonth"
						type="number"
						min="1"
						max="31"
						placeholder="Leave empty to use start date's day"
						bind:value={dayOfMonth}
					/>
				</div>
				<div class="grid gap-3">
					<Label for="edit-description">Description</Label>
					<Input
						id="edit-description"
						name="description"
						placeholder="Optional description"
						bind:value={description}
					/>
				</div>
			</div>
			<Dialog.Footer>
				<Dialog.Close type="button" class={buttonVariants({ variant: 'outline' })}>
					Cancel
				</Dialog.Close>
				<Button type="submit">Update Recurring</Button>
			</Dialog.Footer>
		</Dialog.Content>
	</form>
</Dialog.Root>

<AlertDialog.Root bind:open={isDeleteDialogOpen}>
	<AlertDialog.Content>
		<AlertDialog.Header>
			<AlertDialog.Title>Delete recurring?</AlertDialog.Title>
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
