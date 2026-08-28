<script lang="ts">
	import ArrowRightIcon from '@tabler/icons-svelte/icons/arrow-right';

	import { resolve } from '$app/paths';
	import type { ResolvedPathname } from '$app/types';
	import { isToday } from '$lib';
	import { computeBudgetRows } from '$lib/budget';
	import DataTable from '$lib/components/data-table-revamp.svelte';
	import { Badge } from '$lib/components/ui/badge/index.js';
	import { buttonVariants } from '$lib/components/ui/button/index.js';
	import { Label } from '$lib/components/ui/label/index.js';
	import * as Select from '$lib/components/ui/select/index.js';
	import * as Tabs from '$lib/components/ui/tabs/index.js';
	import { appState } from '$lib/states.svelte';

	import { columns as budgetVsActualColumns } from './data-table/budget-vs-actual-columns';
	import {
		mergeRecentActivity,
		columns as recentActivityColumns
	} from './data-table/recent-activity-columns';
	import { columns as savingsGoalsColumns } from './data-table/savings-goals-columns';
	import { columns as upcomingRecurringColumns } from './data-table/upcoming-recurring-columns';

	let recentActivityRows = $derived(mergeRecentActivity(appState.expenses, appState.incomes));
	let upcomingRecurringRows = $derived(appState.recurringActions.filter((r) => r.isActive));
	let savingsGoalsRows = $derived(appState.piggyBanks);
	let budgetVsActualRows = $derived(computeBudgetRows(appState.categories, appState.expenses));

	let views = $derived([
		{
			id: 'recent-activity',
			label: 'Recent Activity',
			badge: recentActivityRows.filter((r) => isToday(r.actionedAt)).length
		},
		{
			id: 'upcoming-recurring',
			label: 'Upcoming Recurring',
			badge: upcomingRecurringRows.length
		},
		{
			id: 'savings-goals',
			label: 'Savings Goals',
			badge: savingsGoalsRows.filter((p) => p.amount < p.target).length
		},
		{
			id: 'budget-vs-actual',
			label: 'Budget vs Actual',
			badge: budgetVsActualRows.filter((b) => b.progress > 100).length
		}
	]);

	let view = $state('recent-activity');
	let viewLabel = $derived(views.find((v) => view === v.id)?.label ?? 'Select a view');
</script>

{#snippet viewAllLink(href: ResolvedPathname, label: string)}
	<a {href} class={buttonVariants({ variant: 'outline', size: 'sm' })}>
		{label}
		<ArrowRightIcon />
	</a>
{/snippet}

<Tabs.Root bind:value={view} class="w-full flex-col justify-start gap-6">
	<div class="flex items-center justify-between px-4 lg:px-6">
		<Label for="view-selector" class="sr-only">View</Label>
		<Select.Root type="single" bind:value={view}>
			<Select.Trigger class="flex w-fit @4xl/main:hidden" size="sm" id="view-selector">
				{viewLabel}
			</Select.Trigger>
			<Select.Content>
				{#each views as v (v.id)}
					<Select.Item value={v.id}>{v.label}</Select.Item>
				{/each}
			</Select.Content>
		</Select.Root>
		<Tabs.List
			class="hidden **:data-[slot=badge]:size-5 **:data-[slot=badge]:rounded-full **:data-[slot=badge]:bg-muted-foreground/30 **:data-[slot=badge]:px-1 @4xl/main:flex"
		>
			{#each views as v (v.id)}
				<Tabs.Trigger value={v.id}>
					{v.label}
					{#if v.badge > 0}
						<Badge variant="secondary">{v.badge}</Badge>
					{/if}
				</Tabs.Trigger>
			{/each}
		</Tabs.List>
	</div>

	<Tabs.Content value="recent-activity" class="flex flex-col gap-4 px-4 lg:px-6">
		<DataTable data={recentActivityRows} columns={recentActivityColumns} getRowId={(row) => row.id}>
			{#snippet controls()}
				{@render viewAllLink(resolve('/expense'), 'View all expenses')}
				{@render viewAllLink(resolve('/income'), 'View all incomes')}
			{/snippet}
		</DataTable>
	</Tabs.Content>
	<Tabs.Content value="upcoming-recurring" class="flex flex-col gap-4 px-4 lg:px-6">
		<DataTable data={upcomingRecurringRows} columns={upcomingRecurringColumns}>
			{#snippet controls()}
				{@render viewAllLink(resolve('/recurring'), 'View all recurring')}
			{/snippet}
		</DataTable>
	</Tabs.Content>
	<Tabs.Content value="savings-goals" class="flex flex-col gap-4 px-4 lg:px-6">
		<DataTable data={savingsGoalsRows} columns={savingsGoalsColumns}>
			{#snippet controls()}
				{@render viewAllLink(resolve('/piggy-bank'), 'View all goals')}
			{/snippet}
		</DataTable>
	</Tabs.Content>
	<Tabs.Content value="budget-vs-actual" class="flex flex-col gap-4 px-4 lg:px-6">
		<DataTable data={budgetVsActualRows} columns={budgetVsActualColumns}>
			{#snippet controls()}
				{@render viewAllLink(resolve('/budget'), 'View all budgets')}
			{/snippet}
		</DataTable>
	</Tabs.Content>
</Tabs.Root>
