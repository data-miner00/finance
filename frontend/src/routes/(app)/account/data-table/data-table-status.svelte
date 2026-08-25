<script lang="ts">
	import AlertTriangleIcon from '@tabler/icons-svelte/icons/alert-triangle';
	import CircleCheckFilledIcon from '@tabler/icons-svelte/icons/circle-check-filled';
	import CircleXFilledIcon from '@tabler/icons-svelte/icons/circle-x-filled';

	import { Badge } from '$lib/components/ui/badge/index.js';

	let { progress }: { progress: number } = $props();
</script>

{#if progress > 100}
	<Badge variant="destructive" class="px-1.5">
		<CircleXFilledIcon /> Over Target
	</Badge>
{:else if progress >= 80}
	<Badge variant="secondary" class="px-1.5">
		<AlertTriangleIcon /> Near Target
	</Badge>
{:else}
	<Badge variant="outline" class="px-1.5 text-muted-foreground">
		<CircleCheckFilledIcon class="fill-green-500 dark:fill-green-400" /> On Track
	</Badge>
{/if}
