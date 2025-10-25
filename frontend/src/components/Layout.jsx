import React from 'react'
import Header from './Header'

export function AppLayout({ children }){
  return (
    <div className="app">
      <Header />
      <main className="app-main">
        <div className="container">{children}</div>
      </main>
      <footer className="footer">© FIAP - Secretaria • Projeto de exemplo</footer>
    </div>
  )
}
