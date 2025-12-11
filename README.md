# Web Games
[![Test](https://github.com/byr0n3/WebGames/actions/workflows/test.yml/badge.svg)](https://github.com/byr0n3/WebGames/actions/workflows/test.yml)

A web-app for playing simple games, made in C# & Blazor.

## Name suggestions

- **_Virtual Funhouse_**
- **Arcadia Nexus**
- **Pixel Playground**

## Development

### Database setup

```shell
createuser --pwprompt webgames
createdb --owner webgames webgames
```

### Secrets

```shell
# Database (if default credentials changed)
dotnet user-secrets set "ConnectionStrings:WebGames" "📈" --project WebGames

# SMTP
dotnet user-secrets set "Smtp:Host" "📧" --project WebGames
dotnet user-secrets set "Smtp:Username" "🧍" --project WebGames
dotnet user-secrets set "Smtp:Password" "🔑" --project WebGames
```
