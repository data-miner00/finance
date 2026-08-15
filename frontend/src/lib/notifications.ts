import { goto } from '$app/navigation';

import { getNotifications, getUnreadNotificationCount, markNotificationRead } from './services';
import type { Notification } from './services/types';
import { appState } from './states.svelte';

export async function refreshNotifications(): Promise<void> {
	const [notifications, unreadCount] = await Promise.all([
		getNotifications(),
		getUnreadNotificationCount()
	]);
	appState.notifications = notifications;
	appState.unreadNotificationCount = unreadCount;
}

export async function handleNotificationClick(notification: Notification): Promise<void> {
	if (!notification.isRead) {
		await markNotificationRead(notification.id);
		await refreshNotifications();
	}

	if (notification.entityType === 'Category') {
		await goto('/budget');
	} else if (notification.entityType === 'Expense') {
		await goto('/expense');
	} else if (notification.entityType === 'Income') {
		await goto('/income');
	}
}
