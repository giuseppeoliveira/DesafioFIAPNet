import React, { useEffect, useState } from 'react'
import { Link, useNavigate } from 'react-router-dom'
import api from '../services/api'
import Pagination from '../components/Pagination'
import AlunoFormInline from '../components/AlunoFormInline'
import { FiEdit, FiTrash2, FiUserPlus } from 'react-icons/fi'
import { formatCPF, formatDate } from '../utils/format'
import { useAlert } from '../contexts/AlertContext'

export default function Alunos() {
  const [alunos, setAlunos] = useState([])
  const [page, setPage] = useState(1)
  const [totalPages, setTotalPages] = useState(1)
  const [search, setSearch] = useState('')
  const [searchType, setSearchType] = useState('cpf') // 'cpf' or 'nome'
  const navigate = useNavigate()

  const [showSelector, setShowSelector] = useState(false)
  const [selectedAluno, setSelectedAluno] = useState(null)
  const [turmaSearch, setTurmaSearch] = useState('')
  const [turmasResults, setTurmasResults] = useState([])
  // selected turma ids for current open selector (Set)
  const [selectedTurmaIds, setSelectedTurmaIds] = useState(new Set())
  // persistent map alunoId -> array of turmaIds (kept in localStorage so selection 'persists')
  const [selectedMap, setSelectedMap] = useState(() => {
    try {
      const raw = localStorage.getItem('matriculas_map')
      return raw ? JSON.parse(raw) : {}
    } catch (e) { return {} }
  })
  const [showForm, setShowForm] = useState(false)
  const [editingCpf, setEditingCpf] = useState(null)

  const pageSize = 10
''
  const { showAlert, showConfirm } = useAlert()

  useEffect(() => {
    fetchAlunos(search, page)
  }, [page, search])

  async function fetchAlunos(searchQuery, searchPage) {
    try {
      // send both params; backend will interpret empty strings as no filter
      const cpfQuery = searchType === 'cpf' ? searchQuery : ''
      const nomeQuery = searchType === 'nome' ? searchQuery : ''
      const res = await api.get(`/alunos?cpfQuery=${encodeURIComponent(cpfQuery)}&nomeQuery=${encodeURIComponent(nomeQuery)}&pagina=${searchPage}`)

      const items = res.data.items || []
      const total = res.data.paginas || 1

      setAlunos(items)
      setTotalPages(total)
    } catch (err) {
      console.error(err)
    }
  }

  async function handleDelete(id) {
    const confirmed = await showConfirm('Confirma exclusão do aluno?')
    if (!confirmed) return
    try {
      await api.delete(`/alunos/${id}`)
      // refetch current page
      fetchAlunos(search, page)
    } catch (err) {
      showAlert(err?.response?.data?.message || 'Erro ao excluir')
    }
  }

  function openTurmaSelector(aluno) {
    setSelectedAluno(aluno)
    setShowSelector(true)
    setTurmaSearch('')
    setTurmasResults([])
    // carregar primeiras turmas
    fetchTurmasForSelector('')
    // initialize selected set from persisted map (if exists)
    const existing = (selectedMap && selectedMap[aluno.id]) || []
    setSelectedTurmaIds(new Set(existing))
  }

  async function fetchTurmasForSelector(searchQuery) {
    try {
      const res = await api.get(`/turmas?nomeQuery=${encodeURIComponent(searchQuery || '')}&pagina=1&tamanhoPagina=100`)
      const items = res.data.items || []
      // sort alphabetically for a predictable order
      items.sort((a,b) => (a.nome || '').localeCompare(b.nome || ''))
      setTurmasResults(items)
    } catch (err) {
      console.error(err)
      showAlert('Erro ao buscar turmas')
    }
  }

  // Toggle selection for a turma in the current selector
  function toggleTurmaSelection(turmaId) {
    setSelectedTurmaIds(prev => {
      const next = new Set(prev)
      if (next.has(turmaId)) next.delete(turmaId)
      else next.add(turmaId)
      return next
    })
  }

  async function doMatricularSelected() {
    if (!selectedAluno || !selectedAluno.id) {
      showAlert('Aluno não possui id; não é possível matricular. Edite o aluno para garantir que ele tenha um id numérico.')
      return
    }

    const toMatricular = Array.from(selectedTurmaIds)
    if (toMatricular.length === 0) {
      showAlert('Selecione ao menos uma turma')
      return
    }

    const results = { ok: [], skipped: [], errors: [] }

    for (const turmaId of toMatricular) {
      try {
        await api.post(`/alunos/${selectedAluno.id}/matriculas?turmaId=${turmaId}`)
        results.ok.push(turmaId)
      } catch (err) {
        const code = err?.response?.status
        // 403 means already matriculado (service throws InvalidOperationException)
        if (code === 403) results.skipped.push(turmaId)
        else results.errors.push({ turmaId, message: err?.response?.data?.message || err.message })
      }
    }

    // persist selection map (keep checked turmas for this aluno)
    const nextMap = { ...selectedMap }
    nextMap[selectedAluno.id] = Array.from(selectedTurmaIds)
    setSelectedMap(nextMap)
    try { localStorage.setItem('matriculas_map', JSON.stringify(nextMap)) } catch (e) {}

    // show summary
    if (results.ok.length > 0) await showAlert('Matricula(s) realizada(s) com sucesso')
    else if (results.skipped.length > 0 && results.errors.length === 0) await showAlert('Nenhuma nova matricula necessária (já estava matriculado)')
    else if (results.errors.length > 0) showAlert('Erro(s) ao matricular algumas turmas')

    // close selector and refresh list
    setShowSelector(false)
    setSelectedAluno(null)
    fetchAlunos(search, page)
  }

  function handleFormSaved() {
    setShowForm(false)
    setEditingCpf(null)
    fetchAlunos(search, page)
  }

  return (
    <div>
      <div style={{display:'flex',justifyContent:'space-between',alignItems:'center',gap:12}}>
        <div>
          <h2>Alunos</h2>
          <p className="muted">Lista de alunos</p>
        </div>
        <div style={{display:'flex',gap:8,alignItems:'center'}}>
          <select value={searchType} onChange={(e) => setSearchType(e.target.value)} style={{height:36,borderRadius:8,padding:'6px 8px'}}>
            <option value="cpf">CPF</option>
            <option value="nome">Nome</option>
          </select>
          <input className="search" placeholder={searchType === 'cpf' ? 'Buscar por CPF' : 'Buscar por nome'} value={search} onChange={(e) => setSearch(e.target.value)} />
          <button className="btn" onClick={() => { setEditingCpf(null); setShowForm(true); }}>Cadastrar Aluno</button>
        </div>
      </div>

      <div className="card" style={{marginTop:12}}>
        <table>
          <thead>
            <tr>
              <th>Nome</th>
              <th>CPF</th>
              <th>Data Nasc.</th>
              <th>Email</th>
              <th style={{width:160}}>Ações</th>
            </tr>
          </thead>
          <tbody>
            {alunos.map((a) => (
              <tr key={a.id || a.cpf}>
                <td>{a.nome}</td>
                <td>{formatCPF(a.cpf)}</td>
                <td>{formatDate(a.dataNascimento)}</td>
                <td>{a.email}</td>
                <td>
                  <div className="action-group">
                    <button className="icon-btn" title="Editar" onClick={() => { setEditingCpf(a.cpf); setShowForm(true); }}><FiEdit /></button>
                    <button className="icon-btn" title="Excluir" onClick={() => handleDelete(a.id || a.cpf)}><FiTrash2 /></button>
                    <button className="icon-btn" title="Matricular" onClick={() => openTurmaSelector(a)}><FiUserPlus /></button>
                  </div>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>

      {/* Turma selector modal */}
      {showSelector && (
        <div className="modal-overlay">
          <div className="modal">
            <h3>Matricular: {selectedAluno?.nome}</h3>
            <div style={{display:'flex',gap:8,marginBottom:8}}>
              <input className="search" placeholder="Buscar turmas por nome" value={turmaSearch} onChange={(e) => setTurmaSearch(e.target.value)} />
              <button className="btn" onClick={() => fetchTurmasForSelector(turmaSearch)}>Buscar</button>
            </div>
            <div style={{maxHeight:240,overflow:'auto',border:'1px solid #eee',borderRadius:8,padding:8}}>
              {turmasResults.length === 0 && <p className="muted">Nenhuma turma encontrada</p>}
              <ul style={{listStyle:'none',margin:0,padding:0,display:'grid',gap:6}}>
                {turmasResults.map((t) => {
                  const tid = t.id || t.nome
                  const checked = selectedTurmaIds.has(tid)
                  return (
                    <li key={tid} style={{display:'flex',justifyContent:'space-between',alignItems:'center',gap:8,padding:8,borderRadius:6,background: checked ? 'rgba(91,43,138,0.06)' : 'transparent',cursor:'pointer'}} onClick={() => toggleTurmaSelection(tid)}>
                      <div>
                        <strong>{t.nome}</strong>
                        <div className="muted">{t.descricao}</div>
                      </div>
                      <div style={{minWidth:120,display:'flex',justifyContent:'flex-end'}}>
                        <label style={{display:'inline-flex',alignItems:'center',gap:8}}>
                          <input type="checkbox" checked={checked} onChange={() => toggleTurmaSelection(tid)} />
                          <span className="muted">Selecionar</span>
                        </label>
                      </div>
                    </li>
                  )
                })}
              </ul>
            </div>
            <div style={{marginTop:12,display:'flex',justifyContent:'space-between',alignItems:'center'}}>
              <div>
                <button className="btn secondary" onClick={() => { setShowSelector(false); setSelectedAluno(null); setSelectedTurmaIds(new Set()); }}>Fechar</button>
              </div>
              <div>
                <button className="btn" disabled={selectedTurmaIds.size === 0} onClick={() => doMatricularSelected()}>Matricular</button>
              </div>
            </div>
          </div>
        </div>
      )}

      {/* Aluno form modal (create / edit inline) */}
      {showForm && (
        <div className="modal-overlay">
          <div className="modal modal--wide">
            <AlunoFormInline cpf={editingCpf} onCancel={() => { setShowForm(false); setEditingCpf(null); }} onSaved={handleFormSaved} />
          </div>
        </div>
      )}

      <div style={{marginTop:12}}>
        <Pagination page={page} totalPages={totalPages} onChange={setPage} />
      </div>
    </div>
  )
}
