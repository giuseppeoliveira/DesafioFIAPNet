using System;
using System.Collections.Generic;

namespace SchoolAPI.Domain.Entities;

public partial class Turma
{
    public int Id { get; set; }

    public string Nome { get; set; } = null!;

    public string Descricao { get; set; } = null!;

    public DateTime? Datacriado { get; set; }

    public virtual ICollection<Matricula> Matricula { get; set; } = new List<Matricula>();
}
