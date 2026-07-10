<script lang="ts">
	import { DownloadIcon } from '@lucide/svelte';
	import { toast } from 'svelte-sonner';

	import { Button } from '$lib/components/ui/button';
	import { Input } from '$lib/components/ui/input/index.js';
	import { exportAllExpense, importExpenses } from '$lib/services';

	async function exportAllData() {
		try {
			await exportAllExpense();
			toast.success('Successfully exported expenses.');
		} catch (e) {
			toast.error('Failed to export expenses. ' + e);
		}
	}

	let files: FileList | undefined = $state();

	async function importAllData() {
		if (!files?.[0]) return;
		const file = files[0];

		try {
			await importExpenses(file);
			toast.success('Imported successfully.');
		} catch (e) {
			toast.error('Failed to import. ' + e);
		}
	}
</script>

<h1 class="text-2xl font-bold">Data Management</h1>

<p>Manage your data that is being used in this application.</p>

<section class="mt-6 max-w-sm">
	<h2 class="mb-1 text-lg font-semibold">Expense Data</h2>

	<p class="mb-4 text-sm text-muted-foreground">Export or import your expense data at any time.</p>

	<div class="flex items-center gap-2">
		<Button variant="outline" onclick={exportAllData}>
			<DownloadIcon />
			Download as JSON
		</Button>

		<div>
			<Input
				bind:files
				type="file"
				accept=".json"
				placeholder="Import"
				name="importExpenses"
				onchange={importAllData}
			/>
		</div>
	</div>
</section>
