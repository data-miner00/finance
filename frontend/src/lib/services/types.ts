export type AccountType = 0 | 1 | 2 | 3 | 4;

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
	currency?: string | null;
}

export interface CreateAccountRequest {
	name: string;
	description?: string | null;
	accountType: AccountType;
	balance: number;
	currency?: string | null;
}

export interface UpdateAccountRequest extends CreateAccountRequest {}

export interface Expense extends EntityBase {
	categoryId?: string;
	name: string;
	description?: string | null;
	amount: number;
	currency?: string | null;
	location?: string | null;
	receiptImage?: string | null;
	actionedAt: string;
}

export interface CreateExpenseRequest {
	categoryId?: string;
	name: string;
	description?: string | null;
	amount: number;
	currency?: string | null;
	location?: string | null;
	receiptImage?: string | null;
	actionedAt?: string | null;
}

export interface UpdateExpenseRequest extends CreateExpenseRequest {}

export interface Income extends EntityBase {
	name: string;
	description?: string | null;
	amount: number;
}

export interface CreateIncomeRequest {
	name: string;
	description?: string | null;
	amount: number;
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
	recurringAt: string;
	startAt: string;
	recurrenceType: string;
	intervalValue: number;
	dayOfMonth?: number | null;
	type: number;
}

export interface CreateRecurringActionRequest {
	name: string;
	description?: string | null;
	isActive: boolean;
	amount: number;
	startAt: string;
	recurrenceType: string;
	intervalValue: number;
	dayOfMonth?: number | null;
	type: number;
}

export interface UpdateRecurringActionRequest extends Omit<CreateRecurringActionRequest, 'type'> {}
