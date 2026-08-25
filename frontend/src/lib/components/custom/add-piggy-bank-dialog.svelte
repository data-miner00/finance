<script lang="ts">
	import { toast } from 'svelte-sonner';

	import { Button, buttonVariants } from '$lib/components/ui/button';
	import * as Dialog from '$lib/components/ui/dialog/index.js';
	import { Input } from '$lib/components/ui/input/index.js';
	import { Label } from '$lib/components/ui/label/index.js';
	import { createPiggyBank } from '$lib/services';
	import { appState } from '$lib/states.svelte';

	let name = $state('');
	let amount = $state(0);
	let target = $state(0);
	let description = $state('');
	let deadline = $state('');

	async function addPiggyBank() {
		try {
			const piggyBank = await createPiggyBank({
				name,
				amount,
				target,
				description: description || undefined,
				deadline: deadline || undefined
			});

			appState.piggyBanks = [...appState.piggyBanks, piggyBank];
			appState.isAddPiggyBankDialogOpen = false;
			name = '';
			amount = 0;
			target = 0;
			description = '';
			deadline = '';

			toast.success('Piggy bank created successfully.');
		} catch (error) {
			toast.error('Failed to create piggy bank. ' + error);
		}
	}
</script>

<Dialog.Root bind:open={appState.isAddPiggyBankDialogOpen}>
	<form>
		<Dialog.Content class="sm:max-w-106.25">
			<Dialog.Header>
				<Dialog.Title>Add Piggy Bank</Dialog.Title>
				<Dialog.Description>Fill in the details to create a new piggy bank.</Dialog.Description>
			</Dialog.Header>
			<div class="grid gap-4">
				<div class="grid gap-3">
					<Label for="name-1">Name</Label>
					<Input id="name-1" name="name" placeholder="e.g. Vacation Fund" bind:value={name} />
				</div>
				<div class="grid gap-3">
					<Label for="amount-1">Current Amount</Label>
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
					<Label for="target-1">Target Amount</Label>
					<Input
						id="target-1"
						name="target"
						placeholder="0.00"
						type="number"
						step="0.01"
						bind:value={target}
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
				<div class="grid gap-3">
					<Label for="deadline-1">Deadline (Optional)</Label>
					<Input id="deadline-1" name="deadline" type="date" bind:value={deadline} />
				</div>
			</div>
			<Dialog.Footer>
				<Dialog.Close type="button" class={buttonVariants({ variant: 'outline' })}>
					Cancel
				</Dialog.Close>
				<Button type="submit" onclick={addPiggyBank}>Create Piggy Bank</Button>
			</Dialog.Footer>
		</Dialog.Content>
	</form>
</Dialog.Root>
