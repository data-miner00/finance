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
