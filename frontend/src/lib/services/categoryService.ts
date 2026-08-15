import { apiDelete, apiGet, apiPost, apiPut } from './api';
import type {
	Category,
	CreateCategoryRequest,
	MergeCategoriesRequest,
	UpdateCategoryRequest
} from './types';

const path = '/category';

export function categoryNameExists(categories: Category[], name: string, excludeId?: string): boolean {
	const normalized = name.trim().toLowerCase();
	return categories.some(
		(category) => category.id !== excludeId && category.name.trim().toLowerCase() === normalized
	);
}

export async function getCategories(signal?: AbortSignal): Promise<Category[]> {
	return apiGet<Category[]>(path, signal);
}

export async function getCategoryById(id: string, signal?: AbortSignal): Promise<Category> {
	return apiGet<Category>(`${path}/${id}`, signal);
}

export async function createCategory(
	request: CreateCategoryRequest,
	signal?: AbortSignal
): Promise<Category> {
	return apiPost<CreateCategoryRequest, Category>(path, request, signal);
}

export async function updateCategory(
	id: string,
	request: UpdateCategoryRequest,
	signal?: AbortSignal
): Promise<Category> {
	return apiPut<UpdateCategoryRequest, Category>(`${path}/${id}`, request, signal);
}

export async function deleteCategory(id: string, signal?: AbortSignal): Promise<void> {
	return apiDelete(`${path}/${id}`, signal);
}

export async function mergeCategories(
	request: MergeCategoriesRequest,
	signal?: AbortSignal
): Promise<void> {
	return apiPost<MergeCategoriesRequest, void>(`${path}/merge`, request, signal);
}
