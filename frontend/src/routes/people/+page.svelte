<script lang="ts">
	import { PlusIcon } from '@lucide/svelte';
	import { onMount } from 'svelte';

	import { Button, buttonVariants } from '$lib/components/ui/button';
	import * as Dialog from '$lib/components/ui/dialog/index.js';
	import { Input } from '$lib/components/ui/input/index.js';
	import { Label } from '$lib/components/ui/label/index.js';
	import { createPerson } from '$lib/services';
	import { appState } from '$lib/states.svelte';

	import { columns } from './data-table/column';
	import DataTable from './data-table/index.svelte';

	let isDialogOpen = $state(false);

	let name = $state('');
	let alias = $state('');
	let description = $state('');

	async function addPerson() {
		const person = await createPerson({
			name,
			alias: alias || undefined,
			description: description || undefined
		});

		appState.people = [...appState.people, person];
		isDialogOpen = false;
		name = '';
		description = '';
		alias = '';
	}

	onMount(() => {
		appState.pageTitle = 'People';
	});
</script>

{#snippet dataTableControls()}
	<Button size="sm" onclick={() => (isDialogOpen = true)}>
		<PlusIcon />
		Create Person
	</Button>
{/snippet}

<div class="flex flex-col gap-4 py-4 md:gap-6 md:py-6">
	<div class="px-4 lg:px-6">
		<div class="max-w-4xl space-y-2">
			<h1 class="text-xl font-bold">People</h1>
			<p class="text-sm text-muted-foreground">Manage your contacts and team members.</p>
		</div>
	</div>

	<div class="px-4 lg:px-6">
		<DataTable data={appState.people} {columns} controls={dataTableControls} />
	</div>
</div>

<Dialog.Root bind:open={isDialogOpen}>
	<form>
		<Dialog.Content class="sm:max-w-106.25">
			<Dialog.Header>
				<Dialog.Title>Add Person</Dialog.Title>
				<Dialog.Description>Fill in the details to create a new person.</Dialog.Description>
			</Dialog.Header>
			<div class="grid gap-4">
				<div class="grid gap-3">
					<Label for="name-1">Name</Label>
					<Input id="name-1" name="name" placeholder="e.g. Vacation Fund" bind:value={name} />
				</div>
				<div class="grid gap-3">
					<Label for="alias-1">Alias</Label>
					<Input id="alias-1" name="alias" placeholder="e.g. John Doe" bind:value={alias} />
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
				<Button type="submit" onclick={addPerson}>Create Person</Button>
			</Dialog.Footer>
		</Dialog.Content>
	</form>
</Dialog.Root>
