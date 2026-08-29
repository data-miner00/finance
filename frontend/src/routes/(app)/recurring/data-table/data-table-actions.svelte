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
	import { deleteRecurringAction, updateRecurringAction } from '$lib/services';
	import { appState } from '$lib/states.svelte';

	let { id }: { id: string } = $props();

	let isEditDialogOpen = $state(false);
	let isDeleteDialogOpen = $state(false);
	let name = $state('');
	let amount = $state(0);
	let currency = $state(appState.settings.defaultCurrency);
	let startAt = $state('');
	let recurrenceType = $state(2);
	let intervalValue = $state(1);
	let dayOfMonth = $state<number | null>(null);
	let description = $state('');
	let isActive = $state(true);

	function openEditDialog() {
		const item = appState.recurringActions.find((r) => r.id === id);
		if (!item) return;
		name = item.name;
		amount = item.amount;
		currency = item.currency;
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
		try {
			const updated = await updateRecurringAction(id, {
				name,
				amount,
				currency,
				startAt,
				recurrenceType,
				intervalValue,
				dayOfMonth,
				description,
				isActive
			});
			appState.recurringActions = appState.recurringActions.map((r) => (r.id === id ? updated : r));
			isEditDialogOpen = false;
			toast.success('Recurring action updated successfully.');
		} catch (error) {
			toast.error('Failed to update recurring action. ' + error);
		}
	}

	async function confirmDelete() {
		try {
			await deleteRecurringAction(id);
			appState.recurringActions = appState.recurringActions.filter((r) => r.id !== id);
			toast.success('Recurring action deleted successfully.');
		} catch (error) {
			toast.error('Failed to delete recurring action. ' + error);
		} finally {
			isDeleteDialogOpen = false;
		}
	}
</script>

<RowActionsMenu onEdit={openEditDialog} onDelete={() => (isDeleteDialogOpen = true)} copyId={id} />

<Dialog.Root bind:open={isEditDialogOpen}>
	<form>
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
					<Label for="edit-startAt">Start Date</Label>
					<Input id="edit-startAt" name="startAt" type="date" bind:value={startAt} />
				</div>
				<div class="grid gap-3">
					<Label for="edit-recurrenceType">Recurrence Type</Label>
					<Select.Root
						type="single"
						name="recurrenceType"
						value={recurrenceType.toString()}
						onValueChange={(value) => (recurrenceType = parseInt(value))}
					>
						<Select.Trigger id="edit-recurrenceType" class="w-full">
							{#if recurrenceType === 0}
								Daily
							{:else if recurrenceType === 1}
								Weekly
							{:else if recurrenceType === 2}
								Monthly
							{:else if recurrenceType === 3}
								Yearly
							{/if}
						</Select.Trigger>
						<Select.Content>
							<Select.Item value="0" label="Daily">Daily</Select.Item>
							<Select.Item value="1" label="Weekly">Weekly</Select.Item>
							<Select.Item value="2" label="Monthly">Monthly</Select.Item>
							<Select.Item value="3" label="Yearly">Yearly</Select.Item>
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
				<Button type="submit" onclick={saveRecurring}>Update Recurring</Button>
			</Dialog.Footer>
		</Dialog.Content>
	</form>
</Dialog.Root>

<ConfirmAlertDialog
	bind:open={isDeleteDialogOpen}
	title="Delete recurring?"
	description="This action cannot be undone."
	onConfirm={confirmDelete}
/>
