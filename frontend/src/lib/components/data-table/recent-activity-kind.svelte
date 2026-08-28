<script lang="ts">
	import TrendingDownIcon from '@tabler/icons-svelte/icons/trending-down';
	import TrendingUpIcon from '@tabler/icons-svelte/icons/trending-up';

	import { Badge } from '$lib/components/ui/badge/index.js';

	let { kind }: { kind: 'expense' | 'income' } = $props();
</script>

{#if kind === 'income'}
	<Badge variant="outline" class="px-1.5 text-muted-foreground">
		<TrendingUpIcon class="text-green-600 dark:text-green-400" /> Income
	</Badge>
{:else}
	<Badge variant="outline" class="px-1.5 text-muted-foreground">
		<TrendingDownIcon class="text-red-600 dark:text-red-400" /> Expense
	</Badge>
{/if}
