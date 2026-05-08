import { type ButtonVariant } from '@/const/button'
import { cn } from '@/lib/utils'
import { type ButtonHTMLAttributes, ReactNode } from 'react'

export type Props = {
	children: ReactNode
	variant?: ButtonVariant
	isLoading?: boolean
	loadingText?: string
	className?: string
} & ButtonHTMLAttributes<HTMLButtonElement>

export const Button = ({
	children,
	variant = 'primary',
	isLoading,
	loadingText,
	className,
	disabled,
	...props
}: Props) => {
	return (
		<button
			disabled={disabled || isLoading}
			className={cn(
				'rounded-lg h-10 px-5 hover:cursor-pointer',
				variant === 'primary' &&
					'bg-linear-to-r from-purple-600 to-purple-500 text-white  hover:from-purple-700 hover:to-purple-600 shadow-md transition-colors duration-300',
				variant === 'secondary' &&
					'border border-gray-200 bg-white text-gray-900 hover:bg-gray-50 transition-colors duration-300',
				className
			)}
			{...props}
		>
			{isLoading ? loadingText : children}
		</button>
	)
}
