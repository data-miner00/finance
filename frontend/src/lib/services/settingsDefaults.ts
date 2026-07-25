import type { Setting } from './types';

export interface KnownSettings {
	pieChartDisplayTop: number;
}

export const DEFAULT_SETTINGS: KnownSettings = {
	pieChartDisplayTop: 5
};

const KEYS = {
	pieChartDisplayTop: 'pieChartDisplayTop'
} as const;

export function parseSettings(raw: Setting[]): KnownSettings {
	const map = new Map(raw.map((s) => [s.key, s.value]));
	return {
		pieChartDisplayTop: parseIntOrDefault(
			map.get(KEYS.pieChartDisplayTop),
			DEFAULT_SETTINGS.pieChartDisplayTop
		)
	};
}

export function toSettingsValues(partial: Partial<KnownSettings>): Record<string, string> {
	const values: Record<string, string> = {};
	if (partial.pieChartDisplayTop !== undefined) {
		values[KEYS.pieChartDisplayTop] = String(partial.pieChartDisplayTop);
	}
	return values;
}

function parseIntOrDefault(raw: string | undefined, fallback: number): number {
	if (raw === undefined) return fallback;
	const parsed = Number.parseInt(raw, 10);
	return Number.isNaN(parsed) ? fallback : parsed;
}
