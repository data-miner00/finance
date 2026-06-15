<script lang="ts">
	import { CircleDollarSignIcon } from '@lucide/svelte';
	import HandCoinsIcon from '@lucide/svelte/icons/hand-coins';
	import Bell from '@tabler/icons-svelte/icons/bell';
	import Braces from '@tabler/icons-svelte/icons/braces';
	import CameraIcon from '@tabler/icons-svelte/icons/camera';
	import DashboardIcon from '@tabler/icons-svelte/icons/dashboard';
	import DatabaseIcon from '@tabler/icons-svelte/icons/database';
	import FileAiIcon from '@tabler/icons-svelte/icons/file-ai';
	import FileDescriptionIcon from '@tabler/icons-svelte/icons/file-description';
	import FileWordIcon from '@tabler/icons-svelte/icons/file-word';
	import InfinityIcon from '@tabler/icons-svelte/icons/infinity';
	import ListDetailsIcon from '@tabler/icons-svelte/icons/list-details';
	import MoneybagMove from '@tabler/icons-svelte/icons/moneybag-move';
	import MoneybagMoveBack from '@tabler/icons-svelte/icons/moneybag-move-back';
	import PigMoney from '@tabler/icons-svelte/icons/pig-money';
	import ReportIcon from '@tabler/icons-svelte/icons/report';
	import SearchIcon from '@tabler/icons-svelte/icons/search';
	import SettingsIcon from '@tabler/icons-svelte/icons/settings';
	import Tax from '@tabler/icons-svelte/icons/tax';
	import UserScan from '@tabler/icons-svelte/icons/user-scan';
	import type { ComponentProps } from 'svelte';

	import { PUBLIC_API_BASE_URL } from '$env/static/public';
	import * as Sidebar from '$lib/components/ui/sidebar/index.js';

	import NavDocuments from './nav-documents.svelte';
	import NavMain from './nav-main.svelte';
	import NavSecondary from './nav-secondary.svelte';
	import NavUser from './nav-user.svelte';

	const data = {
		user: {
			name: 'shadcn',
			email: 'm@example.com',
			avatar: '/avatars/shadcn.jpg'
		},
		navMain: [
			{
				title: 'Dashboard',
				url: '/',
				icon: DashboardIcon
			},
			{
				title: 'Accounts',
				url: '/account',
				icon: ListDetailsIcon
			},
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
				title: 'Recurring',
				url: '/recurring',
				icon: InfinityIcon
			},
			{
				title: 'Piggy Bank',
				url: '/piggy-bank',
				icon: PigMoney
			},
			{
				title: 'Debts',
				url: '/debts',
				icon: HandCoinsIcon
			},
			{
				title: 'Tax',
				url: '/tax',
				icon: Tax
			},
			{
				title: 'People',
				url: '/people',
				icon: UserScan
			},
			{
				title: 'Analytics',
				url: '/analytics',
				icon: ReportIcon
			},
			{
				title: 'Notifications',
				url: '/notifications',
				icon: Bell
			}
		],
		navClouds: [
			{
				title: 'Capture',
				icon: CameraIcon,
				isActive: true,
				url: '#',
				items: [
					{
						title: 'Active Proposals',
						url: '#'
					},
					{
						title: 'Archived',
						url: '#'
					}
				]
			},
			{
				title: 'Proposal',
				icon: FileDescriptionIcon,
				url: '#',
				items: [
					{
						title: 'Active Proposals',
						url: '#'
					},
					{
						title: 'Archived',
						url: '#'
					}
				]
			},
			{
				title: 'Prompts',
				icon: FileAiIcon,
				url: '#',
				items: [
					{
						title: 'Active Proposals',
						url: '#'
					},
					{
						title: 'Archived',
						url: '#'
					}
				]
			}
		],
		navSecondary: [
			{
				title: 'Settings',
				url: '#',
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
				name: 'Data Library',
				url: '#',
				icon: DatabaseIcon
			},
			{
				name: 'Reports',
				url: '#',
				icon: ReportIcon
			},
			{
				name: 'Word Assistant',
				url: '#',
				icon: FileWordIcon
			}
		]
	};

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
		<NavMain items={data.navMain} />
		<NavDocuments items={data.documents} />
		<NavSecondary items={data.navSecondary} class="mt-auto" />
	</Sidebar.Content>
	<Sidebar.Footer>
		<NavUser user={data.user} />
	</Sidebar.Footer>
</Sidebar.Root>
