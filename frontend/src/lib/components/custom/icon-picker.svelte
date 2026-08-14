<script lang="ts">
	import { categoryIconNames, categoryIcons } from '$lib/category-icons';
	import { cn } from '$lib/utils';

	type Props = {
		value: string | null | undefined;
	};

	let { value = $bindable(null) }: Props = $props();
</script>

<div class="grid grid-cols-8 gap-1.5">
	{#each categoryIconNames as name (name)}
		{@const Icon = categoryIcons[name]}
		<button
			type="button"
			class={cn(
				'flex items-center justify-center rounded-md border p-2 hover:bg-accent',
				value === name && 'border-primary bg-accent'
			)}
			onclick={() => (value = name)}
			aria-label={name}
			aria-pressed={value === name}
		>
			<Icon class="size-4" />
		</button>
	{/each}
</div>
