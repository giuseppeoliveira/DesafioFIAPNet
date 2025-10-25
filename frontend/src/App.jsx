import React from 'react'
import { Routes, Route, Navigate } from 'react-router-dom'
import Login from './pages/Login'
import Alunos from './pages/Alunos'
import Turmas from './pages/Turmas'
import AlunoForm from './components/AlunoForm'
import TurmaForm from './components/TurmaForm'
import Home from './pages/Home'
import AdminDashboard from './pages/AdminDashboard'
import ProtectedRoute from './components/ProtectedRoute'
import { AppLayout } from './components/Layout'

export default function App() {
  return (
    <AppLayout>
      <Routes>
        <Route path="/" element={<Home />} />
        <Route path="/login" element={<Login />} />

        <Route path="/admin" element={<ProtectedRoute><AdminDashboard /></ProtectedRoute>} />

        <Route path="/alunos" element={<ProtectedRoute><Alunos /></ProtectedRoute>} />
        <Route path="/alunos/new" element={<ProtectedRoute><AlunoForm /></ProtectedRoute>} />
        <Route path="/alunos/:id/edit" element={<ProtectedRoute><AlunoForm /></ProtectedRoute>} />

        <Route path="/turmas" element={<ProtectedRoute><Turmas /></ProtectedRoute>} />
        <Route path="/turmas/new" element={<ProtectedRoute><TurmaForm /></ProtectedRoute>} />
        <Route path="/turmas/:id/edit" element={<ProtectedRoute><TurmaForm /></ProtectedRoute>} />

        <Route path="*" element={<Navigate to="/" />} />
      </Routes>
    </AppLayout>
  )
}
