# ObraFácil

Sistema de orçamentos para construção civil — Desktop Windows (WPF).  
Offline-first · .NET 8 · SQLite · MVVM

## Estrutura

```
obra-facil/
├── src/
│   ├── ObraFacil.Domain/          # Entidades, enums, interfaces
│   ├── ObraFacil.Application/     # Casos de uso, DTOs, serviços
│   ├── ObraFacil.Infrastructure/  # EF Core + SQLite, repositórios, PDF
│   └── ObraFacil.Wpf/             # UI WPF (MVVM)
└── tests/
```

## Rodar

```bash
dotnet run --project src/ObraFacil.Wpf
```

## Primeira migration

```bash
dotnet ef migrations add Init \
  --project src/ObraFacil.Infrastructure \
  --startup-project src/ObraFacil.Wpf
dotnet ef database update \
  --project src/ObraFacil.Infrastructure \
  --startup-project src/ObraFacil.Wpf
```
