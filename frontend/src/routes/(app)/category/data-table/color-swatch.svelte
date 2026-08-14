<script lang="ts">
	type Props = {
		color?: string | null;
	};

	let { color }: Props = $props();
</script>

{#if color}
	<div class="flex items-center gap-2">
		<span class="size-4 rounded-full border" style={`background-color: ${color}`}></span>
		<span class="text-xs text-muted-foreground">{color}</span>
	</div>
{/if}
