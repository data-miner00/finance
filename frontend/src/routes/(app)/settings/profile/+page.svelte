<script lang="ts">
	import InfoIcon from '@lucide/svelte/icons/info';
	import { toast } from 'svelte-sonner';

	import * as Avatar from '$lib/components/ui/avatar/index.js';
	import { Button } from '$lib/components/ui/button';
	import * as InputGroup from '$lib/components/ui/input-group/index.js';
	import * as Label from '$lib/components/ui/label/index.js';
	import * as Tooltip from '$lib/components/ui/tooltip/index.js';
	import { saveProfile as saveProfileRequest } from '$lib/services';
	import type { SaveProfileRequest } from '$lib/services';
	import { appState } from '$lib/states.svelte';

	let profile = $state<SaveProfileRequest>({
		avatarImage: appState.profile?.avatarImage || 'notexist.jpg',
		username: appState.profile?.username || 'User',
		firstName: appState.profile?.firstName || '',
		lastName: appState.profile?.lastName || '',
		email: appState.profile?.email || '',
		bio: appState.profile?.bio || '',
		companyName: appState.profile?.companyName || '',
		websiteUrl: appState.profile?.websiteUrl || ''
	});

	async function saveProfile() {
		try {
			appState.profile = await saveProfileRequest(profile);
			toast.success('Profile updated successfully');
		} catch (error) {
			toast.error('Failed to update profile');
		}
	}
</script>

<h1 class="text-2xl font-bold">Profile Settings</h1>

<p>Manage your profile information and preferences that will be visible to yourself.</p>

<section class="mt-6">
	<div class="grid w-full max-w-sm gap-4">
		<Avatar.Root class="h-16 w-16">
			<Avatar.Image src={profile.avatarImage} alt={`@${profile.username}`} />
			<Avatar.Fallback
				>{profile.firstName?.charAt(0) || 'C'}{profile.lastName?.charAt(0) || 'N'}</Avatar.Fallback
			>
		</Avatar.Root>

		<InputGroup.Root>
			<InputGroup.Input id="username" placeholder="shadcn" bind:value={profile.username} />
			<InputGroup.Addon>
				<Label.Root for="username">@</Label.Root>
			</InputGroup.Addon>
		</InputGroup.Root>
		<InputGroup.Root>
			<InputGroup.Input id="firstName" placeholder="Shaun" bind:value={profile.firstName} />
			<InputGroup.Addon align="block-start">
				<Label.Root for="firstName" class="text-foreground">First Name</Label.Root>
			</InputGroup.Addon>
		</InputGroup.Root>
		<InputGroup.Root>
			<InputGroup.Input id="lastName" placeholder="Smith" bind:value={profile.lastName} />
			<InputGroup.Addon align="block-start">
				<Label.Root for="lastName" class="text-foreground">Last Name</Label.Root>
			</InputGroup.Addon>
		</InputGroup.Root>
		<InputGroup.Root>
			<InputGroup.Input id="email" placeholder="shadcn@vercel.com" bind:value={profile.email} />
			<InputGroup.Addon align="block-start">
				<Label.Root for="email" class="text-foreground">Email</Label.Root>
				<Tooltip.Root>
					<Tooltip.Trigger>
						{#snippet child({ props })}
							<InputGroup.Button
								{...props}
								variant="ghost"
								aria-label="Help"
								class="ms-auto rounded-full"
								size="icon-xs"
							>
								<InfoIcon />
							</InputGroup.Button>
						{/snippet}
					</Tooltip.Trigger>
					<Tooltip.Content>
						<p>We'll use this to send you notifications</p>
					</Tooltip.Content>
				</Tooltip.Root>
			</InputGroup.Addon>
		</InputGroup.Root>
		<InputGroup.Root>
			<InputGroup.Textarea
				id="bio"
				placeholder="Tell us about yourself..."
				bind:value={profile.bio}
			/>
			<InputGroup.Addon align="block-start">
				<Label.Root for="bio" class="text-foreground">Bio</Label.Root>
			</InputGroup.Addon>
		</InputGroup.Root>
		<InputGroup.Root>
			<InputGroup.Input id="website" placeholder="https://" bind:value={profile.websiteUrl} />
			<InputGroup.Addon align="block-start">
				<Label.Root for="website" class="text-foreground">Website</Label.Root>
			</InputGroup.Addon>
		</InputGroup.Root>

		<InputGroup.Root>
			<InputGroup.Input id="avatar" placeholder="https://" bind:value={profile.avatarImage} />
			<InputGroup.Addon align="block-start">
				<Label.Root for="avatar" class="text-foreground">Avatar</Label.Root>
			</InputGroup.Addon>
		</InputGroup.Root>

		<InputGroup.Root>
			<InputGroup.Input
				id="companyName"
				placeholder="Company Name"
				bind:value={profile.companyName}
			/>
			<InputGroup.Addon align="block-start">
				<Label.Root for="companyName" class="text-foreground">Company Name</Label.Root>
			</InputGroup.Addon>
		</InputGroup.Root>
		<Button size="sm" variant="outline" onclick={saveProfile}>Save</Button>
	</div>
</section>
