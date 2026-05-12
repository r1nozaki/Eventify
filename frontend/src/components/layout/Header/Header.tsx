'use client'

import Logo from '@/components/core/Logo'
import { PrimaryButton } from '@/components/core/Button/PrimaryButton'
import { Button } from '@/components/core/Button/Button'
import NavItemList from './components/Navigation/components/NavItemList'
import { SearchInput } from '@/components/core/SearchInput/SearchInput'
import { useAuth } from '@/contexts/authContext'
import { useAuthModal } from '@/contexts/authModalContext'
import { ChevronDown, LogOut, Menu, UserRound, X } from 'lucide-react'
import { usePathname, useRouter } from 'next/navigation'
import { FormEvent, useEffect, useRef, useState } from 'react'

export const Header = () => {
	const router = useRouter()
	const pathname = usePathname()
	const auth = useAuth()
	const { openLogin } = useAuthModal()

	const [mobileOpen, setMobileOpen] = useState(false)
	const [acctOpen, setAcctOpen] = useState(false)
	const [desktopSearch, setDesktopSearch] = useState('')
	const [mobileSearch, setMobileSearch] = useState('')
	const accountRef = useRef<HTMLDivElement>(null)

	const closeMenus = () => {
		setMobileOpen(false)
		setAcctOpen(false)
	}

	useEffect(() => {
		setMobileOpen(false)
		setAcctOpen(false)
	}, [pathname])

	useEffect(() => {
		const onEsc = (e: KeyboardEvent) => {
			if (e.key === 'Escape') closeMenus()
		}

		window.addEventListener('keydown', onEsc)
		return () => window.removeEventListener('keydown', onEsc)
	}, [])

	useEffect(() => {
		if (!acctOpen) return undefined

		const handlePointerDown = (ev: MouseEvent) => {
			if (
				accountRef.current &&
				!accountRef.current.contains(ev.target as Node)
			)
				setAcctOpen(false)
		}

		document.addEventListener('mousedown', handlePointerDown)
		return () => document.removeEventListener('mousedown', handlePointerDown)
	}, [acctOpen])

	const submitDesktopSearch = (event: FormEvent<HTMLFormElement>) => {
		event.preventDefault()
		const trimmed = desktopSearch.trim()
		if (!trimmed) return
		closeMenus()
		setDesktopSearch('')
		router.push(`/events?search=${encodeURIComponent(trimmed)}`)
	}

	const submitMobileSearch = (event: FormEvent<HTMLFormElement>) => {
		event.preventDefault()
		const trimmed = mobileSearch.trim()
		if (!trimmed) return
		setMobileSearch('')
		closeMenus()
		router.push(`/events?search=${encodeURIComponent(trimmed)}`)
	}

	return (
		<header className='sticky top-0 z-40 border-b border-gray-200 bg-white'>
			<div className='relative mx-auto flex h-16 max-w-6xl items-center gap-2 px-3 sm:h-20 sm:gap-3 sm:px-6 lg:gap-8 lg:px-10'>
				<button
					type='button'
					onClick={() => setMobileOpen((open) => !open)}
					className='rounded-lg border border-gray-200 p-2 text-gray-900 lg:hidden'
					aria-expanded={mobileOpen}
					aria-controls='mobile-menu'
				>
					{mobileOpen ? (
						<X className='h-6 w-6' aria-hidden />
					) : (
						<Menu className='h-6 w-6' aria-hidden />
					)}
					<span className='sr-only'>Меню</span>
				</button>

				<div className='flex min-w-0 flex-1 items-center gap-3 lg:flex-none lg:gap-10'>
					<div className='min-w-0 shrink-0'>
						<Logo />
					</div>
					<nav className='hidden flex-1 justify-center lg:flex' aria-label='Головне'>
						<NavItemList />
					</nav>
				</div>

				<form
					onSubmit={submitDesktopSearch}
					className='hidden min-w-[220px] max-w-xs flex-1 lg:block xl:max-w-sm'
				>
					<label htmlFor='header-search' className='sr-only'>
						Глобальний пошук подій
					</label>
					<SearchInput
						id='header-search'
						value={desktopSearch}
						onChange={(e) => setDesktopSearch(e.target.value)}
						placeholder='Пошук подій…'
					/>
				</form>

				<div className='ml-auto flex items-center gap-2'>
					{!auth.isHydrated ? (
						<PulseBar />
					) : auth.isAuthenticated ? (
						<div className='relative' ref={accountRef}>
							<button
								type='button'
								onClick={() => setAcctOpen((v) => !v)}
								aria-expanded={acctOpen}
								className='flex h-11 items-center gap-2 rounded-lg border border-gray-200 px-3 text-gray-900 transition hover:border-purple-200 hover:bg-gray-50 focus-visible:outline-none focus-visible:ring-4 focus-visible:ring-purple-100'
							>
								<UserRound className='h-5 w-5 text-purple-600' aria-hidden />
								<span className='hidden max-w-[8rem] truncate text-sm font-medium sm:inline'>
									{auth.user?.username ?? 'Профіль'}
								</span>
								<ChevronDown className='h-4 w-4 text-gray-500' aria-hidden />
							</button>

							{acctOpen ? (
								<div className='absolute right-0 z-50 mt-3 w-64 rounded-xl border border-gray-200 bg-white shadow-lg'>
									<div className='border-b border-gray-200 px-4 py-3 text-sm'>
										<p className='truncate font-semibold text-gray-900'>
											{auth.user?.username}
										</p>
										<p className='truncate text-xs text-gray-500'>
											{auth.user?.email}
										</p>
										<p className='mt-1 text-xs uppercase text-purple-600'>
											{auth.user?.role}
										</p>
									</div>
									<div className='p-3'>
										<Button
											type='button'
											className='w-full justify-between'
											onClick={() => {
												closeMenus()
												void auth.clearSession()
												router.push('/')
											}}
											variant='secondary'
										>
											Вийти
											<LogOut className='h-4 w-4 text-gray-700' aria-hidden />
										</Button>
									</div>
								</div>
							) : null}
						</div>
					) : (
						<PrimaryButton
							className='flex items-center gap-1 whitespace-nowrap'
							onClick={() => openLogin(pathname || '/')}
							type='button'
						>
							<UserRound size={18} aria-hidden /> Увійти
						</PrimaryButton>
					)}
				</div>
			</div>

			{mobileOpen ? (
				<div
					id='mobile-menu'
					className='fixed inset-0 top-20 z-[60] lg:hidden overscroll-none bg-black/35'
				>
					<div className='h-full bg-white'>
						<div className='max-h-[calc(100vh-5rem)] overflow-y-auto border-t border-gray-200 px-4 py-6'>
							<div className='mb-8'>
								<form onSubmit={submitMobileSearch}>
									<label htmlFor='mobile-menu-search' className='sr-only'>
										Пошук подій
									</label>
									<SearchInput
										id='mobile-menu-search'
										value={mobileSearch}
										onChange={(e) => setMobileSearch(e.target.value)}
										placeholder='Шукати заходи…'
									/>
								</form>
							</div>
							<nav aria-label='Основна навігація'>
								<NavItemList layout='vertical' />
							</nav>
							<div className='mt-10 space-y-3 border-t border-gray-200 pt-8'>
								{auth.isAuthenticated ? (
									<>
										<div className='rounded-xl border border-gray-200 px-4 py-3'>
											<p className='text-sm font-semibold text-gray-900'>
												{auth.user?.username}
											</p>
											<p className='text-xs text-gray-500'>{auth.user?.email}</p>
										</div>
										<Button
											type='button'
											className='w-full'
											variant='secondary'
											onClick={() => {
												closeMenus()
												void auth.clearSession()
												router.push('/')
											}}
										>
											Вийти
										</Button>
									</>
								) : (
									<PrimaryButton
										className='w-full justify-center gap-2'
										type='button'
										onClick={() => {
											closeMenus()
											openLogin(pathname || '/')
										}}
									>
										<UserRound className='h-4 w-4' aria-hidden /> Увійти
									</PrimaryButton>
								)}
							</div>
						</div>
					</div>
				</div>
			) : null}
		</header>
	)
}

const PulseBar = () => (
	<div className='h-11 w-40 animate-pulse rounded-lg bg-gray-200' />
)
