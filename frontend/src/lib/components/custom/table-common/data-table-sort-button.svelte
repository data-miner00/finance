<script lang="ts">
	import ArrowUpDownIcon from '@lucide/svelte/icons/arrow-up-down';
	import type { ComponentProps } from 'svelte';

	import { Button } from '$lib/components/ui/button/index.js';

	let { variant = 'ghost', title, ...restProps }: ComponentProps<typeof Button> = $props();
</script>

<Button {variant} {...restProps}>
	{title}
	<ArrowUpDownIcon class="ms-2" />
</Button>
