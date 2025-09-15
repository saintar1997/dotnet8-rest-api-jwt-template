$folders = @(
  "Dotnet8RestApiJwtTemplate.Api/Attributes",
  "Dotnet8RestApiJwtTemplate.Api/Clients",
  "Dotnet8RestApiJwtTemplate.Api/Configs",
  "Dotnet8RestApiJwtTemplate.Api/Constants",
  "Dotnet8RestApiJwtTemplate.Api/Controllers",
  "Dotnet8RestApiJwtTemplate.Api/DTOs",
  "Dotnet8RestApiJwtTemplate.Api/Enums",
  "Dotnet8RestApiJwtTemplate.Api/Models",
  "Dotnet8RestApiJwtTemplate.Api/Repositories",
  "Dotnet8RestApiJwtTemplate.Api/Services",
  "Dotnet8RestApiJwtTemplate.Api/Utilities",
  "Dotnet8RestApiJwtTemplate.Test/UnitTests"
)

foreach ($folder in $folders) {
  New-Item -ItemType Directory -Force -Path $folder | Out-Null
  New-Item -ItemType File -Force -Path "$folder/.gitkeep" | Out-Null
}