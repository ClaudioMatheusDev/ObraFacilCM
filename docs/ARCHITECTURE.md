# Arquitetura — ObraFácil (WPF)

## Camadas

```
┌──────────────────────────────────────────────────────┐
│  ObraFacil.Wpf  (Presentation — WPF)                 │
│  App.xaml (DI bootstrap) / MainWindow (navegação)    │
│  Pages: lista/form por módulo                         │
│  ViewModels: MVVM com CommunityToolkit               │
│  Converters / Styles                                 │
└─────────────────────┬────────────────────────────────┘
                      │ usa interfaces
┌─────────────────────▼────────────────────────────────┐
│  ObraFacil.Application                               │
│  Services / DTOs / Interfaces                        │
└─────────────────────┬────────────────────────────────┘
                      │ usa interfaces de repositório
┌─────────────────────▼────────────────────────────────┐
│  ObraFacil.Domain                                    │
│  Entities / Enums / Interfaces / Exceptions          │
└──────────────────────────────────────────────────────┘
                      ▲ implementa
┌─────────────────────┴────────────────────────────────┐
│  ObraFacil.Infrastructure                            │
│  EF Core + SQLite / Repositories / PDF (QuestPDF)    │
└──────────────────────────────────────────────────────┘
```

## Regra de snapshot

Quando um item é adicionado ao orçamento, `OrcamentoService.MapItem()` copia
`Descricao`, `Unidade`, `PrecoUnitario` e `Categoria` para campos `*Snapshot`
no `ItemOrcamento`. Atualizações futuras no catálogo não afetam orçamentos já salvos.

## Banco de dados

Localização: `%LocalAppData%\ObraFacil\obrafacil.db`  
Migration via EF Core CLI — ver README.

## Migrar para online (v2)

Criar `ObraFacil.Api` (ASP.NET Core) que reutiliza `Application` + `Infrastructure`.  
Trocar implementações de `IRepository<T>` por clientes HTTP — Domain/Application não mudam.
