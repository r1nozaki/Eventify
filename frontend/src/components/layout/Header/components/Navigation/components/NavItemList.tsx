'use client'

import { usePathname } from 'next/navigation'
import { NAV_LINK } from '@/const/navigation'
import NavItem from './NavItem'

const NavItemList = () => {
	const pathname = usePathname()

	return (
		<div className='w-full'>
			<ul className='flex items-center gap-4'>
				{NAV_LINK.map(({ label, path }) => (
					<li key={path}>
						<NavItem
							label={label}
							path={path}
							isActive={pathname === path}
						/>
					</li>
				))}
			</ul>
		</div>
	)
}

export default NavItemList
