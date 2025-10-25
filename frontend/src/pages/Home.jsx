import React from 'react'
import { useNavigate } from 'react-router-dom'
import { FiLogIn, FiUsers, FiLayers } from 'react-icons/fi'
import bgIndex from '../assets/bg-index.png'

export default function Home(){
  const navigate = useNavigate()
  return (
    <div>
      <div className="hero hero--center" style={{backgroundImage:`linear-gradient(90deg, rgba(91,43,138,0.65), rgba(87,36,145,0.55)), url(${bgIndex})`, backgroundSize:'cover', backgroundPosition:'center'}}>
        <div className="container">
          <div style={{maxWidth:900}}>
            <h2>Secretaria FIAP — Sistema de Gestão</h2>
            <p style={{fontSize:16}}>Painel administrativo para gerenciar alunos, turmas e matrículas. Use este sistema para cadastrar alunos, criar turmas, e realizar matrículas de forma simples, segura e eficiente.</p>
            {/* As requested: no images and no buttons inside the purple rectangle */}
          </div>
        </div>
      </div>

      <div className="grid" style={{gridTemplateColumns:'repeat(auto-fit,minmax(240px,1fr))',gap:16}}>
        <div className="card">
          <h3><FiUsers style={{marginRight:8}}/> Controle de Alunos</h3>
          <p className="muted">Cadastre, pesquise e gerencie o histórico dos alunos.</p>
        </div>
        <div className="card">
          <h3><FiLayers style={{marginRight:8}}/> Controle de Turmas</h3>
          <p className="muted">Crie turmas, visualize a quantidade de alunos e gerencie vagas.</p>
        </div>
        <div className="card">
          <h3><FiLayers style={{marginRight:8}}/> Matrículas</h3>
          <p className="muted">Realize matrículas e consulte alunos por turma.</p>
        </div>
      </div>
    </div>
  )
}
