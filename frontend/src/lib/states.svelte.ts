export type CurrentPage =
	| 'dashboard'
	| 'account'
	| 'piggyBank'
	| 'income'
	| 'expense'
	| 'settings'
	| 'recurring'
	| 'tax'
	| 'people'
	| 'debt';

export type AppState = {
	accounts: Awaited<ReturnType<typeof import('./services').getAccounts>>;
	expenses: Awaited<ReturnType<typeof import('./services').getExpenses>>;
	incomes: Awaited<ReturnType<typeof import('./services').getIncomes>>;
	recurringActions: Awaited<ReturnType<typeof import('./services').getRecurringActions>>;
	piggyBanks: Awaited<ReturnType<typeof import('./services').getPiggyBanks>>;
	people: Awaited<ReturnType<typeof import('./services/personService').getPeople>>;
	taxes: Awaited<ReturnType<typeof import('./services/taxService').getTaxes>>;
	isAddTransactionDialogOpen: boolean;
	pageTitle: string;
	categories: string[];
	knownLocations: string[];
	currentPage: CurrentPage;
	isCommandPaletteOpen: boolean;
};

export const appState = $state<AppState>({
	pageTitle: 'Dashboard',
	currentPage: 'dashboard',
	accounts: [],
	expenses: [],
	incomes: [],
	recurringActions: [],
	piggyBanks: [],
	people: [],
	taxes: [],
	isAddTransactionDialogOpen: false,
	categories: [],
	knownLocations: [],
	isCommandPaletteOpen: false
});
