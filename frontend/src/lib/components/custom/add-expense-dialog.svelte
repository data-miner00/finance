<script lang="ts">
	import { Button, buttonVariants } from '$lib/components/ui/button/index.js';
	import * as Dialog from '$lib/components/ui/dialog/index.js';
	import { Input } from '$lib/components/ui/input/index.js';
	import { Label } from '$lib/components/ui/label/index.js';
	import Calendar from '$lib/components/ui/calendar/calendar.svelte';
	import * as Popover from '$lib/components/ui/popover/index.js';
	import ChevronDownIcon from '@lucide/svelte/icons/chevron-down';
	import { getLocalTimeZone, today, type CalendarDate } from '@internationalized/date';
	import { createExpense } from '$lib/services';
	import { appState } from '$lib/states.svelte';
	const id = $props.id();

	type Props = {
		open: boolean;
	};

	let { open = $bindable(false) }: Props = $props();

	let isCalendarPopoverOpen = $state(false);
	let name = $state('');
	let description = $state('');
	let category = $state(''); // Todo, make this select from existing categories or allow creating new ones
	let amount = $state(0);
	let location = $state('');
	let actionedAt = $state<CalendarDate>();

	async function handleSubmit(event: Event) {
		event.preventDefault();

		const expense = await createExpense({
			name,
			description,
			category,
			amount,
			location,
			actionedAt: actionedAt ? actionedAt.toString() : null
		});

		appState.expenses = [...appState.expenses, expense];

		open = false;
	}
</script>

<Dialog.Root bind:open>
	<form>
		<Dialog.Trigger type="button" class={buttonVariants({ variant: 'outline' })}>
			Open Dialog
		</Dialog.Trigger>
		<Dialog.Content class="sm:max-w-[425px]">
			<Dialog.Header>
				<Dialog.Title>Add Expense</Dialog.Title>
				<Dialog.Description>Enter the details for the new expense.</Dialog.Description>
			</Dialog.Header>
			<div class="grid gap-4">
				<div class="grid gap-3">
					<Label for="name-1">Name</Label>
					<Input
						id="name-1"
						name="name"
						bind:value={name}
						placeholder="e.g. Texas Chicken"
						autocomplete="off"
					/>
				</div>
				<div class="grid gap-3">
					<Label for="category-1">Category</Label>
					<Input id="category-1" name="category" bind:value={category} placeholder="e.g. Food" />
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
					<Label for="amount-1">Amount</Label>
					<Input
						id="amount-1"
						name="amount"
						type="number"
						step="0.01"
						min="0"
						bind:value={amount}
						placeholder="e.g. 100.00"
					/>
				</div>
				<div class="grid gap-3">
					<Label for="location-1">Location</Label>
					<Input
						id="location-1"
						name="location"
						bind:value={location}
						placeholder="e.g. Melaka Supermarket"
					/>
				</div>

				<div class="grid gap-3">
					<Label for="{id}-date" class="px-1">Actioned Date</Label>
					<Popover.Root bind:open={isCalendarPopoverOpen}>
						<Popover.Trigger id="{id}-date">
							{#snippet child({ props })}
								<Button {...props} variant="outline" class="w-48 justify-between font-normal">
									{actionedAt
										? actionedAt.toDate(getLocalTimeZone()).toLocaleDateString()
										: 'Select date'}
									<ChevronDownIcon />
								</Button>
							{/snippet}
						</Popover.Trigger>
						<Popover.Content class="w-auto overflow-hidden p-0" align="start">
							<Calendar
								type="single"
								bind:value={actionedAt}
								captionLayout="dropdown"
								onValueChange={() => {
									open = false;
								}}
								maxValue={today(getLocalTimeZone())}
							/>
						</Popover.Content>
					</Popover.Root>
				</div>
			</div>
			<Dialog.Footer>
				<Dialog.Close type="button" class={buttonVariants({ variant: 'outline' })}>
					Cancel
				</Dialog.Close>
				<Button type="submit" onclick={handleSubmit}>Create</Button>
			</Dialog.Footer>
		</Dialog.Content>
	</form>
</Dialog.Root>
