<script lang="ts">
	import { CheckIcon, CirclePlusIcon } from '@lucide/svelte';

	import { Badge } from '$lib/components/ui/badge/index.js';
	import { Button } from '$lib/components/ui/button/index.js';
	import * as Command from '$lib/components/ui/command/index.js';
	import * as Popover from '$lib/components/ui/popover/index.js';
	import { Separator } from '$lib/components/ui/separator/index.js';
	import { cn } from '$lib/utils';

	type Option = { value: string; label: string };

	let {
		title,
		options,
		selected = $bindable([])
	}: { title: string; options: Option[]; selected: string[] } = $props();

	let open = $state(false);

	function toggle(value: string) {
		selected = selected.includes(value)
			? selected.filter((v) => v !== value)
			: [...selected, value];
	}

	function clear() {
		selected = [];
	}
</script>

<Popover.Root bind:open>
	<Popover.Trigger>
		{#snippet child({ props })}
			<Button {...props} variant="outline" size="sm" class="border-dashed">
				<CirclePlusIcon />
				{title}
				{#if selected.length > 0}
					<Separator orientation="vertical" class="mx-1 h-4" />
					<Badge variant="secondary" class="rounded-sm px-1 font-normal lg:hidden">
						{selected.length}
					</Badge>
					<div class="hidden gap-1 lg:flex">
						{#if selected.length > 2}
							<Badge variant="secondary" class="rounded-sm px-1 font-normal">
								{selected.length} selected
							</Badge>
						{:else}
							{#each options.filter((o) => selected.includes(o.value)) as option (option.value)}
								<Badge variant="secondary" class="rounded-sm px-1 font-normal">
									{option.label}
								</Badge>
							{/each}
						{/if}
					</div>
				{/if}
			</Button>
		{/snippet}
	</Popover.Trigger>
	<Popover.Content class="w-52 p-0" align="start">
		<Command.Root>
			<Command.Input placeholder={title} />
			<Command.List>
				<Command.Empty>No results.</Command.Empty>
				<Command.Group>
					{#each options as option (option.value)}
						<Command.Item value={option.label} onSelect={() => toggle(option.value)}>
							<CheckIcon class={cn(!selected.includes(option.value) && 'text-transparent')} />
							{option.label}
						</Command.Item>
					{/each}
				</Command.Group>
				{#if selected.length > 0}
					<Command.Separator />
					<Command.Group>
						<Command.Item value="clear-filters" onSelect={clear} class="justify-center text-center">
							Clear filters
						</Command.Item>
					</Command.Group>
				{/if}
			</Command.List>
		</Command.Root>
	</Popover.Content>
</Popover.Root>
