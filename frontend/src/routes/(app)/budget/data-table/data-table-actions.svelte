<script lang="ts">
	import EllipsisIcon from '@lucide/svelte/icons/ellipsis';
	import { toast } from 'svelte-sonner';

	import * as AlertDialog from '$lib/components/ui/alert-dialog/index.js';
	import { Button, buttonVariants } from '$lib/components/ui/button/index.js';
	import * as Dialog from '$lib/components/ui/dialog/index.js';
	import * as DropdownMenu from '$lib/components/ui/dropdown-menu/index.js';
	import { Input } from '$lib/components/ui/input/index.js';
	import { Label } from '$lib/components/ui/label/index.js';
	import { updateCategory } from '$lib/services';
	import { appState } from '$lib/states.svelte';

	let { id }: { id: string } = $props();

	let isEditDialogOpen = $state(false);
	let isRemoveDialogOpen = $state(false);
	let amount = $state(0);

	function openEditDialog() {
		const category = appState.categories.find((item) => item.id === id);
		if (!category) return;

		amount = category.budgetAmount ?? 0;
		isEditDialogOpen = true;
	}

	async function saveBudget(event: Event) {
		event.preventDefault();
		const category = appState.categories.find((item) => item.id === id);
		if (!category) return;

		try {
			const updated = await updateCategory(id, {
				name: category.name,
				color: category.color,
				icon: category.icon,
				budgetAmount: amount
			});
			appState.categories = appState.categories.map((c) => (c.id === id ? updated : c));
			isEditDialogOpen = false;
			toast.success('Budget updated successfully.');
		} catch {
			toast.error('Failed to update budget.');
		}
	}

	async function confirmRemove() {
		const category = appState.categories.find((item) => item.id === id);
		if (!category) return;

		try {
			const updated = await updateCategory(id, {
				name: category.name,
				color: category.color,
				icon: category.icon,
				budgetAmount: null
			});
			appState.categories = appState.categories.map((c) => (c.id === id ? updated : c));
			isRemoveDialogOpen = false;
			toast.success('Budget removed.');
		} catch {
			isRemoveDialogOpen = false;
			toast.error('Failed to remove budget.');
		}
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
			<DropdownMenu.Item onclick={openEditDialog}>Edit</DropdownMenu.Item>
		</DropdownMenu.Group>
		<DropdownMenu.Separator />
		<DropdownMenu.Item onclick={() => (isRemoveDialogOpen = true)} variant="destructive">
			Remove Budget
		</DropdownMenu.Item>
	</DropdownMenu.Content>
</DropdownMenu.Root>

<Dialog.Root bind:open={isEditDialogOpen}>
	<form>
		<Dialog.Content class="sm:max-w-lg">
			<Dialog.Header>
				<Dialog.Title>Edit Budget</Dialog.Title>
				<Dialog.Description>
					Update the monthly budget amount for this category.
				</Dialog.Description>
			</Dialog.Header>
			<div class="grid gap-4">
				<div class="grid gap-3">
					<Label for="edit-budget-amount">Amount</Label>
					<Input
						id="edit-budget-amount"
						name="amount"
						type="number"
						step="0.01"
						placeholder="0.00"
						bind:value={amount}
					/>
				</div>
			</div>
			<Dialog.Footer>
				<Dialog.Close type="button" class={buttonVariants({ variant: 'outline' })}>
					Cancel
				</Dialog.Close>
				<Button type="submit" onclick={saveBudget}>Update Budget</Button>
			</Dialog.Footer>
		</Dialog.Content>
	</form>
</Dialog.Root>

<AlertDialog.Root bind:open={isRemoveDialogOpen}>
	<AlertDialog.Content>
		<AlertDialog.Header>
			<AlertDialog.Title>Remove budget?</AlertDialog.Title>
			<AlertDialog.Description>
				This clears the monthly budget for this category. The category itself is not deleted.
			</AlertDialog.Description>
		</AlertDialog.Header>
		<AlertDialog.Footer>
			<AlertDialog.Cancel type="button" class={buttonVariants({ variant: 'outline' })}>
				Cancel
			</AlertDialog.Cancel>
			<AlertDialog.Action onclick={confirmRemove}>Remove</AlertDialog.Action>
		</AlertDialog.Footer>
	</AlertDialog.Content>
</AlertDialog.Root>
