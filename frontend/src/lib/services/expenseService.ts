import {
	apiDelete,
	apiDeleteForJson,
	apiDownloadFile,
	apiGet,
	apiPost,
	apiPut,
	apiUploadFile,
	apiUploadFileForJson
} from './api';
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

export async function importExpenses(jsonFile: File, signal?: AbortSignal) {
	const formData = new FormData();
	formData.append('file', jsonFile);

	return apiUploadFile(`${path}/import`, formData);
}

export async function uploadExpenseReceipt(id: string, file: File): Promise<Expense> {
	const formData = new FormData();
	formData.append('file', file);

	return apiUploadFileForJson<Expense>(`${path}/${id}/receipt`, formData);
}

export async function deleteExpenseReceipt(id: string, signal?: AbortSignal): Promise<Expense> {
	return apiDeleteForJson<Expense>(`${path}/${id}/receipt`, signal);
}
