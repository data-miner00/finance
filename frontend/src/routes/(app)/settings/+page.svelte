<script lang="ts">
	import { onMount } from 'svelte';
	import { toast } from 'svelte-sonner';

	import { Button } from '$lib/components/ui/button/index.js';
	import { Input } from '$lib/components/ui/input';
	import * as Label from '$lib/components/ui/label/index.js';
	import Separator from '$lib/components/ui/separator/separator.svelte';
	import { Switch } from '$lib/components/ui/switch';
	import { parseSettings, saveSettings, toSettingsValues } from '$lib/services';
	import { appState } from '$lib/states.svelte';

	async function saveChanges() {
		try {
			const updated = await saveSettings({
				values: toSettingsValues({
					pieChartDisplayTop: tempPieChartDisplayTop,
					enableTax: tempEnableTax
				})
			});
			appState.settings = parseSettings(updated);
			toast.success('Settings saved');
		} catch (error) {
			toast.error('Failed to save settings');
		}
	}

	onMount(() => {
		appState.pageTitle = 'General Settings';
		appState.currentPage = 'settings';
	});

	let tempPieChartDisplayTop = $state(appState.settings.pieChartDisplayTop);
	let tempEnableTax = $state(appState.settings.enableTax);
</script>

<h1 class="text-2xl font-bold">General Settings</h1>

<p>Manage your general application settings and preferences here.</p>

<section class="mt-6 max-w-sm">
	<h2 class="mb-1 text-lg font-semibold">Display Preferences</h2>

	<p class="mb-4 text-sm text-muted-foreground">Customize how your data is displayed.</p>

	<Label.Root class="mb-3" for="pieChartDisplayTop">Pie Chart Display Top</Label.Root>

	<Input
		name="pieChartDisplayTop"
		placeholder="e.g. 10"
		min={1}
		max={10}
		bind:value={tempPieChartDisplayTop}
		type="number"
		class="mb-4"
	/>

	<Separator class="my-6 " />

	<h2 class="mb-1 text-lg font-semibold">Features</h2>

	<p class="mb-4 text-sm text-muted-foreground">Enable or disable optional features.</p>

	<div class="mb-4 flex items-center justify-between">
		<Label.Root for="enableTax">Enable Tax</Label.Root>
		<Switch id="enableTax" bind:checked={tempEnableTax} />
	</div>

	<Separator class="my-6 " />

	<div class="grid w-full gap-4">
		<Button size="sm" variant="outline" onclick={saveChanges}>Save Changes</Button>
	</div>
</section>
