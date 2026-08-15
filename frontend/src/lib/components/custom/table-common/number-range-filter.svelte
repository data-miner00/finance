<script lang="ts">
	import { CirclePlusIcon } from '@lucide/svelte';

	import { formatCurrency } from '$lib';
	import { Badge } from '$lib/components/ui/badge/index.js';
	import { Button } from '$lib/components/ui/button/index.js';
	import { Input } from '$lib/components/ui/input/index.js';
	import { Label } from '$lib/components/ui/label/index.js';
	import * as Popover from '$lib/components/ui/popover/index.js';
	import { Separator } from '$lib/components/ui/separator/index.js';

	let {
		title,
		min = $bindable(),
		max = $bindable()
	}: { title: string; min: number | undefined; max: number | undefined } = $props();

	let open = $state(false);
	const isActive = $derived(min !== undefined || max !== undefined);

	function clear() {
		min = undefined;
		max = undefined;
		open = false;
	}
</script>

<Popover.Root bind:open>
	<Popover.Trigger>
		{#snippet child({ props })}
			<Button {...props} variant="outline" size="sm" class="border-dashed">
				<CirclePlusIcon />
				{title}
				{#if isActive}
					<Separator orientation="vertical" class="mx-1 h-4" />
					<Badge variant="secondary" class="rounded-sm px-1 font-normal">
						{min !== undefined ? formatCurrency(min) : 'Any'} &ndash; {max !== undefined
							? formatCurrency(max)
							: 'Any'}
					</Badge>
				{/if}
			</Button>
		{/snippet}
	</Popover.Trigger>
	<Popover.Content class="w-64" align="start">
		<div class="grid gap-3">
			<div class="grid grid-cols-2 gap-2">
				<div class="grid gap-1.5">
					<Label for="amount-min">Min</Label>
					<Input id="amount-min" type="number" step="0.01" placeholder="0.00" bind:value={min} />
				</div>
				<div class="grid gap-1.5">
					<Label for="amount-max">Max</Label>
					<Input id="amount-max" type="number" step="0.01" placeholder="0.00" bind:value={max} />
				</div>
			</div>
			<Button variant="ghost" size="sm" disabled={!isActive} onclick={clear}>Clear</Button>
		</div>
	</Popover.Content>
</Popover.Root>
