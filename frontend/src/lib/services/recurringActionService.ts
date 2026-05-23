import { apiDelete, apiGet, apiPost, apiPut } from './api';
import type {
	RecurringAction,
	CreateRecurringActionRequest,
	UpdateRecurringActionRequest
} from './types';

const path = '/recurring';

export async function getRecurringActions(signal?: AbortSignal): Promise<RecurringAction[]> {
	return apiGet<RecurringAction[]>(path, signal);
}

export async function getRecurringActionById(
	id: string,
	signal?: AbortSignal
): Promise<RecurringAction> {
	return apiGet<RecurringAction>(`${path}/${id}`, signal);
}

export async function createRecurringAction(
	request: CreateRecurringActionRequest,
	signal?: AbortSignal
): Promise<RecurringAction> {
	return apiPost<CreateRecurringActionRequest, RecurringAction>(path, request, signal);
}

export async function updateRecurringAction(
	id: string,
	request: UpdateRecurringActionRequest,
	signal?: AbortSignal
): Promise<void> {
	return apiPut<UpdateRecurringActionRequest>(`${path}/${id}`, request, signal);
}

export async function deleteRecurringAction(id: string, signal?: AbortSignal): Promise<void> {
	return apiDelete(`${path}/${id}`, signal);
}
