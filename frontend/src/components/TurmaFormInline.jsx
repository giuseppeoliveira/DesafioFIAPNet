import React, { useEffect, useState } from 'react'
import api from '../services/api'
import { useAlert } from '../contexts/AlertContext'

export default function TurmaFormInline({ id: initialId = null, onSaved = () => {}, onCancel = () => {} }){
  const [form, setForm] = useState({ nome:'', descricao:'' })
  const [errors, setErrors] = useState({})
  const [id, setId] = useState(0)
  const { showAlert } = useAlert()
  const [loading, setLoading] = useState(false)

  useEffect(() => {
    if(initialId) load(initialId)
  }, [initialId])

  async function load(searchId){
    try{
      setLoading(true)
      const res = await api.get(`/turmas/${searchId}`)
      setForm({ nome: res.data?.nome || '', descricao: res.data?.descricao || '' })
      setId(res.data?.id || searchId)
    }catch(err){
      console.error(err)
      showAlert('Falha ao carregar turma')
    }finally{
      setLoading(false)
    }
  }

  function validate(){
    const e = {}
    if(!form.nome || form.nome.length < 3 || form.nome.length > 100) e.nome = 'Nome deve ter entre 3 e 100 caracteres'
    if(!form.descricao || form.descricao.length < 10 || form.descricao.length > 250) e.descricao = 'Descrição deve ter entre 10 e 250 caracteres'
    setErrors(e)
    return Object.keys(e).length === 0
  }

  async function handleSubmit(e){
    e.preventDefault()
    if(!validate()) return
    try{
      if(initialId){
        await api.put(`/turmas/${id}`, form)
        showAlert('Turma atualizada').then(() => onSaved())
      }else{
        await api.post('/turmas', form)
        showAlert('Turma criada').then(() => onSaved())
      }
    }catch(err){
      showAlert(err?.response?.data?.message || 'Erro ao salvar')
    }
  }

  return (
    <div>
      <div style={{display:'flex',justifyContent:'space-between',alignItems:'center'}}>
        <h3 style={{margin:0}}>{initialId ? 'Editar Turma' : 'Cadastrar Turma'}</h3>
        <button className="top-btn" onClick={onCancel}>X</button>
      </div>

      <form onSubmit={handleSubmit} className="form" style={{marginTop:12}}>
        <label>Nome</label>
        <input value={form.nome} onChange={(e) => setForm({ ...form, nome: e.target.value })} />
        {errors.nome && <small className="error">{errors.nome}</small>}

        <label>Descrição</label>
        <textarea value={form.descricao} onChange={(e) => setForm({ ...form, descricao: e.target.value })} />
        {errors.descricao && <small className="error">{errors.descricao}</small>}

        <div style={{ marginTop: 12, display:'flex', gap:8, justifyContent:'flex-end' }}>
          <button type="button" className="btn secondary" onClick={onCancel}>Cancelar</button>
          <button type="submit" className="btn">Salvar</button>
        </div>
      </form>
    </div>
  )
}
