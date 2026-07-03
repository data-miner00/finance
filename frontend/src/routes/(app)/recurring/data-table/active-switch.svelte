<script lang="ts">
	import { toast } from 'svelte-sonner';

	import { Switch } from '$lib/components/ui/switch/index.js';
	import { toggleRecurringActionActiveStatus } from '$lib/services';

	type Props = {
		id: string;
		checked: boolean;
	};

	let { checked = $bindable(false), id }: Props = $props();

	function onCheckedChange(newValue: boolean) {
		toggleRecurringActionActiveStatus(id)
			.then(() => {
				checked = newValue;
				toast.success(`Recurring action ${newValue ? 'activated' : 'deactivated'}`);
			})
			.catch((error) => {
				console.error('Error toggling active status:', error);
				toast.error('Failed to update recurring action status.');
			});
	}
</script>

<div class="flex items-center space-x-2">
	<Switch name="is-active" {checked} {onCheckedChange} />
</div>
