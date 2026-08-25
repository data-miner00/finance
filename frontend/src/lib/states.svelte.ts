import { DEFAULT_SETTINGS, type KnownSettings } from './services/settingsDefaults';
import type { Category, Notification } from './services/types';

export type CurrentPage =
	| 'dashboard'
	| 'account'
	| 'category'
	| 'piggyBank'
	| 'income'
	| 'expense'
	| 'settings'
	| 'recurring'
	| 'tax'
	| 'budget'
	| 'debt'
	| 'notification';

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
	categories: Category[];
	notifications: Notification[];
	unreadNotificationCount: number;
	knownLocations: string[];
	knownAgents: string[];
	currentPage: CurrentPage;
	isCommandPaletteOpen: boolean;
	isAddAccountDialogOpen: boolean;
	isAddCategoryDialogOpen: boolean;
	settings: KnownSettings;
	isAddTaxDialogOpen: boolean;
	isAddPiggyBankDialogOpen: boolean;
	isAddRecurringActionDialogOpen: boolean;
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
	notifications: [],
	unreadNotificationCount: 0,
	knownLocations: [],
	knownAgents: [],
	isCommandPaletteOpen: false,
	isAddAccountDialogOpen: false,
	isAddCategoryDialogOpen: false,
	isAddTaxDialogOpen: false,
	isAddPiggyBankDialogOpen: false,
	isAddRecurringActionDialogOpen: false,
	settings: { ...DEFAULT_SETTINGS }
});
