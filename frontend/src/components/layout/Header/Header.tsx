'use client'

import Logo from '@/components/core/Logo'
import NavItemList from './components/Navigation/components/NavItemList'
import { PrimaryButton } from '@/components/core/Button/PrimaryButton'
import { User } from 'lucide-react'
import { SearchInput } from './components/Navigation/components/SearchInput'
import { useState } from 'react'
import { Dialog } from '@/components/core/Dialog/Dialog'

export const Header = () => {
	const [isOpenAuthModal, setIsOpenAuthModal] = useState(false)
	const [isOpenBurger, setIsOpenBurger] = useState(false)

	const handleAuthModal = () => {
		setIsOpenAuthModal(!isOpenAuthModal)
	}
	return (
		<header className='relative flex items-center justify-between h-20 px-10 bg-white border-b border-gray-200'>
			<div className='flex items-center w-full'>
				<Logo />

				<nav className='md:px-6 lg:px-10'>
					<NavItemList />
				</nav>
			</div>

			<div className='flex items-center gap-3'>
				<SearchInput className='w-64' />
				<PrimaryButton
					className='flex items-center gap-1'
					onClick={handleAuthModal}
				>
					<User size={18} />
					<span>Увійти</span>
				</PrimaryButton>
			</div>
			<Dialog isOpen={isOpenAuthModal}>
				<p>Hello</p>
				{/* modal */}
			</Dialog>
		</header>
	)
}
