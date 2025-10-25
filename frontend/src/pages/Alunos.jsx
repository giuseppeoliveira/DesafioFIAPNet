import React, { useEffect, useState } from 'react'
import { Link, useNavigate } from 'react-router-dom'
import api from '../services/api'
import Pagination from '../components/Pagination'

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

  const pageSize = 10
''
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
    if (!confirm('Confirma exclusão do aluno?')) return
    try {
      await api.delete(`/alunos/${id}`)
      fetchAlunos('[]', 1)
    } catch (err) {
      alert(err?.response?.data?.message || 'Erro ao excluir')
    }
  }

  function openTurmaSelector(aluno) {
    setSelectedAluno(aluno)
    setShowSelector(true)
    setTurmaSearch('')
    setTurmasResults([])
    // carregar primeiras turmas
    fetchTurmasForSelector('')
  }

  async function fetchTurmasForSelector(searchQuery) {
    try {
      const res = await api.get(`/turmas?nomeQuery=${encodeURIComponent(searchQuery || '')}&pagina=1&tamanhoPagina=10`)
      const items = res.data.items || []
      setTurmasResults(items)
    } catch (err) {
      console.error(err)
      alert('Erro ao buscar turmas')
    }
  }

  async function selectTurma(turma) {
    if (!selectedAluno || !selectedAluno.id) {
      alert('Aluno não possui id; não é possível matricular. Edite o aluno para garantir que ele tenha um id numérico.')
      return
    }

    try {
      await api.post(`/alunos/${selectedAluno.id}/matriculas?turmaId=${turma.id}`)
      alert('Aluno matriculado com sucesso')
      setShowSelector(false)
      setSelectedAluno(null)
      // opcional: atualizar lista de alunos ou turmas se necessário
      fetchAlunos(search, page)
    } catch (err) {
      alert(err?.response?.data?.message || 'Erro ao matricular')
    }
  }

  return (
    <div>
      <div style={{display:'flex',justifyContent:'space-between',alignItems:'center',gap:12}}>
        <div>
          <h2>Alunos</h2>
          <p className="muted">Listagem de alunos — ordenada e paginada</p>
        </div>
        <div style={{display:'flex',gap:8,alignItems:'center'}}>
          <select value={searchType} onChange={(e) => setSearchType(e.target.value)} style={{height:32}}>
            <option value="cpf">CPF</option>
            <option value="nome">Nome</option>
          </select>
          <input className="search" placeholder={searchType === 'cpf' ? 'Buscar por CPF' : 'Buscar por nome'} value={search} onChange={(e) => setSearch(e.target.value)} />
          <button className="btn" onClick={() => navigate('/alunos/new')}>Novo Aluno</button>
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
                <td>{a.cpf}</td>
                <td>{a.dataNascimento}</td>
                <td>{a.email}</td>
                <td>
                  <button className="btn" onClick={() => navigate(`/alunos/${a.cpf}/edit`)}>Editar</button>
                  <button style={{marginLeft:8}} className="btn" onClick={() => handleDelete(a.id || a.cpf)}>Excluir</button>
                  <button style={{marginLeft:8}} className="btn" onClick={() => openTurmaSelector(a)}>Matricular</button>
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
            <div style={{maxHeight:240,overflow:'auto'}}>
              {turmasResults.length === 0 && <p className="muted">Nenhuma turma encontrada</p>}
              <ul>
                {turmasResults.map((t) => (
                  <li key={t.id || t.nome} style={{display:'flex',justifyContent:'space-between',alignItems:'center',gap:8,padding:6}}>
                    <div>
                      <strong>{t.nome}</strong>
                      <div className="muted">{t.descricao}</div>
                    </div>
                    <div>
                      <button className="btn" onClick={() => selectTurma(t)}>Selecionar</button>
                    </div>
                  </li>
                ))}
              </ul>
            </div>
            <div style={{marginTop:12,display:'flex',justifyContent:'flex-end'}}>
              <button className="btn" onClick={() => { setShowSelector(false); setSelectedAluno(null); }}>Fechar</button>
            </div>
          </div>
        </div>
      )}

      <div style={{marginTop:12}}>
        <Pagination page={page} totalPages={totalPages} onChange={setPage} />
      </div>
    </div>
  )
}
