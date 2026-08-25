<script lang="ts">
	import { PlusIcon } from '@lucide/svelte';
	import CreditCardIcon from '@lucide/svelte/icons/credit-card';
	import SettingsIcon from '@lucide/svelte/icons/settings';
	import UserIcon from '@lucide/svelte/icons/user';

	import { goto } from '$app/navigation';
	import * as Command from '$lib/components/ui/command/index.js';
	import { appState } from '$lib/states.svelte';

	type Props = {
		isOpen: boolean;
	};

	let { isOpen = $bindable(false) }: Props = $props();

	function gotoPage(path: string) {
		goto(path);
		isOpen = false;
	}

	function openCreateExpenseDialog() {
		appState.openAddDialog = 'transaction';
		isOpen = false;
	}

	function openCreateAccountDialog() {
		appState.openAddDialog = 'account';
		isOpen = false;
	}

	function openCreateTaxDialog() {
		appState.openAddDialog = 'tax';
		isOpen = false;
	}
</script>

<Command.Dialog class="rounded-lg border shadow-md md:min-w-112.5" bind:open={isOpen}>
	<Command.Input placeholder="Type a command or search..." />
	<Command.List>
		<Command.Empty>No results found.</Command.Empty>
		<Command.Group heading="Suggestions">
			<Command.Item onSelect={openCreateExpenseDialog}>
				<PlusIcon />
				<span>Create Expense</span>
			</Command.Item>
			<Command.Item onSelect={openCreateAccountDialog}>
				<PlusIcon />
				<span>Create Account</span>
			</Command.Item>
			<Command.Item onSelect={openCreateTaxDialog}>
				<PlusIcon />
				<span>Create Tax</span>
			</Command.Item>
		</Command.Group>
		<Command.Separator />
		<Command.Group heading="Pages">
			<Command.Item onSelect={() => gotoPage('/expense')}>
				<UserIcon />
				<span>All Expenses</span>
				<Command.Shortcut>⌘E</Command.Shortcut>
			</Command.Item>
			<Command.Item onSelect={() => gotoPage('/income')}>
				<CreditCardIcon />
				<span>All Income</span>
				<Command.Shortcut>⌘I</Command.Shortcut>
			</Command.Item>
			<Command.Item onSelect={() => gotoPage('/recurring')}>
				<SettingsIcon />
				<span>Recurring</span>
				<Command.Shortcut>⌘R</Command.Shortcut>
			</Command.Item>
		</Command.Group>
	</Command.List>
</Command.Dialog>
