using ObraFacil.Application.DTOs;
using ObraFacil.Application.Interfaces;
using ObraFacil.Domain.Exceptions;
using System.Windows;

namespace ObraFacil.Wpf.Views.Clientes;

public partial class ClienteFormWindow : Window
{
    private readonly IClienteService _service;
    private int? _clienteId;

    public string Titulo    { get => (string)GetValue(TituloProperty); set => SetValue(TituloProperty, value); }
    public static readonly DependencyProperty TituloProperty = DependencyProperty.Register(nameof(Titulo), typeof(string), typeof(ClienteFormWindow));

    public string Nome        { get => (string)GetValue(NomeProperty); set => SetValue(NomeProperty, value); }
    public static readonly DependencyProperty NomeProperty = DependencyProperty.Register(nameof(Nome), typeof(string), typeof(ClienteFormWindow));

    public string? Telefone   { get; set; }
    public string? Email      { get; set; }
    public string? Documento  { get; set; }
    public string? Endereco   { get; set; }
    public string? Observacoes { get; set; }

    public ClienteFormWindow(IClienteService service)
    {
        _service   = service;
        DataContext = this;
        Titulo     = "Novo Cliente";
        InitializeComponent();
    }

    public async void CarregarCliente(int id)
    {
        _clienteId = id;
        Titulo     = "Editar Cliente";
        var c = await _service.ObterAsync(id);
        Nome = c.Nome;
        Telefone    = c.Telefone    ?? string.Empty;
        Email       = c.Email       ?? string.Empty;
        Documento   = c.Documento   ?? string.Empty;
        Endereco    = c.Endereco    ?? string.Empty;
        Observacoes = c.Observacoes ?? string.Empty;
    }

    private async void BtnSalvar_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(Nome)) { MessageBox.Show("Nome é obrigatório.", "Atenção"); return; }
        try
        {
            var dto = new ClienteInputDto(Nome, Telefone, Email, Documento, Endereco, Observacoes);
            if (_clienteId.HasValue) await _service.AtualizarAsync(_clienteId.Value, dto);
            else                     await _service.CriarAsync(dto);
            DialogResult = true;
        }
        catch (ObraFacilException ex) { MessageBox.Show(ex.Message, "Erro"); }
    }

    private void BtnCancelar_Click(object sender, RoutedEventArgs e) => DialogResult = false;
}
