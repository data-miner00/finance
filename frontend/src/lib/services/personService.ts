import { apiDelete, apiGet, apiPost, apiPut } from './api';
import type { CreatePersonRequest, UpdatePersonRequest, Person } from './types';

const path = '/person';

export async function getPeople(signal?: AbortSignal): Promise<Person[]> {
	return apiGet<Person[]>(path, signal);
}

export async function getPersonById(id: string, signal?: AbortSignal): Promise<Person> {
	return apiGet<Person>(`${path}/${id}`, signal);
}

export async function createPerson(
	request: CreatePersonRequest,
	signal?: AbortSignal
): Promise<Person> {
	return apiPost<CreatePersonRequest, Person>(path, request, signal);
}

export async function updatePerson(
	id: string,
	request: UpdatePersonRequest,
	signal?: AbortSignal
): Promise<Person> {
	return apiPut<UpdatePersonRequest, Person>(`${path}/${id}`, request, signal);
}

export async function deletePerson(id: string, signal?: AbortSignal): Promise<void> {
	return apiDelete(`${path}/${id}`, signal);
}
