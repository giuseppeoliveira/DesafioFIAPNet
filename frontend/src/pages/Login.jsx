import React, { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { useAuth } from '../contexts/AuthContext'
import { FiLock, FiMail } from 'react-icons/fi'

export default function Login() {
  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')
  const [error, setError] = useState(null)
  const { login, setToken } = useAuth()
  const navigate = useNavigate()

  const handleSubmit = async (e) => {
    e.preventDefault()
    setError(null)
    try {
      const ok = await login(email, password)
      if (ok) {
        navigate('/admin')
      } else {
        setError('Credenciais inválidas')
      }
    } catch (err) {
        setError(err?.response?.data?.message || 'Erro ao autenticar')
    }
  }

  return (
    <div style={{display:'flex',alignItems:'center',justifyContent:'center',minHeight:'70vh'}}>
      <div className="card" style={{maxWidth:420,width:'100%',padding:28}}>
        <div style={{textAlign:'center',marginBottom:8}}>
          <h2 style={{margin:0,color:'var(--color-primary)'}}>Admin Login</h2>
          <p className="muted">Acesse o painel administrativo</p>
        </div>
        <form onSubmit={handleSubmit} className="form">
          <label>Email</label>
          <div style={{display:'flex',alignItems:'center',gap:8}}>
            <FiMail />
            <input value={email} onChange={(e) => setEmail(e.target.value)} type="email" required style={{flex:1}} />
          </div>

          <label>Senha</label>
          <div style={{display:'flex',alignItems:'center',gap:8}}>
            <FiLock />
            <input value={password} onChange={(e) => setPassword(e.target.value)} type="password" required style={{flex:1}} />
          </div>

          <div style={{display:'flex',justifyContent:'space-between',alignItems:'center',marginTop:12}}>
            <button className="btn" type="submit">Entrar</button>
            <button type="button" className="btn secondary" onClick={() => { setEmail('admin@gmail.com'); setPassword('Fiap@2025') }}>Preencher</button>
          </div>
          {error && <p className="error">{error}</p>}
        </form>
      </div>
    </div>
  )
}
