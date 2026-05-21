<script lang="ts" generics="TData, TValue">
	import { type ColumnDef, getCoreRowModel, type VisibilityState } from '@tanstack/table-core';
	import { createSvelteTable, FlexRender } from '$lib/components/ui/data-table/index.js';
	import * as Table from '$lib/components/ui/table/index.js';

	type DataTableProps<TData, TValue> = {
		columns: ColumnDef<TData, TValue>[];
		data: TData[];
	};

	let { data, columns }: DataTableProps<TData, TValue> = $props();

	const table = createSvelteTable({
		get data() {
			return data;
		},
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

	const timeRanges = [
		{ value: 'day', label: 'Day' },
		{ value: 'month', label: 'Month' },
		{ value: 'year', label: 'Year' },
		{ value: 'all', label: 'All' }
	];
	let timeRange = $state<'day' | 'month' | 'year' | 'all'>('day');
	const timeRangeLabel = $derived(
		timeRanges.find((tr) => tr.value === timeRange)?.label ?? 'Select a time range'
	);

	import * as DropdownMenu from '$lib/components/ui/dropdown-menu/index.js';
	import { Button } from '$lib/components/ui/button/index.js';
	import LayoutColumnsIcon from '@tabler/icons-svelte/icons/layout-columns';
	import ChevronDownIcon from '@tabler/icons-svelte/icons/chevron-down';
	import PlusIcon from '@tabler/icons-svelte/icons/plus';
	import * as Select from '$lib/components/ui/select/index.js';

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
	<Select.Root type="single" name="timeRange" bind:value={timeRange}>
		<Select.Trigger class="w-[180px]">
			{timeRangeLabel}
		</Select.Trigger>
		<Select.Content>
			<Select.Group>
				{#each timeRanges as range (range.value)}
					<Select.Item value={range.value} label={range.label}>
						{range.label}
					</Select.Item>
				{/each}
			</Select.Group>
		</Select.Content>
	</Select.Root>
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
