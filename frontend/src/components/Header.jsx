import React from 'react'
import { Link, useNavigate, useLocation } from 'react-router-dom'
import { useAuth } from '../contexts/AuthContext'
import { FiHome } from 'react-icons/fi'

export default function Header(){
  const { isAuthenticated, logout } = useAuth()
  const navigate = useNavigate()
  const location = useLocation()

  function handleHomeClick(){
    // when authenticated go to admin selection, otherwise to public home
    if (isAuthenticated) navigate('/admin')
    else navigate('/')
  }

  const isLoginRoute = location.pathname === '/login'

  return (
    <header className="app-header">
      <div className="left">
        {/* FIAP external link (logo/icon) */}
        <a className="top-link" href="https://www.fiap.com.br/" target="_blank" rel="noopener noreferrer" aria-label="Casa">
          <FiHome style={{verticalAlign:'middle', marginRight:8, fontSize:20}} />
        </a>
      </div>

      <div className="right">
        {/* On the login page show a Home link in the header (public index) */}
        {!isAuthenticated && isLoginRoute && (
          <Link to="/" className="fiap-btn" aria-label="Home">Home</Link>
        )}

        {/* When not authenticated show only Login (as requested).
            When authenticated show a Home button (to admin selection) and a Logoff button. */}
        {!isAuthenticated ? (
          <Link to="/login" className="fiap-btn" aria-label="Login">Login</Link>
        ) : (
          <>
            <button className="fiap-btn" onClick={handleHomeClick} aria-label="Home">Home</button>
            <button className="fiap-btn" onClick={() => logout()} aria-label="Logoff">Logoff</button>
          </>
        )}
      </div>
    </header>
  )
}
