import { apiDelete, apiGet, apiPost, apiPut } from './api';
import type {
	RecurringExpense,
	CreateRecurringExpenseRequest,
	UpdateRecurringExpenseRequest
} from './types';

const path = '/recurringexpense';

export async function getRecurringExpenses(signal?: AbortSignal): Promise<RecurringExpense[]> {
	return apiGet<RecurringExpense[]>(path, signal);
}

export async function getRecurringExpenseById(
	id: string,
	signal?: AbortSignal
): Promise<RecurringExpense> {
	return apiGet<RecurringExpense>(`${path}/${id}`, signal);
}

export async function createRecurringExpense(
	request: CreateRecurringExpenseRequest,
	signal?: AbortSignal
): Promise<RecurringExpense> {
	return apiPost<CreateRecurringExpenseRequest, RecurringExpense>(path, request, signal);
}

export async function updateRecurringExpense(
	id: string,
	request: UpdateRecurringExpenseRequest,
	signal?: AbortSignal
): Promise<void> {
	return apiPut<UpdateRecurringExpenseRequest>(`${path}/${id}`, request, signal);
}

export async function deleteRecurringExpense(id: string, signal?: AbortSignal): Promise<void> {
	return apiDelete(`${path}/${id}`, signal);
}
