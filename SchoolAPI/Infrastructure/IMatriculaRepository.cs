using Microsoft.EntityFrameworkCore;
using SchoolAPI.Domain.Entities;

namespace SchoolAPI.Infrastructure;

public interface IMatriculaRepository
{
    Task<Matricula?> MatriculaMesmoAlunoTurma(int alunoId, int turmaId, CancellationToken cancellationToken);
    Task AdicionarMatricula(Matricula matricula, CancellationToken cancellationToken);
}

public class MatriculaRepository : IMatriculaRepository
{
    private readonly SchoolAPIContext schoolAPIContext;

    public MatriculaRepository(SchoolAPIContext schoolAPIContext)
    {
        this.schoolAPIContext = schoolAPIContext;
    }
    public async Task AdicionarMatricula(Matricula matricula, CancellationToken cancellationToken)
    {
        await schoolAPIContext.Matricula.AddAsync(matricula, cancellationToken);
        await schoolAPIContext.SaveChangesAsync(cancellationToken);
    }

    public Task<Matricula?> MatriculaMesmoAlunoTurma(int alunoId, int turmaId, CancellationToken cancellationToken)
    {
        return schoolAPIContext.Matricula
            .FirstOrDefaultAsync(m => m.Alunoid == alunoId && m.Turmaid == turmaId, cancellationToken);
    }
}
