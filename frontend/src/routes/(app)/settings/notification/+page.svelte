<script lang="ts">
	import { onMount } from 'svelte';

	import { Button } from '$lib/components/ui/button/index.js';
	import { handleNotificationClick, refreshNotifications } from '$lib/notifications';
	import { markAllNotificationsRead } from '$lib/services';
	import { appState } from '$lib/states.svelte';

	onMount(() => {
		appState.pageTitle = 'Notification Settings';
		appState.currentPage = 'settings';
	});

	async function markAllRead() {
		await markAllNotificationsRead();
		await refreshNotifications();
	}
</script>

<div class="flex flex-col gap-4 px-4 lg:px-6">
	<div class="flex items-center justify-between">
		<h1 class="text-lg font-medium">Notifications</h1>
		<Button
			variant="outline"
			onclick={markAllRead}
			disabled={appState.unreadNotificationCount === 0}
		>
			Mark all as read
		</Button>
	</div>

	{#if appState.notifications.length === 0}
		<p class="text-sm text-muted-foreground">You're all caught up.</p>
	{:else}
		<div class="flex flex-col divide-y rounded-md border">
			{#each appState.notifications as notification (notification.id)}
				<button
					class="flex flex-col items-start gap-1 px-4 py-3 text-left hover:bg-muted/50"
					onclick={() => handleNotificationClick(notification)}
				>
					<div class="flex w-full items-center gap-2">
						{#if !notification.isRead}
							<span class="size-1.5 shrink-0 rounded-full bg-primary"></span>
						{/if}
						<span class={notification.isRead ? 'font-normal' : 'font-semibold'}>
							{notification.title}
						</span>
					</div>
					<span class="text-sm text-muted-foreground">{notification.message}</span>
					<span class="text-xs text-muted-foreground">
						{new Date(notification.createdAt).toLocaleString()}
					</span>
				</button>
			{/each}
		</div>
	{/if}
</div>
