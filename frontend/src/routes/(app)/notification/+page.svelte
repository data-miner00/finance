<script lang="ts">
	import { onMount } from 'svelte';

	import { Button } from '$lib/components/ui/button/index.js';
	import { handleNotificationClick, refreshNotifications } from '$lib/notifications';
	import { markAllNotificationsRead } from '$lib/services';
	import { appState } from '$lib/states.svelte';

	onMount(() => {
		appState.pageTitle = 'Notifications';
		appState.currentPage = 'notification';
	});

	async function markAllRead() {
		await markAllNotificationsRead();
		await refreshNotifications();
	}
</script>

<div class="flex flex-col gap-4 py-4 md:gap-6 md:py-6">
	<div class="px-4 lg:px-6">
		<div class="flex max-w-4xl items-center justify-between gap-4">
			<div class="space-y-2">
				<h1 class="text-xl font-bold">Notifications</h1>
				<p class="text-sm text-muted-foreground">
					Review recent activity and alerts across your accounts, expenses, and budgets.
				</p>
			</div>
			<Button
				variant="outline"
				onclick={markAllRead}
				disabled={appState.unreadNotificationCount === 0}
			>
				Mark all as read
			</Button>
		</div>
	</div>

	<div class="px-4 lg:px-6">
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
</div>
