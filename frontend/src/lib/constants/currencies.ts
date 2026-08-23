export const CURRENCIES = [
	'MYR',
	'USD',
	'EUR',
	'GBP',
	'JPY',
	'SGD',
	'AUD',
	'CAD',
	'CNY',
	'INR',
	'IDR',
	'THB',
	'HKD',
	'NZD',
	'KRW',
	'CHF',
	'VND',
	'PHP'
] as const;

export type CurrencyCode = (typeof CURRENCIES)[number];

export const CURRENCY_LABELS: Record<CurrencyCode, string> = {
	MYR: 'MYR — Malaysian Ringgit',
	USD: 'USD — US Dollar',
	EUR: 'EUR — Euro',
	GBP: 'GBP — British Pound',
	JPY: 'JPY — Japanese Yen',
	SGD: 'SGD — Singapore Dollar',
	AUD: 'AUD — Australian Dollar',
	CAD: 'CAD — Canadian Dollar',
	CNY: 'CNY — Chinese Yuan',
	INR: 'INR — Indian Rupee',
	IDR: 'IDR — Indonesian Rupiah',
	THB: 'THB — Thai Baht',
	HKD: 'HKD — Hong Kong Dollar',
	NZD: 'NZD — New Zealand Dollar',
	KRW: 'KRW — South Korean Won',
	CHF: 'CHF — Swiss Franc',
	VND: 'VND — Vietnamese Dong',
	PHP: 'PHP — Philippine Peso'
};

export const DEFAULT_CURRENCY: CurrencyCode = 'MYR';
