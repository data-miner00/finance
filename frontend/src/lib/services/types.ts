export enum AccountType {
	Savings,
	EWallet,
	Cash,
	App,
	CreditCard
}

export interface EntityBase {
	id: string;
	createdAt: string;
	updatedAt: string;
}

export interface Account extends EntityBase {
	name: string;
	description?: string | null;
	type: AccountType;
	balance: number;
	currency: string;
	annualSpendTarget?: number | null;
}

export interface CreateAccountRequest {
	name: string;
	description?: string | null;
	accountType: AccountType;
	balance: number;
	currency: string;
	annualSpendTarget?: number | null;
}

export interface UpdateAccountRequest extends CreateAccountRequest {}

export interface Category extends EntityBase {
	name: string;
	color?: string | null;
	icon?: string | null;
	budgetAmount?: number | null;
}

export interface CreateCategoryRequest {
	name: string;
	color?: string | null;
	icon?: string | null;
	budgetAmount?: number | null;
}

export interface UpdateCategoryRequest extends CreateCategoryRequest {}

export interface MergeCategoriesRequest {
	sourceCategoryId: string;
	targetCategoryId: string;
}

export interface Expense extends EntityBase {
	categoryName?: string;
	accountId?: string | null;
	accountName?: string;
	name: string;
	description?: string | null;
	amount: number;
	currency: string;
	location?: string | null;
	receiptImage?: string | null;
	actionedAt: string;
	agentName?: string;
}

export interface CreateExpenseRequest {
	categoryName?: string;
	accountId?: string | null;
	name: string;
	description?: string | null;
	amount: number;
	currency: string;
	location?: string | null;
	receiptImage?: string | null;
	agentName?: string;
}

export interface UpdateExpenseRequest extends CreateExpenseRequest {
	actionedAt: string;
}

export interface Income extends EntityBase {
	accountId?: string | null;
	accountName?: string;
	name: string;
	description?: string | null;
	amount: number;
	currency: string;
	actionedAt: string;
}

export interface CreateIncomeRequest {
	name: string;
	description?: string | null;
	amount: number;
	currency: string;
	accountId?: string | null;
}

export interface UpdateIncomeRequest extends CreateIncomeRequest {}

export interface PiggyBank extends EntityBase {
	name: string;
	description?: string | null;
	amount: number;
	target: number;
	currency?: string | null;
	deadline?: string | null;
}

export interface CreatePiggyBankRequest {
	name: string;
	description?: string | null;
	amount: number;
	target: number;
	currency?: string | null;
	deadline?: string | null;
}

export interface UpdatePiggyBankRequest extends CreatePiggyBankRequest {}

export interface RecurringAction extends EntityBase {
	name: string;
	description?: string | null;
	isActive: boolean;
	amount: number;
	currency: string;
	recurringAt: string;
	startAt: string;
	recurrenceType: number;
	intervalValue: number;
	dayOfMonth?: number | null;
	type: number;
	lastExecutedAt?: string;
}

export interface CreateRecurringActionRequest {
	name: string;
	description?: string | null;
	isActive: boolean;
	amount: number;
	currency: string;
	startAt: string;
	recurrenceType: number;
	intervalValue: number;
	dayOfMonth?: number | null;
	type: number;
}

export interface UpdateRecurringActionRequest extends Omit<CreateRecurringActionRequest, 'type'> {}

export interface Tax extends EntityBase {
	name: string;
	description?: string | null;
	amount: number;
}

export interface CreateTaxRequest {
	name: string;
	description?: string | null;
	amount: number;
}

export interface UpdateTaxRequest extends CreateTaxRequest {}

export interface Profile extends EntityBase {
	username: string;
	firstName?: string | null;
	lastName?: string | null;
	email?: string | null;
	bio?: string | null;
	companyName?: string | null;
	websiteUrl?: string | null;
	avatarImage?: string | null;
}

export interface SaveProfileRequest {
	username: string;
	firstName?: string | null;
	lastName?: string | null;
	email?: string | null;
	bio?: string | null;
	companyName?: string | null;
	websiteUrl?: string | null;
	avatarImage?: string | null;
}

export interface Notification extends EntityBase {
	type: string;
	title: string;
	message: string;
	isRead: boolean;
	entityType?: string | null;
	entityId?: string | null;
}

export interface Setting extends EntityBase {
	key: string;
	value: string;
}

export interface UpdateSettingsRequest {
	values: Record<string, string>;
}
