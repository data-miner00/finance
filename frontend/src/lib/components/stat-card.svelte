<script lang="ts">
	import TrendingDownIcon from '@tabler/icons-svelte/icons/trending-down';
	import TrendingUpIcon from '@tabler/icons-svelte/icons/trending-up';

	import { Badge } from '$lib/components/ui/badge/index.js';
	import * as Card from '$lib/components/ui/card/index.js';

	type Props = {
		description: string;
		value: string;
		trendDirection?: 'up' | 'down';
		badgeText?: string;
		footerText?: string;
		footerSubText?: string;
	};

	let { description, value, trendDirection, badgeText, footerText, footerSubText }: Props =
		$props();
</script>

<Card.Root class="@container/card">
	<Card.Header>
		<Card.Description>{description}</Card.Description>
		<Card.Title class="text-2xl font-semibold tabular-nums @[250px]/card:text-3xl">
			{value}
		</Card.Title>
		{#if badgeText}
			<Card.Action>
				<Badge variant="outline">
					{#if trendDirection === 'down'}
						<TrendingDownIcon />
					{:else}
						<TrendingUpIcon />
					{/if}
					{badgeText}
				</Badge>
			</Card.Action>
		{/if}
	</Card.Header>
	{#if footerText || footerSubText}
		<Card.Footer class="flex-col items-start gap-1.5 text-sm">
			{#if footerText}
				<div class="line-clamp-1 flex gap-2 font-medium">
					{footerText}
					{#if trendDirection === 'down'}
						<TrendingDownIcon class="size-4" />
					{:else}
						<TrendingUpIcon class="size-4" />
					{/if}
				</div>
			{/if}
			{#if footerSubText}
				<div class="text-muted-foreground">{footerSubText}</div>
			{/if}
		</Card.Footer>
	{/if}
</Card.Root>
