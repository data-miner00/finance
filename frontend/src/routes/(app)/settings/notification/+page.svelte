<script lang="ts">
	import { onMount } from 'svelte';

	import { appState } from '$lib/states.svelte';

	onMount(() => {
		appState.pageTitle = 'Notification Settings';
		appState.currentPage = 'settings';
	});
</script>

<h1>Notification Settings</h1>
