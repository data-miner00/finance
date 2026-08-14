<script lang="ts">
	import EllipsisIcon from '@lucide/svelte/icons/ellipsis';
	import { toast } from 'svelte-sonner';

	import { copyText } from '$lib';
	import IconPicker from '$lib/components/custom/icon-picker.svelte';
	import * as AlertDialog from '$lib/components/ui/alert-dialog/index.js';
	import { Button, buttonVariants } from '$lib/components/ui/button/index.js';
	import * as Dialog from '$lib/components/ui/dialog/index.js';
	import * as DropdownMenu from '$lib/components/ui/dropdown-menu/index.js';
	import { Input } from '$lib/components/ui/input/index.js';
	import { Label } from '$lib/components/ui/label/index.js';
	import * as Select from '$lib/components/ui/select/index.js';
	import { deleteCategory, mergeCategories, updateCategory } from '$lib/services';
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
		const updated = await updateCategory(id, { name, color, icon });
		appState.categories = appState.categories.map((category) =>
			category.id === id ? updated : category
		);
		isEditDialogOpen = false;
		toast.success('Category updated successfully.');
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
			<DropdownMenu.Item onclick={openEditDialog}>Edit category</DropdownMenu.Item>
			<DropdownMenu.Item onclick={openMergeDialog} disabled={otherCategories.length === 0}>
				Merge into...
			</DropdownMenu.Item>
			<DropdownMenu.Item onclick={() => copyText(id, 'ID copied to clipboard')}>
				Copy category ID
			</DropdownMenu.Item>
		</DropdownMenu.Group>
		<DropdownMenu.Separator />
		<DropdownMenu.Item onclick={() => (isDeleteDialogOpen = true)} variant="destructive">
			Delete category
		</DropdownMenu.Item>
	</DropdownMenu.Content>
</DropdownMenu.Root>

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

<AlertDialog.Root bind:open={isDeleteDialogOpen}>
	<AlertDialog.Content>
		<AlertDialog.Header>
			<AlertDialog.Title>Delete category?</AlertDialog.Title>
			<AlertDialog.Description>
				This action cannot be undone. If the category is still used by any expenses, deletion will
				be blocked — merge it into another category first.
			</AlertDialog.Description>
		</AlertDialog.Header>
		<AlertDialog.Footer>
			<AlertDialog.Cancel type="button" class={buttonVariants({ variant: 'outline' })}>
				Cancel
			</AlertDialog.Cancel>
			<AlertDialog.Action onclick={confirmDelete}>Delete</AlertDialog.Action>
		</AlertDialog.Footer>
	</AlertDialog.Content>
</AlertDialog.Root>
