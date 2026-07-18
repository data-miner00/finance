export type CurrentPage =
	| 'dashboard'
	| 'account'
	| 'piggyBank'
	| 'income'
	| 'expense'
	| 'settings'
	| 'recurring'
	| 'tax'
	| 'debt';

export type AppState = {
	accounts: Awaited<ReturnType<typeof import('./services').getAccounts>>;
	expenses: Awaited<ReturnType<typeof import('./services').getExpenses>>;
	incomes: Awaited<ReturnType<typeof import('./services').getIncomes>>;
	recurringActions: Awaited<ReturnType<typeof import('./services').getRecurringActions>>;
	piggyBanks: Awaited<ReturnType<typeof import('./services').getPiggyBanks>>;
	taxes: Awaited<ReturnType<typeof import('./services/taxService').getTaxes>>;
	profile: Awaited<ReturnType<typeof import('./services').getProfile>>;
	isAddTransactionDialogOpen: boolean;
	pageTitle: string;
	categories: string[];
	knownLocations: string[];
	knownAgents: string[];
	currentPage: CurrentPage;
	isCommandPaletteOpen: boolean;
	isAddAccountDialogOpen: boolean;
	settings: {
		pieChartDisplayTop: number;
	};
	isAddTaxDialogOpen: boolean;
};

export const appState = $state<AppState>({
	pageTitle: 'Dashboard',
	currentPage: 'dashboard',
	accounts: [],
	expenses: [],
	incomes: [],
	recurringActions: [],
	piggyBanks: [],
	taxes: [],
	profile: undefined,
	isAddTransactionDialogOpen: false,
	categories: [],
	knownLocations: [],
	knownAgents: [],
	isCommandPaletteOpen: false,
	isAddAccountDialogOpen: false,
	isAddTaxDialogOpen: false,
	settings: {
		pieChartDisplayTop: 5
	}
});
