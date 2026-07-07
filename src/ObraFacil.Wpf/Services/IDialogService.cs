namespace ObraFacil.Wpf.Services;

public interface IDialogService
{
    void ShowInfo(string message, string title = "Informação");
    void ShowWarning(string message, string title = "Atenção");
    void ShowError(string message, string title = "Erro");
    bool Confirm(string message, string title = "Confirmar");
    string? SelectFile(string title, string filter);
}
