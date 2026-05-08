import { Search } from 'lucide-react'
import { ComponentProps } from 'react'
import { Input } from '@/components/core/Input/Input'
import { cn } from '@/lib/utils'

type Props = ComponentProps<typeof Input>

export const SearchInput = ({ className, ...props }: Props) => {
	return (
		<div className='relative w-full'>
			<Search
				size={18}
				className='absolute z-10 text-gray-400 -translate-y-1/2 left-4 top-1/2'
			/>
			<Input
				className={cn('pl-11', className)}
				{...props}
				placeholder='Пошук...'
			/>
		</div>
	)
}
