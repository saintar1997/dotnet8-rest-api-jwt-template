using Dotnet8RestApiJwtTemplate.Api.Models.AuthenModel;

namespace Dotnet8RestApiJwtTemplate.Api.Services.AuthService;

public interface IAuthenService
{
    AuthenResponse Generate(string username, string nickname);
}
