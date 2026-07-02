<script lang="ts">
	import CircleCheckFilledIcon from '@tabler/icons-svelte/icons/circle-check-filled';
	import LoaderIcon from '@tabler/icons-svelte/icons/loader';

	import { Badge } from '$lib/components/ui/badge/index.js';

	let { progress }: { progress: number } = $props();
</script>

<Badge variant="outline" class="px-1.5 text-muted-foreground">
	{#if progress === 100}
		<CircleCheckFilledIcon class="fill-green-500 dark:fill-green-400" /> Completed
	{:else}
		<LoaderIcon /> Active
	{/if}
</Badge>
