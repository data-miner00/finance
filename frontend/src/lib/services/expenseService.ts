import { apiDelete, apiDownloadFile, apiGet, apiPost, apiPut } from './api';
import type { CreateExpenseRequest, Expense, UpdateExpenseRequest } from './types';

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
): Promise<Expense> {
	return apiPut<UpdateExpenseRequest, Expense>(`${path}/${id}`, request, signal);
}

export async function deleteExpense(id: string, signal?: AbortSignal): Promise<void> {
	return apiDelete(`${path}/${id}`, signal);
}

export async function exportAllExpense(signal?: AbortSignal): Promise<void> {
	const now = new Date();
	return apiDownloadFile(
		`${path}/export?format=json`,
		`export-${now.getFullYear()}-${now.getMonth() + 1}-${now.getDate()}.json`
	);
}
