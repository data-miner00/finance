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

export type AddDialogKind =
	| 'transaction'
	| 'account'
	| 'category'
	| 'tax'
	| 'piggyBank'
	| 'recurringAction';

export type AppState = {
	accounts: Awaited<ReturnType<typeof import('./services').getAccounts>>;
	expenses: Awaited<ReturnType<typeof import('./services').getExpenses>>;
	incomes: Awaited<ReturnType<typeof import('./services').getIncomes>>;
	recurringActions: Awaited<ReturnType<typeof import('./services').getRecurringActions>>;
	piggyBanks: Awaited<ReturnType<typeof import('./services').getPiggyBanks>>;
	taxes: Awaited<ReturnType<typeof import('./services/taxService').getTaxes>>;
	profile: Awaited<ReturnType<typeof import('./services').getProfile>>;
	pageTitle: string;
	categories: Category[];
	notifications: Notification[];
	unreadNotificationCount: number;
	knownLocations: string[];
	knownAgents: string[];
	currentPage: CurrentPage;
	isCommandPaletteOpen: boolean;
	openAddDialog: AddDialogKind | null;
	settings: KnownSettings;
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
	categories: [],
	notifications: [],
	unreadNotificationCount: 0,
	knownLocations: [],
	knownAgents: [],
	isCommandPaletteOpen: false,
	openAddDialog: null,
	settings: { ...DEFAULT_SETTINGS }
});
