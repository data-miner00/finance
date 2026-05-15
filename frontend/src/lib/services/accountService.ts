import { apiDelete, apiGet, apiPost, apiPut } from './api';
import type { Account, CreateAccountRequest, UpdateAccountRequest } from './types';

const path = '/account';

export async function getAccounts(signal?: AbortSignal): Promise<Account[]> {
	return apiGet<Account[]>(path, signal);
}

export async function getAccountById(id: string, signal?: AbortSignal): Promise<Account> {
	return apiGet<Account>(`${path}/${id}`, signal);
}

export async function createAccount(
	request: CreateAccountRequest,
	signal?: AbortSignal
): Promise<Account> {
	return apiPost<CreateAccountRequest, Account>(path, request, signal);
}

export async function updateAccount(
	id: string,
	request: UpdateAccountRequest,
	signal?: AbortSignal
): Promise<void> {
	return apiPut<UpdateAccountRequest>(`${path}/${id}`, request, signal);
}

export async function deleteAccount(id: string, signal?: AbortSignal): Promise<void> {
	return apiDelete(`${path}/${id}`, signal);
}
