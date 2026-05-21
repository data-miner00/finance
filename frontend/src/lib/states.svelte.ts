export type AppState = {
	accounts: Awaited<ReturnType<typeof import('./services').getAccounts>>;
	expenses: Awaited<ReturnType<typeof import('./services').getExpenses>>;
	incomes: Awaited<ReturnType<typeof import('./services').getIncomes>>;
	recurringExpenses: Awaited<ReturnType<typeof import('./services').getRecurringExpenses>>;
	recurringIncomes: Awaited<ReturnType<typeof import('./services').getRecurringIncomes>>;
	piggyBanks: Awaited<ReturnType<typeof import('./services').getPiggyBanks>>;
	isAddTransactionDialogOpen: boolean;
	pageTitle: string;
};

export const appState = $state<AppState>({
	pageTitle: 'Dashboard',
	accounts: [],
	expenses: [],
	incomes: [],
	recurringExpenses: [],
	recurringIncomes: [],
	piggyBanks: [],
	isAddTransactionDialogOpen: false
});
