import React, { useEffect, useState } from 'react'
import api from '../services/api'
import { useAlert } from '../contexts/AlertContext'

function validateCPF(cpf) {
  if (!cpf) return false
  const digits = cpf.replace(/\D/g, '')
  return digits.length === 11
}

function validateEmail(email) {
  return /\S+@\S+\.\S+/.test(email)
}

function validatePassword(pwd) {
  if (!pwd) return false
  return pwd.length >= 8 && /[A-Z]/.test(pwd) && /[a-z]/.test(pwd) && /\d/.test(pwd) && /[^A-Za-z0-9]/.test(pwd)
}

export default function AlunoFormInline({ cpf: initialCpf = null, onSaved = () => {}, onCancel = () => {} }) {
  const [form, setForm] = useState({ nome: '', dataNascimento: '', cpf: '', email: '', senha: '' })
  const [errors, setErrors] = useState({})
  const [id, setId] = useState(0)
  const [loading, setLoading] = useState(false)
  const { showAlert } = useAlert()

  useEffect(() => {
    if (initialCpf) load(initialCpf)
  }, [initialCpf])

  async function load(searchQuery) {
    try {
      setLoading(true)
      const res = await api.get(`/alunos?cpfQuery=${encodeURIComponent(searchQuery)}`)
      const aluno = res.data.items && res.data.items.length > 0 ? res.data.items[0] : null
      if (aluno) {
        setForm({ nome: aluno.nome || '', dataNascimento: aluno.dataNascimento?.substring(0,10) || '', cpf: aluno.cpf || '', email: aluno.email || '', senha: '' })
        setId(aluno.id || 0)
      }
    } catch (err) {
      console.error(err)
      showAlert('Falha ao carregar aluno')
    } finally {
      setLoading(false)
    }
  }

  function validate() {
    const e = {}
    if (!form.nome || form.nome.length < 3 || form.nome.length > 100) e.nome = 'Nome deve ter entre 3 e 100 caracteres'
    if (!form.dataNascimento) e.dataNascimento = 'Data de nascimento é obrigatória'
    if (!validateCPF(form.cpf)) e.cpf = 'CPF inválido (11 dígitos)'
    if (!validateEmail(form.email)) e.email = 'Email inválido'
    if (!initialCpf) {
      // creating: password required
      if (!validatePassword(form.senha)) e.senha = 'Senha fraca (min 8 chars, maiúscula, minúscula, número e símbolo)'
    } else {
      // editing: optional password, but validate if provided
      if (form.senha && !validatePassword(form.senha)) e.senha = 'Senha fraca (se fornecida: min 8 chars, maiúscula, minúscula, número e símbolo)'
    }
    setErrors(e)
    return Object.keys(e).length === 0
  }

  async function handleSubmit(e) {
    e.preventDefault()
    if (!validate()) return
    try {
      if (initialCpf) {
        await api.put(`/alunos/${id}`, form)
        // inform parent
        showAlert('Aluno atualizado').then(() => onSaved())
      } else {
        await api.post('/alunos', form)
        showAlert('Aluno criado').then(() => onSaved())
      }
    } catch (err) {
      showAlert(err?.response?.data?.message || 'Erro ao salvar')
    }
  }


  return (
    <div>
      <div style={{display:'flex',justifyContent:'space-between',alignItems:'center'}}>
        <h3 style={{margin:0}}>{initialCpf ? 'Editar Aluno' : 'Cadastrar Aluno'}</h3>
        <button className="top-btn" onClick={onCancel}>X</button>
      </div>
      <form onSubmit={handleSubmit} className="form" style={{marginTop:12}}>
        <label>Nome</label>
        <input value={form.nome} onChange={(e) => setForm({ ...form, nome: e.target.value })} />
        {errors.nome && <small className="error">{errors.nome}</small>}

        <label>Data de Nascimento</label>
        <input type="date" value={form.dataNascimento} onChange={(e) => setForm({ ...form, dataNascimento: e.target.value })} />
        {errors.dataNascimento && <small className="error">{errors.dataNascimento}</small>}

        <label>CPF</label>
        <input
          value={form.cpf}
          onChange={(e) => {
            const digits = e.target.value.replace(/\D/g, '').slice(0, 11)
            setForm({ ...form, cpf: digits })
          }}
          inputMode="numeric"
          pattern="\d*"
          maxLength={11}
        />
        {errors.cpf && <small className="error">{errors.cpf}</small>}

        <label>Email</label>
        <input type="email" value={form.email} onChange={(e) => setForm({ ...form, email: e.target.value })} />
        {errors.email && <small className="error">{errors.email}</small>}

        <>
          <label>Senha{initialCpf ? ' (preencha para alterar)' : ''}</label>
          <input type="password" value={form.senha} onChange={(e) => setForm({ ...form, senha: e.target.value })} />
          {errors.senha && <small className="error">{errors.senha}</small>}
        </>

        <div style={{ marginTop: 12, display:'flex', gap:8, justifyContent:'flex-end' }}>
          <button type="button" className="btn secondary" onClick={onCancel}>Cancelar</button>
          <button type="submit" className="btn">Salvar</button>
        </div>
      </form>
    </div>
  )
}
