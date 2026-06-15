import { apiDelete, apiGet, apiPost, apiPut } from './api';
import type { CreateTaxRequest, Tax, UpdateTaxRequest } from './types';

const path = '/tax';

export async function getTaxes(signal?: AbortSignal): Promise<Tax[]> {
	return apiGet<Tax[]>(path, signal);
}

export async function getTaxById(id: string, signal?: AbortSignal): Promise<Tax> {
	return apiGet<Tax>(`${path}/${id}`, signal);
}

export async function createTax(request: CreateTaxRequest, signal?: AbortSignal): Promise<Tax> {
	return apiPost<CreateTaxRequest, Tax>(path, request, signal);
}

export async function updateTax(
	id: string,
	request: UpdateTaxRequest,
	signal?: AbortSignal
): Promise<Tax> {
	return apiPut<UpdateTaxRequest, Tax>(`${path}/${id}`, request, signal);
}

export async function deleteTax(id: string, signal?: AbortSignal): Promise<void> {
	return apiDelete(`${path}/${id}`, signal);
}
