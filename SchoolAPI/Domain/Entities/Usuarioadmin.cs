using System;
using System.Collections.Generic;

namespace SchoolAPI.Domain.Entities;

public partial class Usuarioadmin
{
    public int Id { get; set; }

    public string Email { get; set; } = null!;

    public byte[] Senha { get; set; } = null!;

    public byte[] Salt { get; set; } = null!;

    public DateTime? Datacriado { get; set; }
}
