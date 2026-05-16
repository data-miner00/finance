export type AccountType = 0 | 1 | 2 | 3 | 4;

export interface EntityBase {
	id: string;
}

export interface Account extends EntityBase {
	name: string;
	description?: string | null;
	accountType: AccountType;
	amount: number;
	currency?: string | null;
	actionedAt: string;
	createdAt: string;
	updatedAt: string;
}

export interface CreateAccountRequest {
	name: string;
	description?: string | null;
	accountType: AccountType;
	amount: number;
	currency?: string | null;
	actionedAt: string;
}

export interface UpdateAccountRequest extends CreateAccountRequest {}

export interface Expense extends EntityBase {
	category: string;
	name: string;
	description?: string | null;
	amount: number;
	currency?: string | null;
	location?: string | null;
	receiptImage?: string | null;
	actionedAt: string;
	createdAt: string;
}

export interface CreateExpenseRequest {
	category: string;
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
	createdAt: string;
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
	createdAt: string;
	updatedAt: string;
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

export interface RecurringExpense extends EntityBase {
	name: string;
	description?: string | null;
	isActive: boolean;
	executedAt: string;
	createdAt: string;
}

export interface CreateRecurringExpenseRequest {
	name: string;
	description?: string | null;
	isActive: boolean;
	executedAt: string;
}

export interface UpdateRecurringExpenseRequest extends CreateRecurringExpenseRequest {}

export interface RecurringIncome extends EntityBase {
	name: string;
	description?: string | null;
	isActive: boolean;
	executedAt: string;
	createdAt: string;
}

export interface CreateRecurringIncomeRequest {
	name: string;
	description?: string | null;
	isActive: boolean;
	executedAt: string;
}

export interface UpdateRecurringIncomeRequest extends CreateRecurringIncomeRequest {}
