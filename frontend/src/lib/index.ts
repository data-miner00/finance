// place files you want to import through the `$lib` alias in this folder.
import { toast } from 'svelte-sonner';

export function formatCurrency(amount: number): string {
	return new Intl.NumberFormat('en-MY', {
		style: 'currency',
		currency: 'MYR'
	}).format(amount);
}

export const getDaysInMonth = (year: number, month: number) => new Date(year, month, 0).getDate();

export async function copyText(text: string, message = 'Copied to clipboard') {
	const date = new Date();

	try {
		await navigator.clipboard.writeText(text);

		toast.success(message, {
			description: date.toUTCString(),
			action: {
				label: 'Ok',
				onClick: () => console.info('Ok')
			}
		});
	} catch (error) {
		toast.error('Failed to copy to clipboard', {
			description: date.toUTCString(),
			action: {
				label: 'Ok',
				onClick: () => console.info('Ok')
			}
		});
	}
}

export function isCurrentMonth(dateString: string): boolean {
	const date = new Date(dateString);
	const now = new Date();
	return date.getMonth() === now.getMonth() && date.getFullYear() === now.getFullYear();
}

export function isLastMonth(dateString: string): boolean {
	const date = new Date(dateString);
	const now = new Date();
	const lastMonth = new Date(now.getFullYear(), now.getMonth() - 1);
	return date.getMonth() === lastMonth.getMonth() && date.getFullYear() === lastMonth.getFullYear();
}

export function calculatePercentageChange(current: number, previous: number): number {
	if (previous === 0) return 0;
	// Divide by |previous| so the sign reflects improvement/decline even when
	// previous was negative (e.g. an overbudget month) — dividing by a negative
	// previous flips the sign and reports the direction backwards.
	return ((current - previous) / Math.abs(previous)) * 100;
}
