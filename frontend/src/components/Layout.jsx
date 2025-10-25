import React from 'react'
import Header from './Header'
import { useLocation, useNavigate } from 'react-router-dom'
import { FiArrowLeft } from 'react-icons/fi'

export function AppLayout({ children }){
  const location = useLocation()
  const navigate = useNavigate()
  const pathname = location.pathname || '/'
  // hide back button on root, index variations, login and admin selection screen
  const hideBackOn = ['/', '/index.html', '/login', '/admin']
  const shouldHideBack = hideBackOn.some(p => pathname === p || pathname.startsWith(p) || pathname.endsWith(p))

  function handleBack(){
    // when in alunos/turmas return to admin selection (logged-in home)
    if(pathname.startsWith('/alunos') || pathname.startsWith('/turmas')){
      navigate('/admin')
      return
    }
    navigate(-1)
  }

  return (
    <div className="app">
      <Header />
      <main className="app-main">
        <div style={{marginBottom:12}}>
          {!shouldHideBack && (
            <button className="home-btn" onClick={handleBack} aria-label="Voltar">
              <FiArrowLeft style={{marginRight:8}} /> Voltar
            </button>
          )}
        </div>
        <div className="container">{children}</div>
      </main>
      <footer className="footer">© FIAP - Secretaria • Projeto de exemplo</footer>
    </div>
  )
}
