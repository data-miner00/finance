<script lang="ts" generics="TData, TValue">
	import { type ColumnDef, getCoreRowModel, type VisibilityState } from '@tanstack/table-core';
	import { createSvelteTable, FlexRender } from '$lib/components/ui/data-table/index.js';
	import * as Table from '$lib/components/ui/table/index.js';
	import type { Snippet } from 'svelte';

	type DataTableProps<TData, TValue> = {
		columns: ColumnDef<TData, TValue>[];
		data: TData[];
		controls?: Snippet;
	};

	let { data, columns, controls: Controls }: DataTableProps<TData, TValue> = $props();

	const table = createSvelteTable({
		get data() {
			return data;
		},
		// svelte-ignore state_referenced_locally
		columns,
		getCoreRowModel: getCoreRowModel(),
		state: {
			get columnVisibility() {
				return columnVisibility;
			}
		},
		onColumnVisibilityChange: (updater) => {
			if (typeof updater === 'function') {
				columnVisibility = updater(columnVisibility);
			} else {
				columnVisibility = updater;
			}
		}
	});

	import * as DropdownMenu from '$lib/components/ui/dropdown-menu/index.js';
	import { Button } from '$lib/components/ui/button/index.js';
	import LayoutColumnsIcon from '@tabler/icons-svelte/icons/layout-columns';
	import ChevronDownIcon from '@tabler/icons-svelte/icons/chevron-down';
	import PlusIcon from '@tabler/icons-svelte/icons/plus';

	let columnVisibility = $state<VisibilityState>({});
</script>

<div class="mb-6 flex items-center gap-2">
	<DropdownMenu.Root>
		<DropdownMenu.Trigger>
			{#snippet child({ props })}
				<Button variant="outline" size="sm" {...props}>
					<LayoutColumnsIcon />
					<span class="hidden lg:inline">Customize Columns</span>
					<span class="lg:hidden">Columns</span>
					<ChevronDownIcon />
				</Button>
			{/snippet}
		</DropdownMenu.Trigger>
		<DropdownMenu.Content align="end">
			{#each table.getAllColumns().filter((col) => col.getCanHide()) as column (column)}
				<DropdownMenu.CheckboxItem
					class="capitalize"
					bind:checked={() => column.getIsVisible(), (v) => column.toggleVisibility(!!v)}
				>
					{column.id}
				</DropdownMenu.CheckboxItem>
			{/each}
		</DropdownMenu.Content>
	</DropdownMenu.Root>
	<Button variant="outline" size="sm">
		<PlusIcon />
		<span class="hidden lg:inline">Add Section</span>
	</Button>
	{#if Controls}
		{@render Controls()}
	{/if}
</div>
<div class="rounded-md border">
	<Table.Root>
		<Table.Header>
			{#each table.getHeaderGroups() as headerGroup (headerGroup.id)}
				<Table.Row>
					{#each headerGroup.headers as header (header.id)}
						<Table.Head colspan={header.colSpan}>
							{#if !header.isPlaceholder}
								<FlexRender
									content={header.column.columnDef.header}
									context={header.getContext()}
								/>
							{/if}
						</Table.Head>
					{/each}
				</Table.Row>
			{/each}
		</Table.Header>
		<Table.Body>
			{#each table.getRowModel().rows as row (row.id)}
				<Table.Row data-state={row.getIsSelected() && 'selected'}>
					{#each row.getVisibleCells() as cell (cell.id)}
						<Table.Cell>
							<FlexRender content={cell.column.columnDef.cell} context={cell.getContext()} />
						</Table.Cell>
					{/each}
				</Table.Row>
			{:else}
				<Table.Row>
					<Table.Cell colspan={columns.length} class="h-24 text-center">No results.</Table.Cell>
				</Table.Row>
			{/each}
		</Table.Body>
	</Table.Root>
</div>
