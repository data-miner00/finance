import { apiGet, apiPut } from './api';
import type { Notification } from './types';

const path = '/notification';

export async function getNotifications(signal?: AbortSignal): Promise<Notification[]> {
	return apiGet<Notification[]>(path, signal);
}

export async function getUnreadNotificationCount(signal?: AbortSignal): Promise<number> {
	return apiGet<number>(`${path}/unread-count`, signal);
}

export async function markNotificationRead(
	id: string,
	signal?: AbortSignal
): Promise<Notification> {
	return apiPut<undefined, Notification>(`${path}/${id}/read`, undefined, signal);
}

export async function markAllNotificationsRead(signal?: AbortSignal): Promise<void> {
	return apiPut<undefined, void>(`${path}/read-all`, undefined, signal);
}
