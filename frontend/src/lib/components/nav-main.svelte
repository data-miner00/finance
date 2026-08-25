<script lang="ts">
	import CirclePlusFilledIcon from '@tabler/icons-svelte/icons/circle-plus-filled';

	import * as Sidebar from '$lib/components/ui/sidebar/index.js';
	import { appState } from '$lib/states.svelte';

	function toggleAddTransactionDialog() {
		appState.openAddDialog = appState.openAddDialog === 'transaction' ? null : 'transaction';
	}
</script>

<Sidebar.Group>
	<Sidebar.GroupContent class="flex flex-col gap-2">
		<Sidebar.Menu>
			<Sidebar.MenuItem class="flex items-center gap-2">
				<Sidebar.MenuButton
					class="min-w-8 bg-primary text-primary-foreground duration-200 ease-linear hover:bg-primary/90 hover:text-primary-foreground active:bg-primary/90 active:text-primary-foreground"
					tooltipContent="Quick create"
					onclick={toggleAddTransactionDialog}
				>
					<CirclePlusFilledIcon />
					<span>Quick Create</span>
				</Sidebar.MenuButton>
			</Sidebar.MenuItem>
		</Sidebar.Menu>
	</Sidebar.GroupContent>
</Sidebar.Group>
