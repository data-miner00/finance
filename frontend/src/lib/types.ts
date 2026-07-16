export interface MonthlyTotal {
	month: string;
	total: number;
}

export interface DailyTotal {
	day: Date;
	total: number;
}

export type UserProfile = {
	username?: string;
	avatarImage?: string;
	firstName?: string;
	lastName?: string;
	websiteUrl?: string;
	bio?: string;
	email?: string;
	companyName?: string;
	website?: string;
};
