using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using ObraFacil.Application.Interfaces;
using ObraFacil.Domain.Interfaces;
using System.Windows;

namespace ObraFacil.Wpf.ViewModels.Configuracoes;

/// <summary>
/// ViewModel da tela de configurações globais da aplicação.
/// Carrega e persiste os dados da empresa, prazo de validade padrão e próximo número de orçamento.
/// Também oferece operações de exportação e restauração de backup do banco SQLite.
/// </summary>
public partial class ConfiguracoesViewModel : ViewModelBase
{
    private readonly IConfiguracaoRepository _repo;
    private readonly IBackupService          _backup;

    /// <summary>Nome fantasia ou razão social exibido no cabeçalho dos PDFs.</summary>
    [ObservableProperty] string  _nomeEmpresa           = string.Empty;
    /// <summary>Telefone de contato da empresa (opcional).</summary>
    [ObservableProperty] string? _telefoneEmpresa;
    /// <summary>E-mail de contato da empresa (opcional).</summary>
    [ObservableProperty] string? _emailEmpresa;
    /// <summary>Endereço da empresa exibido nos orçamentos (opcional).</summary>
    [ObservableProperty] string? _enderecoEmpresa;
    /// <summary>Prazo de validade padrão para novos orçamentos (dias).</summary>
    [ObservableProperty] int     _validadePadraoEmDias  = 15;
    /// <summary>Próximo número sequencial a ser atribuído a um orçamento.</summary>
    [ObservableProperty] int     _proximoNumeroOrcamento = 1;

    /// <summary>Inicializa o ViewModel com repositório de configurações, serviço de backup e f\u00e1brica de loggers.</summary>
    public ConfiguracoesViewModel(IConfiguracaoRepository repo, IBackupService backup,
        ILoggerFactory loggerFactory) : base(loggerFactory)
    {
        _repo   = repo;
        _backup = backup;
        Title   = "Configurações";
    }

    /// <summary>Carrega as configurações do banco e popula as propriedades observáveis.</summary>
    [RelayCommand]
    public async Task CarregarAsync()
        => await ExecuteSafeAsync(async () =>
        {
            var c = await _repo.GetAsync();
            NomeEmpresa             = c.NomeEmpresa;
            TelefoneEmpresa         = c.TelefoneEmpresa;
            EmailEmpresa            = c.EmailEmpresa;
            EnderecoEmpresa         = c.EnderecoEmpresa;
            ValidadePadraoEmDias    = c.ValidadePadraoEmDias;
            ProximoNumeroOrcamento  = c.ProximoNumeroOrcamento;
        });

    /// <summary>Persiste as configurações editadas pelo usuário e exibe confirmação.</summary>
    [RelayCommand]
    async Task SalvarAsync()
        => await ExecuteSafeAsync(async () =>
        {
            var c = await _repo.GetAsync();
            c.NomeEmpresa             = NomeEmpresa.Trim();
            c.TelefoneEmpresa         = string.IsNullOrWhiteSpace(TelefoneEmpresa)  ? null : TelefoneEmpresa.Trim();
            c.EmailEmpresa            = string.IsNullOrWhiteSpace(EmailEmpresa)     ? null : EmailEmpresa.Trim();
            c.EnderecoEmpresa         = string.IsNullOrWhiteSpace(EnderecoEmpresa)  ? null : EnderecoEmpresa.Trim();
            c.ValidadePadraoEmDias    = ValidadePadraoEmDias;
            c.ProximoNumeroOrcamento  = ProximoNumeroOrcamento;
            await _repo.SalvarAsync(c);
            MessageBox.Show("Configurações salvas com sucesso.", "Sucesso");
        });

    /// <summary>Exporta o banco SQLite para a pasta de backups e informa o caminho resultante.</summary>
    [RelayCommand]
    async Task ExportarBackupAsync()
        => await ExecuteSafeAsync(async () =>
        {
            var path = await _backup.ExportarAsync();
            MessageBox.Show($"Backup salvo em:\n{path}", "Backup concluído");
        }, "Erro ao exportar backup.");

    /// <summary>
    /// Solicita ao usuário um arquivo de backup, pede confirmação e restaura o banco SQLite.
    /// Exibe orientação para reiniciar o aplicativo após a restauração.
    /// </summary>
    [RelayCommand]
    async Task RestaurarBackupAsync()
        => await ExecuteSafeAsync(async () =>
        {
            var dlg = new Microsoft.Win32.OpenFileDialog
            {
                Title  = "Selecionar backup",
                Filter = "SQLite|*.db;*.sqlite|Todos|*.*"
            };
            if (dlg.ShowDialog() != true) return;
            var r = MessageBox.Show(
                "Restaurar o backup vai sobrescrever todos os dados atuais. Continuar?",
                "Atenção", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (r != MessageBoxResult.Yes) return;
            await _backup.RestaurarAsync(dlg.FileName);
            MessageBox.Show("Backup restaurado. Reinicie o aplicativo.", "Sucesso");
        }, "Erro ao restaurar backup.");
}
