import { apiDelete, apiGet, apiPost, apiPut } from './api';
import type { PiggyBank, CreatePiggyBankRequest, UpdatePiggyBankRequest } from './types';

const path = '/piggybank';

export async function getPiggyBanks(signal?: AbortSignal): Promise<PiggyBank[]> {
	return apiGet<PiggyBank[]>(path, signal);
}

export async function getPiggyBankById(id: string, signal?: AbortSignal): Promise<PiggyBank> {
	return apiGet<PiggyBank>(`${path}/${id}`, signal);
}

export async function createPiggyBank(
	request: CreatePiggyBankRequest,
	signal?: AbortSignal
): Promise<PiggyBank> {
	return apiPost<CreatePiggyBankRequest, PiggyBank>(path, request, signal);
}

export async function updatePiggyBank(
	id: string,
	request: UpdatePiggyBankRequest,
	signal?: AbortSignal
): Promise<void> {
	return apiPut<UpdatePiggyBankRequest>(`${path}/${id}`, request, signal);
}

export async function deletePiggyBank(id: string, signal?: AbortSignal): Promise<void> {
	return apiDelete(`${path}/${id}`, signal);
}
