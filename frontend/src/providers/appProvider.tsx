'use client'

import { AuthProvider } from '@/contexts/authContext'
import { AuthModalProvider } from '@/contexts/authModalContext'
import { LenisProvider } from '@/contexts/lenisContext'
import { QueryProvider } from '@/providers/queryProvider'
import { type ReactNode } from 'react'

export const AppProvider = ({ children }: { children: ReactNode }) => (
	<QueryProvider>
		<AuthProvider>
			<LenisProvider>
				<AuthModalProvider>{children}</AuthModalProvider>
			</LenisProvider>
		</AuthProvider>
	</QueryProvider>
)
