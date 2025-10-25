import React, { useEffect, useState } from 'react'
import { useNavigate } from 'react-router-dom'
import api from '../services/api'
import Pagination from '../components/Pagination'
import TurmaFormInline from '../components/TurmaFormInline'
import { FiEdit, FiTrash2, FiInfo } from 'react-icons/fi'
import { useAlert } from '../contexts/AlertContext'

export default function Turmas() {
  const [turmas, setTurmas] = useState([])
  const [page, setPage] = useState(1)
  const [totalPages, setTotalPages] = useState(1)
  const [search, setSearch] = useState('')
  const pageSize = 10
  const navigate = useNavigate()

  const [showDetails, setShowDetails] = useState(false)
  const [details, setDetails] = useState(null)
  const [detailsLoading, setDetailsLoading] = useState(false)
  const [showForm, setShowForm] = useState(false)
  const [editingId, setEditingId] = useState(null)
  const { showAlert, showConfirm } = useAlert()

  useEffect(() => {
    fetchTurmas(search, page)
  }, [page, search])

  async function openDetails(id) {
    if (!id) return
    try {
      setDetailsLoading(true)
      setShowDetails(true)
      const res = await api.get(`/turmas/${id}`)
      // A API pode retornar o objeto diretamente
      const data = res.data
      // normaliza nomes de campos para o formato esperado pela UI (camelCase)
      // o backend pode retornar Nome, Descricao, Id, QuantidadeAlunos, Alunos
      const normalized = {
        id: data.id || data.Id || id,
        nome: data.nome || data.Nome,
        descricao: data.descricao || data.Descricao,
        quantidadeAlunos: data.quantidadeAlunos || data.QuantidadeAlunos,
        alunos: (data.alunos || data.Alunos || []).map(a => ({ id: a.id || a.Id, nome: a.nome || a.Nome }))
      }
      setDetails(normalized)
    } catch (err) {
      console.error(err)
      showAlert('Erro ao carregar detalhes')
      setShowDetails(false)
    } finally {
      setDetailsLoading(false)
    }
  }

  async function handleDesmatricular(alunoId) {
    if (!details?.id) return
    const confirmed = await showConfirm('Confirma remoção da matrícula deste aluno desta turma?')
    if (!confirmed) return
    try {
      await api.delete(`/alunos/${alunoId}/matriculas?turmaId=${details.id}`)
      // refresh details list
      await openDetails(details.id)
      showAlert('Aluno desmatriculado com sucesso')
    } catch (err) {
      console.error(err)
      showAlert(err?.response?.data?.message || 'Erro ao desmatricular')
    }
  }

  async function fetchTurmas(searchQuery, searchPage) {
    try {
      const res = await api.get(`/turmas?nomeQuery=${searchQuery}&pagina=${searchPage}&tamanhoPagina=${pageSize}`)

      const items = res.data.items || []
      const total = res.data.paginas || 1
      
      setTurmas(items)
      setTotalPages(total)
    } catch (err) {
      console.error(err)
    }
  }

  async function handleDelete(id) {
    const confirmed = await showConfirm('Confirma exclusão da turma?')
    if (!confirmed) return
    try {
      await api.delete(`/turmas/${id}`)
      fetchTurmas(search, page)
    } catch (err) {
      showAlert(err?.response?.data?.message || 'Erro ao excluir')
    }
  }

  return (
    <div>
      <div style={{display:'flex',justifyContent:'space-between',alignItems:'center',gap:12}}>
        <div>
          <h2>Turmas</h2>
          <p className="muted">Lista de turmas</p>
        </div>
        <div style={{display:'flex',gap:8,alignItems:'center'}}>
          <input className="search" placeholder="Buscar por nome" value={search} onChange={(e) => setSearch(e.target.value)} />
          <button className="btn" onClick={() => { setEditingId(null); setShowForm(true); }}>Cadastrar Turma</button>
        </div>
      </div>

      <div className="card" style={{marginTop:12}}>
        <table>
          <thead>
            <tr>
              <th>Nome</th>
              <th>Descrição</th>
              <th>Alunos</th>
              <th style={{width:160}}>Ações</th>
            </tr>
          </thead>
          <tbody>
            {turmas.map((t) => (
              <tr key={t.id || t.nome}>
                <td>{t.nome}</td>
                <td>{t.descricao}</td>
                <td>{t.quantidadeAlunos ?? t.alunos?.length ?? '-'}</td>
                <td>
                  <div className="action-group">
                    <button className="icon-btn" title="Editar" onClick={() => { setEditingId(t.id || t.nome); setShowForm(true); }}><FiEdit /></button>
                    <button className="icon-btn" title="Excluir" onClick={() => handleDelete(t.id || t.nome)}><FiTrash2 /></button>
                    <button className="icon-btn" title="Detalhes" onClick={() => openDetails(t.id)}><FiInfo /></button>
                  </div>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>

      {/* Detalhes modal */}
      {showDetails && (
        <div className="modal-overlay">
          <div className="modal">
            {detailsLoading ? (
              <p>Carregando...</p>
            ) : (
              <>
                <h3>{details?.nome}</h3>
                <p className="muted">{details?.descricao}</p>
                <h4>Alunos matriculados ({details?.quantidadeAlunos ?? '-'})</h4>
                <div style={{maxHeight:300,overflow:'auto'}}>
                  {details?.alunos && details.alunos.length > 0 ? (
                    <ul>
                      {details.alunos.map((a) => (
                        <li key={a.id} style={{padding:6,display:'flex',justifyContent:'space-between',alignItems:'center'}}>
                          <div>{a.nome}</div>
                          <div style={{display:'flex',gap:8}}>
                              <button className="btn secondary" onClick={() => handleDesmatricular(a.id)}>Desmatricular</button>
                            </div>
                        </li>
                      ))}
                    </ul>
                  ) : (
                    <p className="muted">Nenhum aluno matriculado</p>
                  )}
                </div>
                <div style={{marginTop:12,display:'flex',justifyContent:'flex-end'}}>
                  <button className="btn" onClick={() => { setShowDetails(false); setDetails(null); }}>Fechar</button>
                </div>
              </>
            )}
          </div>
        </div>
      )}
    
        {showForm && (
          <div className="modal-overlay">
            <div className="modal modal--wide">
              <TurmaFormInline id={editingId} onCancel={() => { setShowForm(false); setEditingId(null); }} onSaved={() => { setShowForm(false); setEditingId(null); fetchTurmas(search, page); }} />
            </div>
          </div>
        )}
      <div style={{marginTop:12}}>
        <Pagination page={page} totalPages={totalPages} onChange={setPage} />
      </div>
    </div>
  )
}
