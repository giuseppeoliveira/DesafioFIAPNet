import React, { useState } from 'react'
import { Link, useNavigate } from 'react-router-dom'
import { useAuth } from '../contexts/AuthContext'
import { FiHome } from 'react-icons/fi'

export default function Header(){
  const { isAuthenticated, logout } = useAuth()
  const [open, setOpen] = useState(false)
  const navigate = useNavigate()

  function handleHomeClick(){
    if (isAuthenticated) navigate('/admin')
    else navigate('/')
  }

  return (
    <header className="app-header">
      <div className="left">
        {/* Casa: always redirect to FIAP homepage with a larger home icon */}
        <a className="top-link" href="https://www.fiap.com.br/" target="_blank" rel="noopener noreferrer" aria-label="Casa">
          <FiHome style={{verticalAlign:'middle', marginRight:8, fontSize:20}} />
        </a>
      </div>

      <div className="right">
        {/* HOME and LOGIN (styled like FIAP buttons, textual with subtle relief) */}
        <button className="fiap-btn" onClick={handleHomeClick} aria-label="Home">Home</button>
        <Link to="/login" className="fiap-btn" aria-label="Login">Login</Link>
      </div>
    </header>
  )
}
