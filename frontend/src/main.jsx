import React from 'react'
import { createRoot } from 'react-dom/client'
import { BrowserRouter } from 'react-router-dom'
import App from './App'
import { AuthProvider } from './contexts/AuthContext'
import { AlertProvider } from './contexts/AlertContext'
import './style.css'

const container = document.getElementById('root')
const root = createRoot(container)
root.render(
	<React.StrictMode>
			<BrowserRouter>
				<AuthProvider>
					<AlertProvider>
						<App />
					</AlertProvider>
				</AuthProvider>
			</BrowserRouter>
	</React.StrictMode>
)
