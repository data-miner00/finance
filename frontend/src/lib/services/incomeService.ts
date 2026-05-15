import { apiDelete, apiGet, apiPost, apiPut } from './api';
import type { Income, CreateIncomeRequest, UpdateIncomeRequest } from './types';

const path = '/income';

export async function getIncomes(signal?: AbortSignal): Promise<Income[]> {
	return apiGet<Income[]>(path, signal);
}

export async function getIncomeById(id: string, signal?: AbortSignal): Promise<Income> {
	return apiGet<Income>(`${path}/${id}`, signal);
}

export async function createIncome(
	request: CreateIncomeRequest,
	signal?: AbortSignal
): Promise<Income> {
	return apiPost<CreateIncomeRequest, Income>(path, request, signal);
}

export async function updateIncome(
	id: string,
	request: UpdateIncomeRequest,
	signal?: AbortSignal
): Promise<void> {
	return apiPut<UpdateIncomeRequest>(`${path}/${id}`, request, signal);
}

export async function deleteIncome(id: string, signal?: AbortSignal): Promise<void> {
	return apiDelete(`${path}/${id}`, signal);
}
