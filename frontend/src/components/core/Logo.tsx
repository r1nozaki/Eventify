import { CalendarHeart } from 'lucide-react'
import Link from 'next/link'

type Props = {
	iconSize?: number
}

const Logo = ({ iconSize = 30 }: Props) => {
	return (
		<Link
			href='/'
			className='flex items-center gap-2'
		>
			<CalendarHeart
				size={iconSize}
				color='#7C3AED'
			/>
			<span className='text-2xl font-bold text-gray-900'>Eventify</span>
		</Link>
	)
}

export default Logo
