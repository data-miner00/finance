<script lang="ts">
	import { page } from '$app/state';

	let { children } = $props();
</script>

<div class="mt-10 flex px-4">
	<aside class="flex basis-72 flex-col gap-5">
		<a href="/settings" class:underline={page.url.pathname === '/settings'}>General</a>
		<a href="/settings/profile" class:underline={page.url.pathname === '/settings/profile'}
			>Profile Related</a
		>
		<a
			href="/settings/notification"
			class:underline={page.url.pathname === '/settings/notification'}>Notification</a
		>
		<a href="/settings/security" class:underline={page.url.pathname === '/settings/security'}
			>Security Settings</a
		>
		<a href="/settings/data" class:underline={page.url.pathname === '/settings/data'}
			>Data Management</a
		>
	</aside>

	<div class="flex flex-1 flex-col gap-4 p-4 pt-0">
		{@render children()}
	</div>
</div>
