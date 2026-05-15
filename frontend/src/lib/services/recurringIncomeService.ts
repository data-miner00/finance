import { apiDelete, apiGet, apiPost, apiPut } from './api';
import type {
	RecurringIncome,
	CreateRecurringIncomeRequest,
	UpdateRecurringIncomeRequest
} from './types';

const path = '/recurringincome';

export async function getRecurringIncomes(signal?: AbortSignal): Promise<RecurringIncome[]> {
	return apiGet<RecurringIncome[]>(path, signal);
}

export async function getRecurringIncomeById(
	id: string,
	signal?: AbortSignal
): Promise<RecurringIncome> {
	return apiGet<RecurringIncome>(`${path}/${id}`, signal);
}

export async function createRecurringIncome(
	request: CreateRecurringIncomeRequest,
	signal?: AbortSignal
): Promise<RecurringIncome> {
	return apiPost<CreateRecurringIncomeRequest, RecurringIncome>(path, request, signal);
}

export async function updateRecurringIncome(
	id: string,
	request: UpdateRecurringIncomeRequest,
	signal?: AbortSignal
): Promise<void> {
	return apiPut<UpdateRecurringIncomeRequest>(`${path}/${id}`, request, signal);
}

export async function deleteRecurringIncome(id: string, signal?: AbortSignal): Promise<void> {
	return apiDelete(`${path}/${id}`, signal);
}
