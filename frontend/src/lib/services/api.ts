import { PUBLIC_API_BASE_URL } from '$env/static/public';

const apiBase = PUBLIC_API_BASE_URL + '/api';

interface RequestOptions extends RequestInit {
	signal?: AbortSignal;
}

async function handleResponse<T>(response: Response): Promise<T> {
	if (!response.ok) {
		const text = await response.text();
		const error = new Error(
			`API request failed with status ${response.status} ${response.statusText}: ${text}`
		);
		throw error;
	}

	if (response.status === 204) {
		return undefined as unknown as T;
	}

	return response.json() as Promise<T>;
}

export async function apiGet<T>(path: string, signal?: AbortSignal): Promise<T> {
	const response = await fetch(`${apiBase}${path}`, {
		method: 'GET',
		headers: {
			Accept: 'application/json'
		},
		signal
	});

	return handleResponse<T>(response);
}

export async function apiPost<TRequest, TResponse>(
	path: string,
	body: TRequest,
	signal?: AbortSignal
): Promise<TResponse> {
	const response = await fetch(`${apiBase}${path}`, {
		method: 'POST',
		headers: {
			Accept: 'application/json',
			'Content-Type': 'application/json'
		},
		body: JSON.stringify(body),
		signal
	});

	return handleResponse<TResponse>(response);
}

export async function apiPut<TRequest>(
	path: string,
	body: TRequest,
	signal?: AbortSignal
): Promise<void> {
	const response = await fetch(`${apiBase}${path}`, {
		method: 'PUT',
		headers: {
			'Content-Type': 'application/json'
		},
		body: JSON.stringify(body),
		signal
	});

	await handleResponse<void>(response);
}

export async function apiDelete(path: string, signal?: AbortSignal): Promise<void> {
	const response = await fetch(`${apiBase}${path}`, {
		method: 'DELETE',
		signal
	});

	await handleResponse<void>(response);
}
