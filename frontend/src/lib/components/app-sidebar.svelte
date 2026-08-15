<script lang="ts">
	import { CircleDollarSignIcon } from '@lucide/svelte';
	import Bell from '@tabler/icons-svelte/icons/bell';
	import Braces from '@tabler/icons-svelte/icons/braces';
	import InfinityIcon from '@tabler/icons-svelte/icons/infinity';
	import ListDetailsIcon from '@tabler/icons-svelte/icons/list-details';
	import MoneybagMove from '@tabler/icons-svelte/icons/moneybag-move';
	import MoneybagMoveBack from '@tabler/icons-svelte/icons/moneybag-move-back';
	import PigMoney from '@tabler/icons-svelte/icons/pig-money';
	import ReportMoney from '@tabler/icons-svelte/icons/report-money';
	import SearchIcon from '@tabler/icons-svelte/icons/search';
	import SettingsIcon from '@tabler/icons-svelte/icons/settings';
	import TagIcon from '@tabler/icons-svelte/icons/tag';
	import Tax from '@tabler/icons-svelte/icons/tax';
	import type { ComponentProps } from 'svelte';

	import { PUBLIC_API_BASE_URL } from '$env/static/public';
	import * as Sidebar from '$lib/components/ui/sidebar/index.js';
	import { appState } from '$lib/states.svelte';

	import NavDocuments from './nav-documents.svelte';
	import NavMain from './nav-main.svelte';
	import NavSecondary from './nav-secondary.svelte';
	import NavUser from './nav-user.svelte';

	let navUser = $derived({
		name: appState.profile?.username || 'Guest',
		email: appState.profile?.email || 'm@example.com',
		avatar: appState.profile?.avatarImage || '/avatars/shadcn.jpg'
	});

	const data = $derived({
		navMain: [],
		navSecondary: [
			{
				title: 'Settings',
				url: '/settings',
				icon: SettingsIcon
			},
			{
				title: 'Swagger',
				url: `${PUBLIC_API_BASE_URL}/swagger/index.html`,
				icon: Braces,
				external: true
			},
			{
				title: 'Search',
				url: '#',
				icon: SearchIcon
			}
		],
		documents: [
			{
				title: 'Expenses',
				url: '/expense',
				icon: MoneybagMove
			},
			{
				title: 'Income',
				url: '/income',
				icon: MoneybagMoveBack
			},
			{
				title: 'Accounts',
				url: '/account',
				icon: ListDetailsIcon
			},
			{
				title: 'Categories',
				url: '/category',
				icon: TagIcon
			},
			{
				title: 'Recurring',
				url: '/recurring',
				icon: InfinityIcon
			},
			...(appState.settings.enablePiggyBank
				? [
						{
							title: 'Piggy Bank',
							url: '/piggy-bank',
							icon: PigMoney
						}
					]
				: []),
			...(appState.settings.enableTax
				? [
						{
							title: 'Tax',
							url: '/tax',
							icon: Tax
						}
					]
				: []),
			...(appState.settings.enableBudget
				? [
						{
							title: 'Budget',
							url: '/budget',
							icon: ReportMoney
						}
					]
				: []),
			{
				title: 'Notifications',
				url: '/notifications',
				icon: Bell
			}
		]
	});

	let { ...restProps }: ComponentProps<typeof Sidebar.Root> = $props();
</script>

<Sidebar.Root collapsible="offcanvas" {...restProps}>
	<Sidebar.Header>
		<Sidebar.Menu>
			<Sidebar.MenuItem>
				<Sidebar.MenuButton class="data-[slot=sidebar-menu-button]:p-1.5!">
					{#snippet child({ props })}
						<a href="/" {...props}>
							<CircleDollarSignIcon class="size-5!" />
							<span class="text-base font-semibold">Finance</span>
						</a>
					{/snippet}
				</Sidebar.MenuButton>
			</Sidebar.MenuItem>
		</Sidebar.Menu>
	</Sidebar.Header>
	<Sidebar.Content>
		<NavMain />
		<NavDocuments items={data.documents} />
		<NavSecondary items={data.navSecondary} class="mt-auto" />
	</Sidebar.Content>
	<Sidebar.Footer>
		<NavUser user={navUser} />
	</Sidebar.Footer>
</Sidebar.Root>
