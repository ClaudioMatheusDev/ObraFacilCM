namespace ObraFacil.Domain.Entities;
public class Configuracao
{
    public int     Id                      { get; set; } = 1;
    public string  NomeEmpresa             { get; set; } = string.Empty;
    public string? TelefoneEmpresa         { get; set; }
    public string? EmailEmpresa            { get; set; }
    public string? EnderecoEmpresa         { get; set; }
    public string? LogoBase64              { get; set; }
    public int     ProximoNumeroOrcamento  { get; set; } = 1;
    public int     ValidadePadraoEmDias    { get; set; } = 15;
}
