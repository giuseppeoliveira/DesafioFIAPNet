import React, { createContext, useContext, useState, useEffect } from 'react'
import api from '../services/api'
import { useNavigate } from 'react-router-dom'

const AuthContext = createContext()

export const AuthProvider = ({ children }) => {
  const [token, setToken] = useState(() => localStorage.getItem('token'))
  const navigate = useNavigate()

  useEffect(() => {
    if (token) localStorage.setItem('token', token)
    else localStorage.removeItem('token')
  }, [token])

  const login = async (email, password) => {
    const res = await api.post('/sessao', { email, senha: password })
    const t = res.data?.token
    console.log('token:', t)
    if (t) {
      setToken(t)
      return true
    }
    return false
  }

  const logout = () => {
    setToken(null)
    navigate('/login')
  }

  const value = {
    token,
    isAuthenticated: !!token,
    login,
    logout,
    setToken
  }

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>
}

export const useAuth = () => useContext(AuthContext)
