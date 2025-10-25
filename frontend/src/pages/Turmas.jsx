import React, { useEffect, useState } from 'react'
import { useNavigate } from 'react-router-dom'
import api from '../services/api'
import Pagination from '../components/Pagination'

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

  useEffect(() => {
    fetchTurmas(search, page)
  }, [page, search])

  async function openDetails(id) {
    if (!id) return
    try {
      setDetailsLoading(true)
      setShowDetails(true)
      const res = await api.get(`/turmas/${id}`)
      // API may return object directly
      const data = res.data
      // normalize field names to lowercase keys expected in UI
      // backend returns Nome, Descricao, Id, QuantidadeAlunos, Alunos (but frontend expects camelCase)
      const normalized = {
        nome: data.nome || data.Nome,
        descricao: data.descricao || data.Descricao,
        quantidadeAlunos: data.quantidadeAlunos || data.QuantidadeAlunos,
        alunos: (data.alunos || data.Alunos || []).map(a => ({ id: a.id || a.Id, nome: a.nome || a.Nome }))
      }
      setDetails(normalized)
    } catch (err) {
      console.error(err)
      alert('Erro ao carregar detalhes')
      setShowDetails(false)
    } finally {
      setDetailsLoading(false)
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
    if (!confirm('Confirma exclusão da turma?')) return
    try {
      await api.delete(`/turmas/${id}`)
      fetchTurmas(search, page)
    } catch (err) {
      alert(err?.response?.data?.message || 'Erro ao excluir')
    }
  }

  return (
    <div>
      <div style={{display:'flex',justifyContent:'space-between',alignItems:'center',gap:12}}>
        <div>
          <h2>Turmas</h2>
          <p className="muted">Listagem de turmas — mostre quantos alunos por turma</p>
        </div>
        <div style={{display:'flex',gap:8,alignItems:'center'}}>
          <input className="search" placeholder="Buscar por nome" value={search} onChange={(e) => setSearch(e.target.value)} />
          <button className="btn" onClick={() => navigate('/turmas/new')}>Nova Turma</button>
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
                  <button className="btn" onClick={() => navigate(`/turmas/${t.id || t.nome}/edit`)}>Editar</button>
                  <button style={{marginLeft:8}} className="btn" onClick={() => handleDelete(t.id || t.nome)}>Excluir</button>
                  <button style={{marginLeft:8}} className="btn" onClick={() => openDetails(t.id)}>Detalhes</button>
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
                          <div>
                            <button className="btn" onClick={() => navigate(`/alunos/${a.id}/edit`)}>Ver aluno</button>
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

      <div style={{marginTop:12}}>
        <Pagination page={page} totalPages={totalPages} onChange={setPage} />
      </div>
    </div>
  )
}
