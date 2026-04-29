using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using ObraFacil.Application.Interfaces;
using ObraFacil.Domain.Interfaces;
using System.Windows;

namespace ObraFacil.Wpf.ViewModels.Configuracoes;

public partial class ConfiguracoesViewModel : ViewModelBase
{
    private readonly IConfiguracaoRepository _repo;
    private readonly IBackupService          _backup;

    [ObservableProperty] string  _nomeEmpresa           = string.Empty;
    [ObservableProperty] string? _telefoneEmpresa;
    [ObservableProperty] string? _emailEmpresa;
    [ObservableProperty] string? _enderecoEmpresa;
    [ObservableProperty] int     _validadePadraoEmDias  = 15;
    [ObservableProperty] int     _proximoNumeroOrcamento = 1;

    public ConfiguracoesViewModel(IConfiguracaoRepository repo, IBackupService backup,
        ILoggerFactory loggerFactory) : base(loggerFactory)
    {
        _repo   = repo;
        _backup = backup;
        Title   = "Configurações";
    }

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

    [RelayCommand]
    async Task ExportarBackupAsync()
        => await ExecuteSafeAsync(async () =>
        {
            var path = await _backup.ExportarAsync();
            MessageBox.Show($"Backup salvo em:\n{path}", "Backup concluído");
        }, "Erro ao exportar backup.");

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
