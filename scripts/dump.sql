-- ===========================================
-- TABELA: USUARIO_ADMIN
-- ===========================================
CREATE TABLE UsuarioAdmin (
    Id SERIAL PRIMARY KEY,
    Email VARCHAR(150) UNIQUE NOT NULL,
    Senha BYTEA NOT NULL,  -- senha criptografada
    Salt BYTEA NOT NULL,   -- salt usado na criptografia
    DataCriado TIMESTAMPTZ DEFAULT NOW()
);

-- ===========================================
-- TABELA: ALUNO
-- ===========================================
CREATE TABLE Aluno (
    Id SERIAL PRIMARY KEY,
    Nome VARCHAR(100) NOT NULL,
    DataNascimento DATE NOT NULL,
    CPF CHAR(11) NOT NULL UNIQUE,
    Email VARCHAR(150) NOT NULL UNIQUE,
    DataCriado TIMESTAMPTZ DEFAULT NOW(),
    Senha BYTEA NOT NULL,  -- senha criptografada
    Salt BYTEA NOT NULL   -- salt usado na criptografia
);

-- ===========================================
-- TABELA: TURMA
-- ===========================================
CREATE TABLE Turma (
    Id SERIAL PRIMARY KEY,
    Nome VARCHAR(100) NOT NULL,
    Descricao VARCHAR(250) NOT NULL,
    DataCriado TIMESTAMPTZ DEFAULT NOW()
);

-- ===========================================
-- TABELA: MATRICULA (relação aluno-turma)
-- ===========================================
CREATE TABLE matricula (
    Id SERIAL PRIMARY KEY,
    AlunoId INT NOT NULL REFERENCES Aluno(id) ON DELETE CASCADE,
    TurmaId INT NOT NULL REFERENCES Turma(id) ON DELETE CASCADE,
    DataMatricula TIMESTAMPTZ DEFAULT NOW(),
    UNIQUE (AlunoId, TurmaId)  -- evita matrícula duplicada
);

-- ===========================================
-- ÍNDICES DE BUSCA
-- ===========================================
CREATE INDEX idx_aluno_nome ON aluno (nome);
CREATE INDEX idx_aluno_cpf ON aluno (cpf);


INSERT INTO usuarioadmin (email,senha,salt,datacriado) VALUES
	 ('admin@gmail.com',decode('70D792FF1DE491CBFF7EB44D040D21CE658B23CA37674FF318C91BAD4AF8AD8B','hex'),decode('DD9047E4F7AD93D5A91DBF18B6DB804537B5F8CDCCE9189213E008A70F6AFCA1','hex'),'2025-10-24 22:29:09.907503');
