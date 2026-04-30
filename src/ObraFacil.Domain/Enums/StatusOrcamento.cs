namespace ObraFacil.Domain.Enums;

/// <summary>Define os possíveis estados de um orçamento no fluxo de trabalho.</summary>
public enum StatusOrcamento
{
    /// <summary>Orçamento em elaboração, ainda não enviado ao cliente.</summary>
    Rascunho = 1,

    /// <summary>Orçamento enviado ao cliente aguardando retorno.</summary>
    Enviado = 2,

    /// <summary>Orçamento aceito e aprovado pelo cliente.</summary>
    Aprovado = 3,

    /// <summary>Orçamento recusado pelo cliente.</summary>
    Recusado = 4
}
