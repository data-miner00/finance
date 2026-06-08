export type AppState = {
	accounts: Awaited<ReturnType<typeof import('./services').getAccounts>>;
	expenses: Awaited<ReturnType<typeof import('./services').getExpenses>>;
	incomes: Awaited<ReturnType<typeof import('./services').getIncomes>>;
	recurringActions: Awaited<ReturnType<typeof import('./services').getRecurringActions>>;
	piggyBanks: Awaited<ReturnType<typeof import('./services').getPiggyBanks>>;
	people: Awaited<ReturnType<typeof import('./services/personService').getPeople>>;
	isAddTransactionDialogOpen: boolean;
	pageTitle: string;
	categories: string[];
};

export const appState = $state<AppState>({
	pageTitle: 'Dashboard',
	accounts: [],
	expenses: [],
	incomes: [],
	recurringActions: [],
	piggyBanks: [],
	people: [],
	isAddTransactionDialogOpen: false,
	categories: []
});
