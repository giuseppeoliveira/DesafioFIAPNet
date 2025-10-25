import React from 'react'
import { Navigate } from 'react-router-dom'
import { useAuth } from '../contexts/AuthContext'

export default function ProtectedRoute({ children }) {
  const { isAuthenticated } = useAuth()
  const token = localStorage.getItem('token')
  if (!isAuthenticated && !token) return <Navigate to="/login" replace />
  return children
}
