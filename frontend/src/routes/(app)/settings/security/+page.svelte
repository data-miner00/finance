<script lang="ts">
	import { onMount } from 'svelte';

	import { appState } from '$lib/states.svelte';

	onMount(() => {
		appState.pageTitle = 'Security Settings';
		appState.currentPage = 'settings';
	});
</script>

<h1>Security Settings</h1>
