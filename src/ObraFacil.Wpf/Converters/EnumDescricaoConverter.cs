using ObraFacil.Domain.Enums;
using System.Globalization;
using System.Windows.Data;

namespace ObraFacil.Wpf.Converters;

public class StatusOrcamentoConverter : IValueConverter
{
    public object Convert(object value, Type _, object __, CultureInfo ___)
        => value is StatusOrcamento s ? s switch
        {
            StatusOrcamento.Rascunho => "Rascunho",
            StatusOrcamento.Enviado  => "Enviado",
            StatusOrcamento.Aprovado => "Aprovado",
            StatusOrcamento.Recusado => "Recusado",
            _                        => value.ToString()!
        } : value?.ToString() ?? string.Empty;

    public object ConvertBack(object value, Type _, object __, CultureInfo ___)
        => throw new NotImplementedException();
}

public class TipoItemConverter : IValueConverter
{
    public object Convert(object value, Type _, object __, CultureInfo ___)
        => value is TipoItem t ? (t == TipoItem.Material ? "Material" : "Serviço") : string.Empty;

    public object ConvertBack(object value, Type _, object __, CultureInfo ___)
        => throw new NotImplementedException();
}

public class UnidadeMedidaConverter : IValueConverter
{
    public object Convert(object value, Type _, object __, CultureInfo ___)
        => value is UnidadeMedida u ? u switch
        {
            UnidadeMedida.Unidade    => "un",
            UnidadeMedida.Metro      => "m",
            UnidadeMedida.MetroQ     => "m²",
            UnidadeMedida.MetroC     => "m³",
            UnidadeMedida.Hora       => "h",
            UnidadeMedida.Quilograma => "kg",
            UnidadeMedida.Litro      => "L",
            _                        => value.ToString()!
        } : string.Empty;

    public object ConvertBack(object value, Type _, object __, CultureInfo ___)
        => throw new NotImplementedException();
}

public class BoolToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type _, object __, CultureInfo ___)
        => value is true
            ? System.Windows.Visibility.Visible
            : System.Windows.Visibility.Collapsed;

    public object ConvertBack(object value, Type _, object __, CultureInfo ___)
        => throw new NotImplementedException();
}
