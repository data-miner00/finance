import { apiGet, apiPut } from './api';
import type { Setting, UpdateSettingsRequest } from './types';

const path = '/settings';

export async function getSettings(signal?: AbortSignal): Promise<Setting[]> {
	return apiGet<Setting[]>(path, signal);
}

export async function saveSettings(
	request: UpdateSettingsRequest,
	signal?: AbortSignal
): Promise<Setting[]> {
	return apiPut<UpdateSettingsRequest, Setting[]>(path, request, signal);
}
