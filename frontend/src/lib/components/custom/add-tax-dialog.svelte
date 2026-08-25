<script lang="ts">
	import { toast } from 'svelte-sonner';

	import { Button, buttonVariants } from '$lib/components/ui/button';
	import * as Dialog from '$lib/components/ui/dialog/index.js';
	import { Input } from '$lib/components/ui/input/index.js';
	import { Label } from '$lib/components/ui/label/index.js';
	import { createTax } from '$lib/services';
	import { appState } from '$lib/states.svelte';

	let name = $state('');
	let amount = $state(0);
	let description = $state('');

	async function addTax() {
		try {
			const tax = await createTax({
				name,
				amount,
				description: description || undefined
			});

			appState.taxes = [...appState.taxes, tax];
			appState.openAddDialog = null;
			name = '';
			amount = 0;
			description = '';

			toast.success('Tax created successfully.');
		} catch (error) {
			toast.error('Failed to create tax. ' + error);
		}
	}
</script>

<Dialog.Root
	bind:open={
		() => appState.openAddDialog === 'tax', (v) => (appState.openAddDialog = v ? 'tax' : null)
	}
>
	<form>
		<Dialog.Content class="sm:max-w-106.25">
			<Dialog.Header>
				<Dialog.Title>Add Tax</Dialog.Title>
				<Dialog.Description>Fill in the details to create a new tax.</Dialog.Description>
			</Dialog.Header>
			<div class="grid gap-4">
				<div class="grid gap-3">
					<Label for="name-1">Name</Label>
					<Input id="name-1" name="name" placeholder="e.g. Income Tax" bind:value={name} />
				</div>
				<div class="grid gap-3">
					<Label for="amount-1">Amount</Label>
					<Input
						id="amount-1"
						name="amount"
						placeholder="0.00"
						type="number"
						step="0.01"
						bind:value={amount}
					/>
				</div>
				<div class="grid gap-3">
					<Label for="description-1">Description</Label>
					<Input
						id="description-1"
						name="description"
						placeholder="Optional description"
						bind:value={description}
					/>
				</div>
			</div>
			<Dialog.Footer>
				<Dialog.Close type="button" class={buttonVariants({ variant: 'outline' })}>
					Cancel
				</Dialog.Close>
				<Button type="submit" onclick={addTax}>Create Tax</Button>
			</Dialog.Footer>
		</Dialog.Content>
	</form>
</Dialog.Root>
