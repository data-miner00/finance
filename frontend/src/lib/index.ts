// place files you want to import through the `$lib` alias in this folder.

export function formatCurrency(amount: number): string {
	return new Intl.NumberFormat('en-MY', {
		style: 'currency',
		currency: 'MYR'
	}).format(amount);
}

export const getDaysInMonth = (year: number, month: number) => new Date(year, month, 0).getDate();
