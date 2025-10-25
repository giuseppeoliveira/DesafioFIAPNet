using System;
using System.Collections.Generic;

namespace SchoolAPI.Domain.Entities;

public partial class Aluno
{
    public int Id { get; set; }

    public string Nome { get; set; } = null!;

    public DateOnly Datanascimento { get; set; }

    public string Cpf { get; set; } = null!;

    public string Email { get; set; } = null!;

    public DateTime? Datacriado { get; set; }

    public byte[] Senha { get; set; } = null!;

    public byte[] Salt { get; set; } = null!;

    public virtual ICollection<Matricula> Matricula { get; set; } = new List<Matricula>();
}
