<script lang="ts">
	import { onMount } from 'svelte';
	import { toast } from 'svelte-sonner';

	import { Button } from '$lib/components/ui/button/index.js';
	import { Input } from '$lib/components/ui/input';
	import * as Label from '$lib/components/ui/label/index.js';
	import * as Select from '$lib/components/ui/select/index.js';
	import Separator from '$lib/components/ui/separator/separator.svelte';
	import { Switch } from '$lib/components/ui/switch';
	import { CURRENCIES, CURRENCY_LABELS, type CurrencyCode } from '$lib/constants/currencies';
	import { parseSettings, saveSettings, toSettingsValues } from '$lib/services';
	import { appState } from '$lib/states.svelte';

	async function saveChanges() {
		try {
			const updated = await saveSettings({
				values: toSettingsValues({
					pieChartDisplayTop: tempPieChartDisplayTop,
					enableTax: tempEnableTax,
					enablePiggyBank: tempEnablePiggyBank,
					defaultCurrency: tempDefaultCurrency,
					enableTablePagination: tempEnableTablePagination
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
	let tempEnablePiggyBank = $state(appState.settings.enablePiggyBank);
	let tempDefaultCurrency = $state(appState.settings.defaultCurrency);
	let tempEnableTablePagination = $state(appState.settings.enableTablePagination);

	// For page hard refresh
	$effect(() => {
		tempPieChartDisplayTop = appState.settings.pieChartDisplayTop;
		tempEnableTax = appState.settings.enableTax;
		tempEnablePiggyBank = appState.settings.enablePiggyBank;
		tempDefaultCurrency = appState.settings.defaultCurrency;
		tempEnableTablePagination = appState.settings.enableTablePagination;
	});
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

	<Label.Root class="mb-3" for="defaultCurrency">Default Currency</Label.Root>

	<Select.Root type="single" name="defaultCurrency" bind:value={tempDefaultCurrency}>
		<Select.Trigger id="defaultCurrency" class="mb-4 w-full">
			{CURRENCY_LABELS[tempDefaultCurrency as CurrencyCode] ?? tempDefaultCurrency}
		</Select.Trigger>
		<Select.Content>
			{#each CURRENCIES as code (code)}
				<Select.Item value={code} label={CURRENCY_LABELS[code]}>
					{CURRENCY_LABELS[code]}
				</Select.Item>
			{/each}
		</Select.Content>
	</Select.Root>

	<div class="mb-4 flex items-center justify-between">
		<Label.Root for="enableTablePagination">Paginate Tables by Default</Label.Root>
		<Switch id="enableTablePagination" bind:checked={tempEnableTablePagination} />
	</div>

	<Separator class="my-6 " />

	<h2 class="mb-1 text-lg font-semibold">Features</h2>

	<p class="mb-4 text-sm text-muted-foreground">Enable or disable optional features.</p>

	<div class="mb-4 flex items-center justify-between">
		<Label.Root for="enableTax">Enable Tax</Label.Root>
		<Switch id="enableTax" bind:checked={tempEnableTax} />
	</div>

	<div class="mb-4 flex items-center justify-between">
		<Label.Root for="enablePiggyBank">Enable Piggy Bank</Label.Root>
		<Switch id="enablePiggyBank" bind:checked={tempEnablePiggyBank} />
	</div>

	<Separator class="my-6 " />

	<div class="grid w-full gap-4">
		<Button size="sm" variant="outline" onclick={saveChanges}>Save Changes</Button>
	</div>
</section>
