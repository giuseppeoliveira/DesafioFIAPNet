import React from 'react'
import { Link } from 'react-router-dom'
import { FiUsers, FiLayers, FiClipboard } from 'react-icons/fi'

export default function AdminDashboard(){

  return (
    <div>
      <div className="admin-hero">
        <h2>Bem-vindo ao Sistema de Gestão</h2>
        <p className="muted">Selecione uma opção para começar a gerenciar</p>
      </div>

  <div className="grid" style={{gridTemplateColumns:'repeat(2,minmax(280px,360px))',gap:20,marginTop:18,justifyContent:'center',marginLeft:'auto',marginRight:'auto'}}>
        <div className="card feature-card">
          <div className="icon-box" style={{background:'rgba(255,85,170,0.12)'}}>
            <FiUsers style={{color:'#ff1e78',fontSize:20}} />
          </div>
          <h3>Alunos</h3>
          <p className="muted">Visualize, filtre e gerencie todos os alunos cadastrados no sistema</p>
          <Link to="/alunos" className="btn pink" style={{marginTop:12}}>Acessar Alunos</Link>
        </div>

        <div className="card feature-card">
          <div className="icon-box" style={{background:'rgba(123,82,255,0.12)'}}>
            <FiLayers style={{color:'#6f2cff',fontSize:20}} />
          </div>
          <h3>Turmas</h3>
          <p className="muted">Gerencie turmas, disciplinas, horários e organize os cursos</p>
          <Link to="/turmas" className="btn purple" style={{marginTop:12}}>Acessar Turmas</Link>
        </div>
      </div>
    </div>
  )
}
