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
				'm-0 z-50 mx-auto my-auto h-fit w-fit max-h-full max-w-full rounded-xl bg-white backdrop:bg-black/70',
				className
			)}
			{...props}
		>
			{children}
		</dialog>
	)
}
