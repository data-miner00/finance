import { apiGet, apiPut } from './api';
import type { Profile, SaveProfileRequest } from './types';

const path = '/profile';

export async function getProfile(signal?: AbortSignal): Promise<Profile | undefined> {
	return apiGet<Profile | undefined>(path, signal);
}

export async function saveProfile(
	request: SaveProfileRequest,
	signal?: AbortSignal
): Promise<Profile> {
	return apiPut<SaveProfileRequest, Profile>(path, request, signal);
}
