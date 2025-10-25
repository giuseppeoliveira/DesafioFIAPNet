namespace SchoolAPI.Domain;


public record PaginacaoQuery (
    int Pagina = 1,
    int TamanhoPagina = 10
)
{
}

public record PaginacaoResponse<TItem>(
    int QntdItens,  
    int Paginas,
    IEnumerable<TItem> Items
)
{
}