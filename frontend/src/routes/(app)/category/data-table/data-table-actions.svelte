<script lang="ts">
	import { toast } from 'svelte-sonner';

	import IconPicker from '$lib/components/custom/icon-picker.svelte';
	import ConfirmAlertDialog from '$lib/components/custom/table-common/confirm-alert-dialog.svelte';
	import RowActionsMenu from '$lib/components/custom/table-common/row-actions-menu.svelte';
	import { Button, buttonVariants } from '$lib/components/ui/button/index.js';
	import * as Dialog from '$lib/components/ui/dialog/index.js';
	import * as DropdownMenu from '$lib/components/ui/dropdown-menu/index.js';
	import { Input } from '$lib/components/ui/input/index.js';
	import { Label } from '$lib/components/ui/label/index.js';
	import * as Select from '$lib/components/ui/select/index.js';
	import {
		categoryNameExists,
		deleteCategory,
		mergeCategories,
		updateCategory
	} from '$lib/services';
	import { appState } from '$lib/states.svelte';

	const defaultColor = '#94a3b8';

	let { id }: { id: string } = $props();

	let isEditDialogOpen = $state(false);
	let isMergeDialogOpen = $state(false);
	let isDeleteDialogOpen = $state(false);
	let name = $state('');
	let color = $state(defaultColor);
	let icon = $state<string | null>(null);
	let mergeTargetId = $state('');

	let otherCategories = $derived(appState.categories.filter((category) => category.id !== id));

	function openEditDialog() {
		const category = appState.categories.find((item) => item.id === id);
		if (!category) return;

		name = category.name;
		color = category.color ?? defaultColor;
		icon = category.icon ?? null;
		isEditDialogOpen = true;
	}

	async function saveCategory(event: Event) {
		event.preventDefault();
		const trimmedName = name.trim();
		if (!trimmedName) {
			toast.error('Category name is required.');
			return;
		}

		if (categoryNameExists(appState.categories, trimmedName, id)) {
			toast.error(`A category named "${trimmedName}" already exists.`);
			return;
		}

		try {
			const updated = await updateCategory(id, { name: trimmedName, color, icon });
			appState.categories = appState.categories.map((category) =>
				category.id === id ? updated : category
			);
			isEditDialogOpen = false;
			toast.success('Category updated successfully.');
		} catch (error) {
			const isDuplicate = error instanceof Error && error.message.includes('409');
			toast.error(
				isDuplicate
					? `A category named "${trimmedName}" already exists.`
					: 'Failed to update category.'
			);
		}
	}

	function openMergeDialog() {
		mergeTargetId = '';
		isMergeDialogOpen = true;
	}

	async function confirmMerge() {
		if (!mergeTargetId) return;

		const source = appState.categories.find((category) => category.id === id);
		const target = appState.categories.find((category) => category.id === mergeTargetId);
		if (!source || !target) return;

		await mergeCategories({ sourceCategoryId: id, targetCategoryId: mergeTargetId });

		appState.expenses = appState.expenses.map((expense) =>
			expense.categoryName === source.name ? { ...expense, categoryName: target.name } : expense
		);
		appState.categories = appState.categories.filter((category) => category.id !== id);

		isMergeDialogOpen = false;
		toast.success(`Merged "${source.name}" into "${target.name}".`);
	}

	async function confirmDelete() {
		try {
			await deleteCategory(id);
			appState.categories = appState.categories.filter((category) => category.id !== id);
			isDeleteDialogOpen = false;
			toast.success('Category deleted successfully.');
		} catch (error) {
			isDeleteDialogOpen = false;
			const isInUse = error instanceof Error && error.message.includes('409');
			toast.error(
				isInUse
					? 'This category is still in use by one or more expenses. Merge it into another category first.'
					: 'Failed to delete category.'
			);
		}
	}
</script>

<RowActionsMenu onEdit={openEditDialog} onDelete={() => (isDeleteDialogOpen = true)} copyId={id}>
	{#snippet extraItems()}
		<DropdownMenu.Item onclick={openMergeDialog} disabled={otherCategories.length === 0}>
			Merge into...
		</DropdownMenu.Item>
	{/snippet}
</RowActionsMenu>

<Dialog.Root bind:open={isEditDialogOpen}>
	<form>
		<Dialog.Content class="sm:max-w-lg">
			<Dialog.Header>
				<Dialog.Title>Edit Category</Dialog.Title>
				<Dialog.Description>Update the category's name, color, and icon.</Dialog.Description>
			</Dialog.Header>
			<div class="grid gap-4">
				<div class="grid gap-3">
					<Label for="edit-category-name">Name</Label>
					<Input id="edit-category-name" name="name" placeholder="e.g. Food" bind:value={name} />
				</div>
				<div class="grid gap-3">
					<Label for="edit-category-color">Color</Label>
					<input
						id="edit-category-color"
						type="color"
						class="h-9 w-16 rounded-md border p-1"
						bind:value={color}
					/>
				</div>
				<div class="grid gap-3">
					<Label>Icon</Label>
					<IconPicker bind:value={icon} />
				</div>
			</div>
			<Dialog.Footer>
				<Dialog.Close type="button" class={buttonVariants({ variant: 'outline' })}>
					Cancel
				</Dialog.Close>
				<Button type="submit" onclick={saveCategory}>Update Category</Button>
			</Dialog.Footer>
		</Dialog.Content>
	</form>
</Dialog.Root>

<Dialog.Root bind:open={isMergeDialogOpen}>
	<Dialog.Content class="sm:max-w-lg">
		<Dialog.Header>
			<Dialog.Title>Merge Category</Dialog.Title>
			<Dialog.Description>
				Every expense using this category will be reassigned to the target category, then this
				category will be deleted.
			</Dialog.Description>
		</Dialog.Header>
		<div class="grid gap-3">
			<Label for="merge-target">Merge into</Label>
			<Select.Root
				type="single"
				value={mergeTargetId}
				onValueChange={(value: string) => (mergeTargetId = value)}
			>
				<Select.Trigger id="merge-target" class="w-full">
					<span data-slot="select-value">
						{otherCategories.find((category) => category.id === mergeTargetId)?.name ??
							'Select a category...'}
					</span>
				</Select.Trigger>
				<Select.Content>
					{#each otherCategories as category (category.id)}
						<Select.Item value={category.id}>{category.name}</Select.Item>
					{/each}
				</Select.Content>
			</Select.Root>
		</div>
		<Dialog.Footer>
			<Button type="button" variant="outline" onclick={() => (isMergeDialogOpen = false)}>
				Cancel
			</Button>
			<Button type="button" onclick={confirmMerge} disabled={!mergeTargetId}>Merge</Button>
		</Dialog.Footer>
	</Dialog.Content>
</Dialog.Root>

<ConfirmAlertDialog
	bind:open={isDeleteDialogOpen}
	title="Delete category?"
	description="This action cannot be undone. If the category is still used by any expenses, deletion will be blocked — merge it into another category first."
	onConfirm={confirmDelete}
/>
