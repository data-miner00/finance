<script lang="ts">
	import CopyIcon from '@lucide/svelte/icons/copy';
	import EllipsisIcon from '@lucide/svelte/icons/ellipsis';
	import PencilIcon from '@lucide/svelte/icons/pencil';
	import Trash2Icon from '@lucide/svelte/icons/trash-2';
	import type { Snippet } from 'svelte';

	import { copyText } from '$lib';
	import { Button } from '$lib/components/ui/button/index.js';
	import * as DropdownMenu from '$lib/components/ui/dropdown-menu/index.js';

	type Props = {
		onEdit: () => void;
		onDelete: () => void;
		editLabel?: string;
		deleteLabel?: string;
		copyId?: string;
		copyLabel?: string;
		extraItems?: Snippet;
	};

	let {
		onEdit,
		onDelete,
		editLabel = 'Edit',
		deleteLabel = 'Delete',
		copyId,
		copyLabel = 'Copy ID',
		extraItems
	}: Props = $props();
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
	<DropdownMenu.Content class="w-36" align="end" sideOffset={4}>
		<DropdownMenu.Group>
			<DropdownMenu.Label>Actions</DropdownMenu.Label>
			<DropdownMenu.Item onclick={onEdit}>
				<PencilIcon />
				{editLabel}
			</DropdownMenu.Item>
			{@render extraItems?.()}
			{#if copyId}
				<DropdownMenu.Item onclick={() => copyText(copyId, 'ID copied to clipboard')}>
					<CopyIcon />
					{copyLabel}
				</DropdownMenu.Item>
			{/if}
		</DropdownMenu.Group>
		<DropdownMenu.Separator />
		<DropdownMenu.Item onclick={onDelete} variant="destructive">
			<Trash2Icon />
			{deleteLabel}
		</DropdownMenu.Item>
	</DropdownMenu.Content>
</DropdownMenu.Root>
