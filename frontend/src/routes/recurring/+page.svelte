<script lang="ts">
	import { appState } from '$lib/states.svelte';
	import DataTable from './data-table/index.svelte';
	import { columns } from './data-table/column';
	import { onMount } from 'svelte';

	import { Button, buttonVariants } from '$lib/components/ui/button';
	import * as Dialog from '$lib/components/ui/dialog/index.js';
	import * as Select from '$lib/components/ui/select/index.js';
	import { Input } from '$lib/components/ui/input/index.js';
	import { Label } from '$lib/components/ui/label/index.js';
	import { createRecurringAction } from '$lib/services';

	let isDialogOpen = $state(false);
	let name = $state('');
	let amount = $state(0);
	let startAt = $state('');
	let recurrenceType = $state('Monthly');
	let intervalValue = $state(1);
	let dayOfMonth = $state<number | null>(null);
	let type = $state('0');
	let description = $state('');
	let isActive = $state(true);

	async function addRecurringAction() {
		const response = await createRecurringAction({
			name,
			amount,
			startAt,
			recurrenceType,
			intervalValue,
			dayOfMonth: dayOfMonth || undefined,
			type: Number(type),
			description: description || undefined,
			isActive
		});

		appState.recurringActions.push(response);
		isDialogOpen = false;
		name = '';
		amount = 0;
		startAt = '';
		recurrenceType = 'Monthly';
		intervalValue = 1;
		dayOfMonth = null;
		type = '0';
		description = '';
		isActive = true;
	}

	onMount(() => {
		appState.pageTitle = 'Recurring Actions';
	});
</script>

<div class="flex flex-col gap-4 py-4 md:gap-6 md:py-6">
	<div class="px-4 lg:px-6">
		<div class="max-w-4xl space-y-2">
			<h1 class="text-xl font-bold">Recurring Actions</h1>
			<p class="text-sm text-muted-foreground">
				Manage your recurring actions and view their details.
			</p>
		</div>

		<Button onclick={() => (isDialogOpen = true)}>Create Recurring Action</Button>
	</div>

	<div class="px-4 lg:px-6">
		<DataTable data={appState.recurringActions} {columns} />
	</div>
</div>

<Dialog.Root bind:open={isDialogOpen}>
	<form>
		<Dialog.Content class="sm:max-w-[425px]">
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
					<Select.Root type="single" name="recurrenceType" bind:value={recurrenceType}>
						<Select.Trigger id="recurrenceType-1" class="w-full">
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
					<Select.Root type="single" name="type" bind:value={type}>
						<Select.Trigger id="type-1" class="w-full">
							{type === '1' ? 'Income' : 'Expense'}
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
