<script lang="ts">
	import { type CalendarDate, getLocalTimeZone, today } from '@internationalized/date';
	import { CheckIcon, ChevronsUpDownIcon } from '@lucide/svelte';
	import ChevronDownIcon from '@lucide/svelte/icons/chevron-down';
	import { tick } from 'svelte';
	import { toast } from 'svelte-sonner';

	import { Button, buttonVariants } from '$lib/components/ui/button/index.js';
	import Calendar from '$lib/components/ui/calendar/calendar.svelte';
	import * as Command from '$lib/components/ui/command/index.js';
	import * as Dialog from '$lib/components/ui/dialog/index.js';
	import { Input } from '$lib/components/ui/input/index.js';
	import { Label } from '$lib/components/ui/label/index.js';
	import * as Popover from '$lib/components/ui/popover/index.js';
	import * as Select from '$lib/components/ui/select/index.js';
	import * as Tabs from '$lib/components/ui/tabs/index.js';
	import { AccountType, createExpense, createIncome } from '$lib/services';
	import { appState } from '$lib/states.svelte';
	import { cn } from '$lib/utils';

	const id = $props.id();

	type Props = {
		open: boolean;
	};

	let { open = $bindable(false) }: Props = $props();
	let tab = $state<'expense' | 'income'>('expense');

	let isCalendarPopoverOpen = $state(false);
	let name = $state('');
	let description = $state<string>();
	let categoryName = $state('');
	let amount = $state(0);
	let location = $state('');
	let actionedAt = $state<CalendarDate>();
	let agentName = $state('');

	async function handleSubmit(event: Event) {
		event.preventDefault();

		if (tab === 'income') {
			const income = await createIncome({
				name,
				description,
				amount
			});

			name = '';
			description = undefined;
			amount = 0;

			appState.incomes = [...appState.incomes, income];
		} else {
			const expense = await createExpense({
				name,
				description,
				categoryName,
				amount,
				location,
				agentName
			});

			name = '';
			description = undefined;
			categoryName = '';
			amount = 0;
			location = '';
			actionedAt = undefined;
			agentName = '';

			appState.expenses.push(expense);
			if (expense.categoryName && !appState.categories.includes(expense.categoryName)) {
				appState.categories.push(expense.categoryName);
			}
		}

		toast.success(`${tab === 'income' ? 'Income' : 'Expense'} created successfully.`);
		open = false;
	}

	let comboOpen = $state(false);
	let triggerRef = $state<HTMLButtonElement>(null!);

	let locationComboOpen = $state(false);
	let locationTriggerRef = $state<HTMLButtonElement>(null!);

	let agentComboOpen = $state(false);
	let agentTriggerRef = $state<HTMLButtonElement>(null!);

	// We want to refocus the trigger button when the user selects
	// an item from the list so users can continue navigating the
	// rest of the form with the keyboard.
	function closeAndFocusTrigger() {
		comboOpen = false;
		tick().then(() => {
			triggerRef.focus();
		});
	}

	function closeAndFocusLocationTrigger() {
		locationComboOpen = false;
		tick().then(() => {
			locationTriggerRef.focus();
		});
	}

	function closeAndFocusAgentTrigger() {
		agentComboOpen = false;
		tick().then(() => {
			agentTriggerRef.focus();
		});
	}

	const paymentMethods = Object.values(AccountType).filter((v) => typeof v === 'string');

	let value = $state('');

	const triggerContent = $derived(
		paymentMethods.find((f) => f === value) ?? 'Select a payment method'
	);
</script>

<Dialog.Root bind:open>
	<form>
		<Dialog.Content class="sm:max-w-106.25">
			<Dialog.Header>
				<Dialog.Title>{tab === 'expense' ? 'Add Expense' : 'Add Income'}</Dialog.Title>
				<Dialog.Description>
					{tab === 'expense'
						? 'Enter the details for the new expense.'
						: 'Enter the details for the new income.'}
				</Dialog.Description>
			</Dialog.Header>

			<Tabs.Root bind:value={tab} class="grid gap-4">
				<Tabs.List variant="line" class="grid grid-cols-2 gap-2">
					<Tabs.Trigger value="expense">Expense</Tabs.Trigger>
					<Tabs.Trigger value="income">Income</Tabs.Trigger>
				</Tabs.List>

				<Tabs.Content value="expense" class="grid gap-4">
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
					<!-- <div class="grid gap-3">
						<Label for="paymentMethod">Method</Label>
						<Select.Root type="single" name="paymentMethod" bind:value>
							<Select.Trigger class="w-full">
								{triggerContent}
							</Select.Trigger>
							<Select.Content>
								<Select.Group>
									{#each paymentMethods as method (method)}
										<Select.Item value={method} label={method}>
											{method}
										</Select.Item>
									{/each}
								</Select.Group>
							</Select.Content>
						</Select.Root>
					</div> -->
					<div class="grid gap-3">
						<Label for="category-1">Category</Label>
						<Popover.Root bind:open={comboOpen}>
							<Popover.Trigger bind:ref={triggerRef}>
								{#snippet child({ props })}
									<Button
										{...props}
										variant="outline"
										class="justify-between"
										role="combobox"
										aria-expanded={open}
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
														closeAndFocusTrigger();
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
						<Label for={`${id}-date`} class="px-1">Actioned Date</Label>
						<Popover.Root bind:open={isCalendarPopoverOpen}>
							<Popover.Trigger id={`${id}-date`}>
								{#snippet child({ props })}
									<Button {...props} variant="outline" class="justify-between font-normal">
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
										isCalendarPopoverOpen = false;
									}}
									maxValue={today(getLocalTimeZone())}
								/>
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
				</Tabs.Content>

				<Tabs.Content value="income" class="grid gap-4">
					<div class="grid gap-3">
						<Label for="name-2">Name</Label>
						<Input
							id="name-2"
							name="name"
							bind:value={name}
							placeholder="e.g. Salary"
							autocomplete="off"
						/>
					</div>
					<div class="grid gap-3">
						<Label for="description-2">Description</Label>
						<Input
							id="description-2"
							name="description"
							bind:value={description}
							placeholder="e.g. May payout"
						/>
					</div>
					<div class="grid gap-3">
						<Label for="amount-2">Amount</Label>
						<Input
							id="amount-2"
							name="amount"
							type="number"
							step="0.01"
							min="0"
							bind:value={amount}
							placeholder="e.g. 100.00"
						/>
					</div>
				</Tabs.Content>
			</Tabs.Root>

			<Dialog.Footer>
				<Dialog.Close type="button" class={buttonVariants({ variant: 'outline' })}>
					Cancel
				</Dialog.Close>
				<Button type="submit" onclick={handleSubmit}>Create</Button>
			</Dialog.Footer>
		</Dialog.Content>
	</form>
</Dialog.Root>
