import { apiDelete, apiGet, apiPost, apiPut } from './api';
import type { Expense, CreateExpenseRequest, UpdateExpenseRequest } from './types';

const path = '/expense';

export async function getExpenses(signal?: AbortSignal): Promise<Expense[]> {
	return apiGet<Expense[]>(path, signal);
}

export async function getExpenseById(id: string, signal?: AbortSignal): Promise<Expense> {
	return apiGet<Expense>(`${path}/${id}`, signal);
}

export async function createExpense(
	request: CreateExpenseRequest,
	signal?: AbortSignal
): Promise<Expense> {
	return apiPost<CreateExpenseRequest, Expense>(path, request, signal);
}

export async function updateExpense(
	id: string,
	request: UpdateExpenseRequest,
	signal?: AbortSignal
): Promise<void> {
	return apiPut<UpdateExpenseRequest>(`${path}/${id}`, request, signal);
}

export async function deleteExpense(id: string, signal?: AbortSignal): Promise<void> {
	return apiDelete(`${path}/${id}`, signal);
}
