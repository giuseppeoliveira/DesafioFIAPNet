using System;
using System.Collections.Generic;

namespace SchoolAPI.Domain.Entities;

public partial class Matricula
{
    public int Id { get; set; }

    public int Alunoid { get; set; }

    public int Turmaid { get; set; }

    public DateTime? Datamatricula { get; set; }

    public virtual Aluno Aluno { get; set; } = null!;

    public virtual Turma Turma { get; set; } = null!;
}
