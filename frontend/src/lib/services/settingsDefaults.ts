import { DEFAULT_CURRENCY } from '$lib/constants/currencies';

import type { Setting } from './types';

export interface KnownSettings {
	pieChartDisplayTop: number;
	enableTax: boolean;
	enablePiggyBank: boolean;
	enableBudget: boolean;
	defaultCurrency: string;
}

export const DEFAULT_SETTINGS: KnownSettings = {
	pieChartDisplayTop: 5,
	enableTax: true,
	enablePiggyBank: true,
	enableBudget: true,
	defaultCurrency: DEFAULT_CURRENCY
};

const KEYS = {
	pieChartDisplayTop: 'pieChartDisplayTop',
	enableTax: 'enableTax',
	enablePiggyBank: 'enablePiggyBank',
	enableBudget: 'enableBudget',
	defaultCurrency: 'defaultCurrency'
} as const;

export function parseSettings(raw: Setting[]): KnownSettings {
	const map = new Map(raw.map((s) => [s.key, s.value]));
	return {
		pieChartDisplayTop: parseIntOrDefault(
			map.get(KEYS.pieChartDisplayTop),
			DEFAULT_SETTINGS.pieChartDisplayTop
		),
		enableTax: parseBoolOrDefault(map.get(KEYS.enableTax), DEFAULT_SETTINGS.enableTax),
		enablePiggyBank: parseBoolOrDefault(
			map.get(KEYS.enablePiggyBank),
			DEFAULT_SETTINGS.enablePiggyBank
		),
		enableBudget: parseBoolOrDefault(map.get(KEYS.enableBudget), DEFAULT_SETTINGS.enableBudget),
		defaultCurrency: map.get(KEYS.defaultCurrency) ?? DEFAULT_SETTINGS.defaultCurrency
	};
}

export function toSettingsValues(partial: Partial<KnownSettings>): Record<string, string> {
	const values: Record<string, string> = {};
	if (partial.pieChartDisplayTop !== undefined) {
		values[KEYS.pieChartDisplayTop] = String(partial.pieChartDisplayTop);
	}
	if (partial.enableTax !== undefined) {
		values[KEYS.enableTax] = String(partial.enableTax);
	}
	if (partial.enablePiggyBank !== undefined) {
		values[KEYS.enablePiggyBank] = String(partial.enablePiggyBank);
	}
	if (partial.enableBudget !== undefined) {
		values[KEYS.enableBudget] = String(partial.enableBudget);
	}
	if (partial.defaultCurrency !== undefined) {
		values[KEYS.defaultCurrency] = partial.defaultCurrency;
	}
	return values;
}

function parseIntOrDefault(raw: string | undefined, fallback: number): number {
	if (raw === undefined) return fallback;
	const parsed = Number.parseInt(raw, 10);
	return Number.isNaN(parsed) ? fallback : parsed;
}

function parseBoolOrDefault(raw: string | undefined, fallback: boolean): boolean {
	if (raw === undefined) return fallback;
	if (raw === 'true') return true;
	if (raw === 'false') return false;
	return fallback;
}
