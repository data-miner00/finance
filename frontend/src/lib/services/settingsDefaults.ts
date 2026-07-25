import type { Setting } from './types';

export interface KnownSettings {
	pieChartDisplayTop: number;
	enableTax: boolean;
}

export const DEFAULT_SETTINGS: KnownSettings = {
	pieChartDisplayTop: 5,
	enableTax: true
};

const KEYS = {
	pieChartDisplayTop: 'pieChartDisplayTop',
	enableTax: 'enableTax'
} as const;

export function parseSettings(raw: Setting[]): KnownSettings {
	const map = new Map(raw.map((s) => [s.key, s.value]));
	return {
		pieChartDisplayTop: parseIntOrDefault(
			map.get(KEYS.pieChartDisplayTop),
			DEFAULT_SETTINGS.pieChartDisplayTop
		),
		enableTax: parseBoolOrDefault(map.get(KEYS.enableTax), DEFAULT_SETTINGS.enableTax)
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
