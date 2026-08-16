<script lang="ts">
	import { BellIcon } from '@lucide/svelte';

	import { Badge } from '$lib/components/ui/badge/index.js';
	import { Button } from '$lib/components/ui/button/index.js';
	import * as DropdownMenu from '$lib/components/ui/dropdown-menu/index.js';
	import { handleNotificationClick, refreshNotifications } from '$lib/notifications';
	import { markAllNotificationsRead } from '$lib/services';
	import { appState } from '$lib/states.svelte';

	const recentNotifications = $derived(appState.notifications.slice(0, 10));

	async function markAllRead() {
		await markAllNotificationsRead();
		await refreshNotifications();
	}
</script>

<DropdownMenu.Root>
	<DropdownMenu.Trigger>
		{#snippet child({ props })}
			<Button {...props} variant="ghost" size="icon" class="relative">
				<span class="sr-only">Notifications</span>
				<BellIcon />
				{#if appState.unreadNotificationCount > 0}
					<Badge
						variant="destructive"
						class="absolute -top-1 -right-1 h-4 min-w-4 justify-center rounded-full px-1 text-[10px]"
					>
						{appState.unreadNotificationCount > 99 ? '99+' : appState.unreadNotificationCount}
					</Badge>
				{/if}
			</Button>
		{/snippet}
	</DropdownMenu.Trigger>
	<DropdownMenu.Content align="end" class="w-80">
		<DropdownMenu.Label class="flex items-center justify-between">
			Notifications
			{#if appState.unreadNotificationCount > 0}
				<button class="text-xs font-normal text-primary hover:underline" onclick={markAllRead}>
					Mark all as read
				</button>
			{/if}
		</DropdownMenu.Label>
		<DropdownMenu.Separator />
		<div class="max-h-96 overflow-y-auto">
			{#if recentNotifications.length === 0}
				<p class="p-4 text-center text-sm text-muted-foreground">You're all caught up.</p>
			{:else}
				{#each recentNotifications as notification (notification.id)}
					<DropdownMenu.Item
						class="flex flex-col items-start gap-0.5 whitespace-normal"
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
						<span class="text-xs text-muted-foreground">{notification.message}</span>
						<span class="text-xs text-muted-foreground">
							{new Date(notification.createdAt).toLocaleString()}
						</span>
					</DropdownMenu.Item>
				{/each}
			{/if}
		</div>
		<DropdownMenu.Separator />
		<DropdownMenu.Item>
			{#snippet child({ props })}
				<a {...props} href="/notification" class="w-full text-center">View all</a>
			{/snippet}
		</DropdownMenu.Item>
	</DropdownMenu.Content>
</DropdownMenu.Root>
