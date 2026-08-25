<script lang="ts">
	import { toast } from 'svelte-sonner';

	import { Button, buttonVariants } from '$lib/components/ui/button';
	import * as Dialog from '$lib/components/ui/dialog/index.js';
	import { Input } from '$lib/components/ui/input/index.js';
	import { Label } from '$lib/components/ui/label/index.js';
	import * as Select from '$lib/components/ui/select/index.js';
	import { createRecurringAction } from '$lib/services';
	import { appState } from '$lib/states.svelte';

	let name = $state('');
	let amount = $state(0);
	let startAt = $state('');
	let recurrenceType = $state(2);
	let intervalValue = $state(1);
	let dayOfMonth = $state<number | null>(null);
	let type = $state(0);
	let description = $state('');

	async function addRecurringAction() {
		try {
			const response = await createRecurringAction({
				name,
				amount,
				startAt,
				recurrenceType,
				intervalValue,
				dayOfMonth: dayOfMonth || undefined,
				type,
				description: description || undefined,
				isActive: true
			});

			appState.recurringActions = [...appState.recurringActions, response];
			appState.isAddRecurringActionDialogOpen = false;
			name = '';
			amount = 0;
			startAt = '';
			recurrenceType = 2;
			intervalValue = 1;
			dayOfMonth = null;
			type = 0;
			description = '';

			toast.success('Recurring action created successfully.');
		} catch (error) {
			toast.error('Failed to create recurring action. ' + error);
		}
	}
</script>

<Dialog.Root bind:open={appState.isAddRecurringActionDialogOpen}>
	<form>
		<Dialog.Content class="sm:max-w-106.25">
			<Dialog.Header>
				<Dialog.Title>Add Recurring Action</Dialog.Title>
				<Dialog.Description
					>Fill in the details to create a new recurring action.</Dialog.Description
				>
			</Dialog.Header>
			<div class="grid gap-4">
				<div class="grid gap-3">
					<Label for="name-1">Name</Label>
					<Input
						id="name-1"
						name="name"
						placeholder="e.g. Monthly Subscription"
						bind:value={name}
					/>
				</div>
				<div class="grid gap-3">
					<Label for="amount-1">Amount</Label>
					<Input
						id="amount-1"
						name="amount"
						placeholder="0.00"
						type="number"
						step="0.01"
						bind:value={amount}
					/>
				</div>
				<div class="grid gap-3">
					<Label for="startAt-1">Start Date</Label>
					<Input id="startAt-1" name="startAt" type="date" bind:value={startAt} required />
				</div>
				<div class="grid gap-3">
					<Label for="recurrenceType-1">Recurrence Type</Label>
					<Select.Root
						type="single"
						name="recurrenceType"
						value={recurrenceType.toString()}
						onValueChange={(value) => (recurrenceType = parseInt(value))}
					>
						<Select.Trigger id="recurrenceType-1" class="w-full">
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
					<Label for="intervalValue-1">Interval</Label>
					<Input
						id="intervalValue-1"
						name="intervalValue"
						type="number"
						min="1"
						bind:value={intervalValue}
					/>
				</div>
				<div class="grid gap-3">
					<Label for="dayOfMonth-1">Day of Month (optional)</Label>
					<Input
						id="dayOfMonth-1"
						name="dayOfMonth"
						type="number"
						min="1"
						max="31"
						placeholder="Leave empty to use start date's day"
						bind:value={dayOfMonth}
					/>
				</div>
				<div class="grid gap-3">
					<Label for="type-1">Type</Label>
					<Select.Root
						type="single"
						name="type"
						value={type.toString()}
						onValueChange={(value) => (type = parseInt(value))}
					>
						<Select.Trigger id="type-1" class="w-full">
							{type === 1 ? 'Income' : 'Expense'}
						</Select.Trigger>
						<Select.Content>
							<Select.Item value="0" label="Expense">Expense</Select.Item>
							<Select.Item value="1" label="Income">Income</Select.Item>
						</Select.Content>
					</Select.Root>
				</div>
				<div class="grid gap-3">
					<Label for="description-1">Description</Label>
					<Input
						id="description-1"
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
				<Button type="submit" onclick={addRecurringAction}>Create Recurring Action</Button>
			</Dialog.Footer>
		</Dialog.Content>
	</form>
</Dialog.Root>
