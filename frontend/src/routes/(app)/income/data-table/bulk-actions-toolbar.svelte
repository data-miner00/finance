<script lang="ts">
	import { toast } from 'svelte-sonner';

	import ConfirmAlertDialog from '$lib/components/custom/table-common/confirm-alert-dialog.svelte';
	import { Button, buttonVariants } from '$lib/components/ui/button/index.js';
	import * as Dialog from '$lib/components/ui/dialog/index.js';
	import { Label } from '$lib/components/ui/label/index.js';
	import * as Select from '$lib/components/ui/select/index.js';
	import { deleteIncome, updateIncome } from '$lib/services';
	import type { Income, UpdateIncomeRequest } from '$lib/services/types';
	import { appState } from '$lib/states.svelte';

	let { selectedIds, onCleared }: { selectedIds: string[]; onCleared: () => void } = $props();

	let isDeleteDialogOpen = $state(false);
	let isReassignDialogOpen = $state(false);
	let reassignAccountId = $state('');
	let isSubmitting = $state(false);

	function toUpdateRequest(income: Income, accountId: string): UpdateIncomeRequest {
		return {
			name: income.name,
			description: income.description ?? undefined,
			amount: income.amount,
			currency: income.currency,
			accountId: accountId || undefined
		};
	}

	function reportOutcome(action: string, succeeded: number, failed: number) {
		if (failed === 0) {
			toast.success(`${action} ${succeeded} income${succeeded === 1 ? '' : 's'}.`);
		} else if (succeeded === 0) {
			toast.error(`Failed to ${action.toLowerCase()} ${failed} income${failed === 1 ? '' : 's'}.`);
		} else {
			toast.warning(
				`${action} ${succeeded} income${succeeded === 1 ? '' : 's'}, ${failed} failed.`
			);
		}
	}

	async function confirmBulkDelete() {
		isSubmitting = true;
		const results = await Promise.allSettled(selectedIds.map((id) => deleteIncome(id)));

		const deletedIds = new Set(
			selectedIds.filter((_, index) => results[index].status === 'fulfilled')
		);
		appState.incomes = appState.incomes.filter((i) => !deletedIds.has(i.id));

		isSubmitting = false;
		isDeleteDialogOpen = false;
		reportOutcome('Deleted', deletedIds.size, selectedIds.length - deletedIds.size);
		if (deletedIds.size > 0) onCleared();
	}

	async function confirmBulkReassign() {
		isSubmitting = true;
		const results = await Promise.allSettled(
			selectedIds.map((id) => {
				const income = appState.incomes.find((i) => i.id === id);
				if (!income) return Promise.reject(new Error('Income not found.'));
				return updateIncome(id, toUpdateRequest(income, reassignAccountId));
			})
		);

		let succeeded = 0;
		for (const result of results) {
			if (result.status === 'fulfilled') {
				succeeded++;
				const updated = result.value;
				appState.incomes = appState.incomes.map((i) => (i.id === updated.id ? updated : i));
			}
		}

		isSubmitting = false;
		isReassignDialogOpen = false;
		reportOutcome('Reassigned', succeeded, selectedIds.length - succeeded);
		if (succeeded > 0) onCleared();
	}

	function openReassignDialog() {
		reassignAccountId = '';
		isReassignDialogOpen = true;
	}

	let selectedCountLabel = $derived(
		`${selectedIds.length} income${selectedIds.length === 1 ? '' : 's'}`
	);
</script>

{#if selectedIds.length > 0}
	<div class="mb-4 flex items-center gap-3 rounded-md border bg-muted/50 px-4 py-2">
		<span class="text-sm font-medium">
			{selectedIds.length} selected
		</span>
		<Button variant="outline" size="sm" onclick={openReassignDialog}>Reassign account</Button>
		<Button variant="outline" size="sm" onclick={() => (isDeleteDialogOpen = true)}>Delete</Button>
		<Button variant="ghost" size="sm" onclick={onCleared}>Clear selection</Button>
	</div>
{/if}

<Dialog.Root bind:open={isReassignDialogOpen}>
	<Dialog.Content class="sm:max-w-sm">
		<Dialog.Header>
			<Dialog.Title>Reassign {selectedCountLabel}</Dialog.Title>
			<Dialog.Description>Assign an account to all selected incomes.</Dialog.Description>
		</Dialog.Header>
		<div class="grid gap-3">
			<Label for="bulk-reassign-account">Account</Label>
			<Select.Root type="single" name="accountId" bind:value={reassignAccountId}>
				<Select.Trigger id="bulk-reassign-account" class="w-full">
					{appState.accounts.find((a) => a.id === reassignAccountId)?.name ?? 'No account'}
				</Select.Trigger>
				<Select.Content>
					<Select.Item value="" label="No account">No account</Select.Item>
					{#each appState.accounts as account (account.id)}
						<Select.Item value={account.id} label={account.name}>
							{account.name}
						</Select.Item>
					{/each}
				</Select.Content>
			</Select.Root>
		</div>
		<Dialog.Footer>
			<Dialog.Close type="button" class={buttonVariants({ variant: 'outline' })}>
				Cancel
			</Dialog.Close>
			<Button type="button" disabled={isSubmitting} onclick={confirmBulkReassign}>Reassign</Button>
		</Dialog.Footer>
	</Dialog.Content>
</Dialog.Root>

<ConfirmAlertDialog
	bind:open={isDeleteDialogOpen}
	title="Delete {selectedCountLabel}?"
	description="This action cannot be undone."
	onConfirm={confirmBulkDelete}
/>
