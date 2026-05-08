import { cn } from '@/lib/utils'
import { DialogHTMLAttributes, ReactNode } from 'react'

type Props = {
	children: ReactNode
	isOpen: boolean
	className?: string
} & DialogHTMLAttributes<HTMLDialogElement>

export const Dialog = ({ children, isOpen, className, ...props }: Props) => {
	return (
		<dialog
			open={isOpen}
			className={cn(
				'm-0 max-h-full max-w-full z-50 bg-white mx-auto my-auto w-fit rounded-xl h-fit rounded-t-xl backdrop:',
				className
			)}
			{...props}
		>
			<div className='backdrop:bg-black/70'>{children}</div>
		</dialog>
	)
}
