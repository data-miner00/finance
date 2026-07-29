<script lang="ts">
	import { PlusIcon } from '@lucide/svelte';

	import { Button } from '$lib/components/ui/button/index.js';
	import { Separator } from '$lib/components/ui/separator/index.js';
	import * as Sidebar from '$lib/components/ui/sidebar/index.js';
	import { type CurrentPage, appState } from '$lib/states.svelte';

	import ModeToggleButton from './custom/mode-toggle-button.svelte';

	type ActionButton = {
		text: string;
		isDisplay: boolean;
		action: () => void;
	};

	let actions: { [K in CurrentPage]: ActionButton } = {
		expense: {
			text: 'Create Expense',
			isDisplay: true,
			action() {
				appState.isAddTransactionDialogOpen = true;
			}
		},
		income: {
			text: 'Create Income',
			isDisplay: true,
			action() {
				appState.isAddTransactionDialogOpen = true;
			}
		},
		account: {
			text: 'Create Account',
			isDisplay: true,
			action() {
				appState.isAddAccountDialogOpen = true;
			}
		},
		recurring: {
			text: 'Create Recurring',
			isDisplay: true,
			action() {
				// noop
			}
		},
		piggyBank: {
			text: 'Create Piggy Bank',
			isDisplay: true,
			action() {
				appState.isAddPiggyBankDialogOpen = true;
			}
		},
		tax: {
			text: 'Create Tax',
			isDisplay: true,
			action() {
				appState.isAddTaxDialogOpen = true;
			}
		},
		debt: {
			text: 'Create Debt',
			isDisplay: true,
			action() {
				// noop
			}
		},
		settings: {
			text: 'Create Expense',
			isDisplay: true,
			action() {
				appState.isAddTransactionDialogOpen = true;
			}
		},
		dashboard: {
			text: 'Create Expense',
			isDisplay: true,
			action() {
				appState.isAddTransactionDialogOpen = true;
			}
		}
	};

	let actionButton = $derived(actions[appState.currentPage]);
</script>

<header
	class="flex h-(--header-height) shrink-0 items-center gap-2 border-b transition-[width,height] ease-linear group-has-data-[collapsible=icon]/sidebar-wrapper:h-(--header-height)"
>
	<div class="flex w-full items-center gap-1 px-4 lg:gap-2 lg:px-6">
		<Sidebar.Trigger class="-ms-1" />
		<Separator orientation="vertical" class="mx-2 data-[orientation=vertical]:h-4" />
		<h1 class="text-base font-medium">{appState.pageTitle}</h1>
		<div class="ms-auto flex items-center gap-2">
			<ModeToggleButton />

			<Button onclick={actionButton.action}>
				<PlusIcon />
				{actionButton.text}
			</Button>
		</div>
	</div>
</header>
