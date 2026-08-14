<script lang="ts">
	import { toast } from 'svelte-sonner';

	import IconPicker from '$lib/components/custom/icon-picker.svelte';
	import { Button, buttonVariants } from '$lib/components/ui/button';
	import * as Dialog from '$lib/components/ui/dialog/index.js';
	import { Input } from '$lib/components/ui/input/index.js';
	import { Label } from '$lib/components/ui/label/index.js';
	import { createCategory } from '$lib/services';
	import { appState } from '$lib/states.svelte';

	const defaultColor = '#94a3b8';

	type Props = {
		open: boolean;
	};

	let { open = $bindable(false) }: Props = $props();
	let name = $state('');
	let color = $state(defaultColor);
	let icon = $state<string | null>(null);

	async function addCategory() {
		const category = await createCategory({ name, color, icon });

		appState.categories = [...appState.categories, category];
		name = '';
		color = defaultColor;
		icon = null;
		open = false;

		toast.success('Category created successfully.');
	}
</script>

<Dialog.Root bind:open>
	<form>
		<Dialog.Content class="sm:max-w-106.25">
			<Dialog.Header>
				<Dialog.Title>Add Category</Dialog.Title>
				<Dialog.Description>Fill in the details to create a new category.</Dialog.Description>
			</Dialog.Header>
			<div class="grid gap-4">
				<div class="grid gap-3">
					<Label for="category-name-1">Name</Label>
					<Input id="category-name-1" name="name" placeholder="e.g. Food" bind:value={name} />
				</div>
				<div class="grid gap-3">
					<Label for="category-color-1">Color</Label>
					<input
						id="category-color-1"
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
				<Button type="submit" onclick={addCategory}>Create Category</Button>
			</Dialog.Footer>
		</Dialog.Content>
	</form>
</Dialog.Root>
