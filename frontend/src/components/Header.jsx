import React from 'react'
import { Link, useNavigate, useLocation } from 'react-router-dom'
import { useAuth } from '../contexts/AuthContext'
import { FiHome } from 'react-icons/fi'

export default function Header(){
  const { isAuthenticated, logout } = useAuth()
  const navigate = useNavigate()
  const location = useLocation()

  function handleHomeClick(){
    // quando autenticado, ir para a tela de seleção/admin; caso contrário, ir para a home pública
    if (isAuthenticated) navigate('/admin')
    else navigate('/')
  }

  const isLoginRoute = location.pathname === '/login'

  return (
    <header className="app-header">
      <div className="left">
  {/* Link externo FIAP (logo/ícone) */}
        <a className="top-link" href="https://www.fiap.com.br/" target="_blank" rel="noopener noreferrer" aria-label="Casa">
          <FiHome style={{verticalAlign:'middle', marginRight:8, fontSize:20}} />
        </a>
      </div>

      <div className="right">
  {/* Na página de login, mostrar um link Home no cabeçalho (índice público) */}
        {!isAuthenticated && isLoginRoute && (
          <Link to="/" className="fiap-btn" aria-label="Home">Home</Link>
        )}

    {/* Quando não autenticado, mostrar apenas Login.
      Quando autenticado, mostrar Home (tela de seleção/admin) e botão de Logoff. */}
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
