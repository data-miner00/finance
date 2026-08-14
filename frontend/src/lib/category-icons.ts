import Baby from '@lucide/svelte/icons/baby';
import Briefcase from '@lucide/svelte/icons/briefcase';
import Bus from '@lucide/svelte/icons/bus';
import Car from '@lucide/svelte/icons/car';
import Coffee from '@lucide/svelte/icons/coffee';
import Dumbbell from '@lucide/svelte/icons/dumbbell';
import Film from '@lucide/svelte/icons/film';
import Fuel from '@lucide/svelte/icons/fuel';
import Gift from '@lucide/svelte/icons/gift';
import GraduationCap from '@lucide/svelte/icons/graduation-cap';
import HeartPulse from '@lucide/svelte/icons/heart-pulse';
import House from '@lucide/svelte/icons/house';
import Landmark from '@lucide/svelte/icons/landmark';
import Music from '@lucide/svelte/icons/music';
import PawPrint from '@lucide/svelte/icons/paw-print';
import Phone from '@lucide/svelte/icons/phone';
import Plane from '@lucide/svelte/icons/plane';
import Shirt from '@lucide/svelte/icons/shirt';
import ShoppingCart from '@lucide/svelte/icons/shopping-cart';
import Tag from '@lucide/svelte/icons/tag';
import Utensils from '@lucide/svelte/icons/utensils';
import Wifi from '@lucide/svelte/icons/wifi';
import Wrench from '@lucide/svelte/icons/wrench';
import Zap from '@lucide/svelte/icons/zap';

export const categoryIcons = {
	utensils: Utensils,
	car: Car,
	house: House,
	zap: Zap,
	film: Film,
	'shopping-cart': ShoppingCart,
	'heart-pulse': HeartPulse,
	'graduation-cap': GraduationCap,
	plane: Plane,
	gift: Gift,
	dumbbell: Dumbbell,
	wifi: Wifi,
	phone: Phone,
	fuel: Fuel,
	coffee: Coffee,
	shirt: Shirt,
	baby: Baby,
	'paw-print': PawPrint,
	wrench: Wrench,
	landmark: Landmark,
	briefcase: Briefcase,
	music: Music,
	bus: Bus,
	tag: Tag
};

export type CategoryIconName = keyof typeof categoryIcons;

export const categoryIconNames = Object.keys(categoryIcons) as CategoryIconName[];

export function getCategoryIcon(name?: string | null) {
	if (!name || !(name in categoryIcons)) return Tag;
	return categoryIcons[name as CategoryIconName];
}
