import { type NavItem } from '@/app/types/navigation'
import { cn } from '@/lib/utils'
import Link from 'next/link'

const NavItem = ({ label, path, isActive }: NavItem) => {
	return (
		<Link
			href={path}
			className={cn(
				'text-gray-500 font-medium transition-colors duration-300 hover:text-purple-600',
				isActive && 'text-purple-600 border-b border-purple-600 pb-3'
			)}
		>
			{label}
		</Link>
	)
}

export default NavItem
