<script lang="ts">
	import { page } from '$app/state';
	import Button from '$lib/components/ui/button/button.svelte';

	const emojis: Record<number, string> = {
		404: '👻',
		420: '🫠',
		500: '💥'
	};
</script>

<div class="flex h-full w-full items-center justify-center">
	<div class="mx-auto my-auto w-fit text-center">
		<h1 class="text-2xl font-bold">{page.status} {page.error?.message}</h1>
		<span style="font-size: 10em">
			{emojis[page.status] ?? emojis[500]}
		</span>

		<p>Looks like something is acting up. Apologies for the inconvenience!</p>

		<div class="mt-4">
			<Button onclick={() => history.back()}>Go back</Button>
		</div>
	</div>
</div>
