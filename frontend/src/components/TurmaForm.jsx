import React, { useEffect, useState } from 'react'
import { useNavigate, useParams } from 'react-router-dom'
import api from '../services/api'

export default function TurmaForm() {
  const { id } = useParams()
  const navigate = useNavigate()
  const [form, setForm] = useState({ nome: '', descricao: '' })
  const [errors, setErrors] = useState({})

  useEffect(() => {
    if (id) load(id)
  }, [id])

  async function load(searchQuery) {
    try {
      const res = await api.get(`/turmas/${searchQuery}`)
      setForm({ nome: res.data?.nome || '', descricao: res.data?.descricao || '' })
    } catch (err) {
      console.error(err)
      alert('Falha ao carregar turma')
    }
  }

  function validate() {
    const e = {}
    if (!form.nome || form.nome.length < 3 || form.nome.length > 100) e.nome = 'Nome deve ter entre 3 e 100 caracteres'
    if (!form.descricao || form.descricao.length < 10 || form.descricao.length > 250) e.descricao = 'Descrição deve ter entre 10 e 250 caracteres'
    setErrors(e)
    return Object.keys(e).length === 0
  }

  async function handleSubmit(e) {
    e.preventDefault()
    if (!validate()) return
    try {
      if (id) {
        await api.put(`/turmas/${id}`, form)
        alert('Turma atualizada')
      } else {
        await api.post('/turmas', form)
        alert('Turma criada')
      }
      navigate('/turmas')
    } catch (err) {
      alert(err?.response?.data?.message || 'Erro ao salvar')
    }
  }

  return (
    <div>
      <h2>{id ? 'Editar Turma' : 'Nova Turma'}</h2>
      <form onSubmit={handleSubmit} className="form">
        <label>Nome</label>
        <input value={form.nome} onChange={(e) => setForm({ ...form, nome: e.target.value })} />
        {errors.nome && <small className="error">{errors.nome}</small>}

        <label>Descrição</label>
        <textarea value={form.descricao} onChange={(e) => setForm({ ...form, descricao: e.target.value })} />
        {errors.descricao && <small className="error">{errors.descricao}</small>}

        <div style={{ marginTop: 12 }}>
          <button type="submit">Salvar</button>
          <button type="button" onClick={() => navigate('/turmas')} style={{ marginLeft: 8 }}>Cancelar</button>
        </div>
      </form>
    </div>
  )
}
