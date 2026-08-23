<script lang="ts" generics="TData, TValue">
	import ChevronDownIcon from '@tabler/icons-svelte/icons/chevron-down';
	import LayoutColumnsIcon from '@tabler/icons-svelte/icons/layout-columns';
	import {
		type ColumnDef,
		type ColumnFiltersState,
		type PaginationState,
		type RowSelectionState,
		type SortingState,
		type VisibilityState,
		getCoreRowModel,
		getFilteredRowModel,
		getPaginationRowModel,
		getSortedRowModel
	} from '@tanstack/table-core';
	import type { Snippet } from 'svelte';

	import { Button } from '$lib/components/ui/button/index.js';
	import { Checkbox } from '$lib/components/ui/checkbox/index.js';
	import {
		FlexRender,
		createSvelteTable,
		renderComponent
	} from '$lib/components/ui/data-table/index.js';
	import * as DropdownMenu from '$lib/components/ui/dropdown-menu/index.js';
	import { Input } from '$lib/components/ui/input/index.js';
	import { Label } from '$lib/components/ui/label/index.js';
	import * as Table from '$lib/components/ui/table/index.js';
	import { appState } from '$lib/states.svelte';

	import DataTableCheckbox from './data-table-checkbox.svelte';

	type DataTableProps<TData, TValue> = {
		columns: ColumnDef<TData, TValue>[];
		data: TData[];
		controls?: Snippet;
		isPaginated?: boolean;
		enableRowSelection?: boolean;
		getRowId?: (row: TData) => string;
		rowSelection?: RowSelectionState;
	};

	let {
		data,
		columns,
		controls: Controls,
		isPaginated: isPaginatedProp,
		enableRowSelection = false,
		getRowId,
		rowSelection = $bindable({})
	}: DataTableProps<TData, TValue> = $props();

	// svelte-ignore state_referenced_locally
	let isPaginated = $state(isPaginatedProp ?? appState.settings.enableTablePagination);

	let pagination = $state<PaginationState>({ pageIndex: 0, pageSize: 10 });
	let sorting = $state<SortingState>([]);
	let columnFilters = $state<ColumnFiltersState>([]);

	// Selecting all filtered rows (not just the current page) keeps behavior consistent whether
	// or not pagination is toggled on for a given table.
	const selectColumn: ColumnDef<TData, TValue> = {
		id: 'select',
		header: ({ table }) =>
			renderComponent(DataTableCheckbox, {
				checked: table.getIsAllRowsSelected(),
				indeterminate: table.getIsSomeRowsSelected() && !table.getIsAllRowsSelected(),
				onCheckedChange: (value: boolean) => table.toggleAllRowsSelected(!!value),
				'aria-label': 'Select all'
			}),
		cell: ({ row }) =>
			renderComponent(DataTableCheckbox, {
				checked: row.getIsSelected(),
				onCheckedChange: (value: boolean) => row.toggleSelected(!!value),
				'aria-label': 'Select row'
			}),
		enableSorting: false,
		enableHiding: false
	};

	// svelte-ignore state_referenced_locally
	let effectiveColumns = enableRowSelection ? [selectColumn, ...columns] : columns;

	const table = createSvelteTable({
		get data() {
			return data;
		},
		columns: effectiveColumns,
		get getRowId() {
			return getRowId;
		},
		get enableRowSelection() {
			return enableRowSelection;
		},
		getCoreRowModel: getCoreRowModel(),
		getPaginationRowModel: getPaginationRowModel(),
		getSortedRowModel: getSortedRowModel(),
		getFilteredRowModel: getFilteredRowModel(),
		state: {
			get columnVisibility() {
				return columnVisibility;
			},
			get pagination() {
				return pagination;
			},
			get sorting() {
				return sorting;
			},
			get columnFilters() {
				return columnFilters;
			},
			get rowSelection() {
				return rowSelection;
			}
		},
		onSortingChange: (updater) => {
			if (typeof updater === 'function') {
				sorting = updater(sorting);
			} else {
				sorting = updater;
			}
		},
		onPaginationChange: (updater) => {
			if (typeof updater === 'function') {
				pagination = updater(pagination);
			} else {
				pagination = updater;
			}
		},
		onColumnFiltersChange: (updater) => {
			if (typeof updater === 'function') {
				columnFilters = updater(columnFilters);
			} else {
				columnFilters = updater;
			}
		},
		onColumnVisibilityChange: (updater) => {
			if (typeof updater === 'function') {
				columnVisibility = updater(columnVisibility);
			} else {
				columnVisibility = updater;
			}
		},
		onRowSelectionChange: (updater) => {
			if (typeof updater === 'function') {
				rowSelection = updater(rowSelection);
			} else {
				rowSelection = updater;
			}
		}
	});

	let columnVisibility = $state<VisibilityState>({});
</script>

<div>
	<div class="flex items-center justify-between py-4">
		<div>
			<Input
				placeholder="Filter names..."
				value={(table.getColumn('name')?.getFilterValue() as string) ?? ''}
				onchange={(e) => {
					table.getColumn('name')?.setFilterValue(e.currentTarget.value);
				}}
				oninput={(e) => {
					table.getColumn('name')?.setFilterValue(e.currentTarget.value);
				}}
				class="max-w-sm"
			/>
		</div>
		<div class="flex items-center gap-2">
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

			<div class="flex items-center gap-2">
				<Checkbox id="is-paginated" bind:checked={isPaginated} />
				<Label for="is-paginated">Paginate</Label>
			</div>

			{#if Controls}
				{@render Controls()}
			{/if}
		</div>
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
				{#each (isPaginated ? table.getPaginationRowModel() : table.getSortedRowModel()).rows as row (row.id)}
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
</div>
{#if isPaginated}
	<div class="flex items-center justify-end space-x-2 py-4">
		<Button
			variant="outline"
			size="sm"
			onclick={() => table.previousPage()}
			disabled={!table.getCanPreviousPage()}
		>
			Previous
		</Button>
		<Button
			variant="outline"
			size="sm"
			onclick={() => table.nextPage()}
			disabled={!table.getCanNextPage()}
		>
			Next
		</Button>
	</div>
{/if}
