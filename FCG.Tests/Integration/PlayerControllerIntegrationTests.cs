using Bogus;
using FCG.Application.Dto.Filter;
using FCG.Application.Dto.Order;
using FCG.Application.Dto.Request;
using FCG.Application.Dto.Response;
using FCG.Application.Dto.Result;
using FCG.Tests.Integration.Base;
using System.Net.Http.Json;

namespace FCG.Tests.Integration;

/// <summary>
/// Testes de integração para o PlayerController
/// Testa todos os endpoints acessíveis por jogadores autenticados
/// </summary>
public class PlayerControllerIntegrationTests : ApiIntegrationTestBase
{
    private readonly Faker _faker;

    public PlayerControllerIntegrationTests()
    {
        _faker = new Faker("pt_BR");
    }

    #region Helper Methods

    /// <summary>
    /// Registra e autentica um novo jogador, retornando o token JWT
    /// </summary>
    private async Task<string> CreateAuthenticatedPlayerAsync()
    {
        var password = "Test@1234";
        var registerDto = new UserCreateRequestDto
        {
            FirstName = _faker.Name.FirstName(),
            LastName = _faker.Name.LastName(),
            DisplayName = _faker.Internet.UserName(),
            Email = _faker.Internet.Email(),
            Password = password,
            ConfirmPassword = password,
            Birthday = _faker.Date.Past(30, DateTime.Now.AddYears(-18))
        };

        var response = await ApiClient.PostAsJsonAsync("/api/auth/register-player", registerDto,
            cancellationToken: TestContext.Current.CancellationToken);

        var authResult = await ReadServiceResultAsync<UserAuthResponseDto>(response);
        Assert.NotNull(authResult);
        Assert.NotEmpty(authResult.Token);

        return authResult.Token;
    }

    #endregion

    #region Update Password Tests

    [Fact]
    public async Task UpdatePlayerPassword_DeveAtualizarSenhaComSucesso()
    {
        // Arrange - Cria e autentica jogador
        var oldPassword = "Test@1234";
        var newPassword = "NewTest@5678";
        var registerDto = new UserCreateRequestDto
        {
            FirstName = _faker.Name.FirstName(),
            LastName = _faker.Name.LastName(),
            DisplayName = _faker.Internet.UserName(),
            Email = _faker.Internet.Email(),
            Password = oldPassword,
            ConfirmPassword = oldPassword,
            Birthday = _faker.Date.Past(30, DateTime.Now.AddYears(-18))
        };

        var registerResponse = await ApiClient.PostAsJsonAsync("/api/auth/register-player", registerDto,
            cancellationToken: TestContext.Current.CancellationToken);
        var authResult = await ReadServiceResultAsync<UserAuthResponseDto>(registerResponse);

        SetAuthorizationHeader(authResult.Token);

        var updateDto = new UserUpdateRequestDto
        {
            FirstName = registerDto.FirstName,
            LastName = registerDto.LastName,
            DisplayName = registerDto.DisplayName,
            Email = registerDto.Email,
            Birthday = registerDto.Birthday,
            Password = oldPassword,
            NewPassword = newPassword,
            ConfirmNewPassword = newPassword
        };

        // Act
        var response = await ApiClient.PutAsJsonAsync("/api/player/password", updateDto,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        // Verifica se pode fazer login com nova senha
        ClearAuthorizationHeader();
        var loginDto = new UserLoginRequestDto
        {
            Email = registerDto.Email,
            Password = newPassword
        };
        var loginResponse = await ApiClient.PostAsJsonAsync("/api/auth/login", loginDto,
            cancellationToken: TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);
    }

    [Fact]
    public async Task UpdatePlayerPassword_DeveRetornarBadRequest_QuandoSenhaAtualIncorreta()
    {
        // Arrange
        var token = await CreateAuthenticatedPlayerAsync();
        SetAuthorizationHeader(token);

        var updateDto = new UserUpdateRequestDto
        {
            FirstName = _faker.Name.FirstName(),
            LastName = _faker.Name.LastName(),
            DisplayName = _faker.Internet.UserName(),
            Email = _faker.Internet.Email(),
            Birthday = _faker.Date.Past(30, DateTime.Now.AddYears(-18)),
            Password = "WrongPassword@123",
            NewPassword = "NewTest@5678",
            ConfirmNewPassword = "NewTest@5678"
        };

        // Act
        var response = await ApiClient.PutAsJsonAsync("/api/player/password", updateDto,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdatePlayerPassword_DeveRetornarBadRequest_QuandoNovasSenhasNaoConferem()
    {
        // Arrange
        var token = await CreateAuthenticatedPlayerAsync();
        SetAuthorizationHeader(token);

        var updateDto = new UserUpdateRequestDto
        {
            FirstName = _faker.Name.FirstName(),
            LastName = _faker.Name.LastName(),
            DisplayName = _faker.Internet.UserName(),
            Email = _faker.Internet.Email(),
            Birthday = _faker.Date.Past(30, DateTime.Now.AddYears(-18)),
            Password = "Test@1234",
            NewPassword = "NewTest@5678",
            ConfirmNewPassword = "DifferentPassword@5678"
        };

        // Act
        var response = await ApiClient.PutAsJsonAsync("/api/player/password", updateDto,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdatePlayerPassword_DeveRetornarUnauthorized_QuandoSemAutenticacao()
    {
        // Arrange
        ClearAuthorizationHeader();

        var updateDto = new UserUpdateRequestDto
        {
            FirstName = _faker.Name.FirstName(),
            LastName = _faker.Name.LastName(),
            DisplayName = _faker.Internet.UserName(),
            Email = _faker.Internet.Email(),
            Birthday = _faker.Date.Past(30, DateTime.Now.AddYears(-18)),
            Password = "Test@1234",
            NewPassword = "NewTest@5678",
            ConfirmNewPassword = "NewTest@5678"
        };

        // Act
        var response = await ApiClient.PutAsJsonAsync("/api/player/password", updateDto,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    #endregion

    #region Get Available Games Tests

    [Fact]
    public async Task GetAvailableGames_DeveRetornarJogosDisponiveis()
    {
        // Arrange
        var token = await CreateAuthenticatedPlayerAsync();
        SetAuthorizationHeader(token);

        var request = new PagedRequestDto<GameFilterDto, GameOrderDto>
        {
            PageNumber = 1,
            PageSize = 10
        };

        // Act
        var response = await ApiClient.GetAsync(
            $"/api/player/available-games?PageNumber={request.PageNumber}&PageSize={request.PageSize}",
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await ReadServiceResultAsync<PagedResult<GalleryGameResponseDto>>(response);
        Assert.NotNull(result);
        Assert.True(result.PageNumber > 0);
        Assert.True(result.PageSize > 0);
    }

    [Fact]
    public async Task GetAvailableGames_DeveRetornarUnauthorized_QuandoSemAutenticacao()
    {
        // Arrange
        ClearAuthorizationHeader();

        // Act
        var response = await ApiClient.GetAsync(
            "/api/player/available-games?PageNumber=1&PageSize=10",
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetAvailableGames_DeveFiltrarPorNome()
    {
        // Arrange
        var token = await CreateAuthenticatedPlayerAsync();
        SetAuthorizationHeader(token);

        var gameName = "Cyber Rebellion";

        // Act
        var response = await ApiClient.GetAsync(
            $"/api/player/available-games?PageNumber=1&PageSize=10&Filter.Name={gameName}",
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await ReadServiceResultAsync<PagedResult<GalleryGameResponseDto>>(response);
        Assert.NotNull(result);
    }

    [Fact]
    public async Task GetAvailableGames_DeveFiltrarPorGenero()
    {
        // Arrange
        var token = await CreateAuthenticatedPlayerAsync();
        SetAuthorizationHeader(token);

        var genre = "Action";

        // Act
        var response = await ApiClient.GetAsync(
            $"/api/player/available-games?PageNumber=1&PageSize=10&Filter.Genre={genre}",
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await ReadServiceResultAsync<PagedResult<GalleryGameResponseDto>>(response);
        Assert.NotNull(result);
    }

    [Fact]
    public async Task GetAvailableGames_DeveFiltrarPorFaixaDePreco()
    {
        // Arrange
        var token = await CreateAuthenticatedPlayerAsync();
        SetAuthorizationHeader(token);

        // Act
        var response = await ApiClient.GetAsync(
            "/api/player/available-games?PageNumber=1&PageSize=10&Filter.MinPrice=10&Filter.MaxPrice=50",
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await ReadServiceResultAsync<PagedResult<GalleryGameResponseDto>>(response);
        Assert.NotNull(result);
    }

    [Fact]
    public async Task GetAvailableGames_DeveOrdenarPorNome()
    {
        // Arrange
        var token = await CreateAuthenticatedPlayerAsync();
        SetAuthorizationHeader(token);

        // Act
        var response = await ApiClient.GetAsync(
            "/api/player/available-games?PageNumber=1&PageSize=10&OrderBy.OrderBy=Name&OrderBy.Ascending=true",
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await ReadServiceResultAsync<PagedResult<GalleryGameResponseDto>>(response);
        Assert.NotNull(result);
    }

    [Fact]
    public async Task GetAvailableGames_DeveRetornarBadRequest_QuandoPageNumberInvalido()
    {
        // Arrange
        var token = await CreateAuthenticatedPlayerAsync();
        SetAuthorizationHeader(token);

        // Act
        var response = await ApiClient.GetAsync(
            "/api/player/available-games?PageNumber=0&PageSize=10",
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    #endregion

    #region Get Cart Tests

    [Fact]
    public async Task GetCartItems_DeveRetornarCarrinhoVazio()
    {
        // Arrange
        var token = await CreateAuthenticatedPlayerAsync();
        SetAuthorizationHeader(token);

        // Act
        var response = await ApiClient.GetAsync("/api/player/cart",
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var cart = await ReadServiceResultAsync<CartResponseDto>(response);
        Assert.NotNull(cart);
        Assert.NotNull(cart.Items);
    }

    [Fact]
    public async Task GetCartItems_DeveRetornarUnauthorized_QuandoSemAutenticacao()
    {
        // Arrange
        ClearAuthorizationHeader();

        // Act
        var response = await ApiClient.GetAsync("/api/player/cart",
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    #endregion

    #region Get Library Tests

    [Fact]
    public async Task GetLibrary_DeveRetornarBibliotecaDoJogador()
    {
        // Arrange
        var token = await CreateAuthenticatedPlayerAsync();
        SetAuthorizationHeader(token);

        var request = new PagedRequestDto<GameFilterDto, GameOrderDto>
        {
            PageNumber = 1,
            PageSize = 10
        };

        // Act
        var response = await ApiClient.GetAsync(
            $"/api/player/library?PageNumber={request.PageNumber}&PageSize={request.PageSize}",
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await ReadServiceResultAsync<PagedResult<LibraryGameResponseDto>>(response);
        Assert.NotNull(result);
        Assert.True(result.PageNumber > 0);
        Assert.True(result.PageSize > 0);
    }

    [Fact]
    public async Task GetLibrary_DeveRetornarUnauthorized_QuandoSemAutenticacao()
    {
        // Arrange
        ClearAuthorizationHeader();

        // Act
        var response = await ApiClient.GetAsync(
            "/api/player/library?PageNumber=1&PageSize=10",
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetLibrary_DeveFiltrarPorNome()
    {
        // Arrange
        var token = await CreateAuthenticatedPlayerAsync();
        SetAuthorizationHeader(token);

        var gameName = "Cyber Rebellion";

        // Act
        var response = await ApiClient.GetAsync(
            $"/api/player/library?PageNumber=1&PageSize=10&Filter.Name={gameName}",
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await ReadServiceResultAsync<PagedResult<LibraryGameResponseDto>>(response);
        Assert.NotNull(result);
    }

    [Fact]
    public async Task GetLibrary_DeveOrdenarPorNome()
    {
        // Arrange
        var token = await CreateAuthenticatedPlayerAsync();
        SetAuthorizationHeader(token);

        // Act
        var response = await ApiClient.GetAsync(
            "/api/player/library?PageNumber=1&PageSize=10&OrderBy.OrderBy=Name&OrderBy.Ascending=true",
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await ReadServiceResultAsync<PagedResult<LibraryGameResponseDto>>(response);
        Assert.NotNull(result);
    }

    [Fact]
    public async Task GetLibrary_DeveRetornarBadRequest_QuandoPageNumberInvalido()
    {
        // Arrange
        var token = await CreateAuthenticatedPlayerAsync();
        SetAuthorizationHeader(token);

        // Act
        var response = await ApiClient.GetAsync(
            "/api/player/library?PageNumber=0&PageSize=10",
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    #endregion

    #region Integration Flow Tests

    [Fact]
    public async Task FluxoCompleto_ConsultarJogosCarrinhoBiblioteca_DeveFuncionarCorretamente()
    {
        // Arrange
        var token = await CreateAuthenticatedPlayerAsync();
        SetAuthorizationHeader(token);

        // 1. Consulta jogos disponíveis
        var gamesResponse = await ApiClient.GetAsync(
            "/api/player/available-games?PageNumber=1&PageSize=10",
            cancellationToken: TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.OK, gamesResponse.StatusCode);

        var games = await ReadServiceResultAsync<PagedResult<GalleryGameResponseDto>>(gamesResponse);
        Assert.NotNull(games);

        // 2. Consulta carrinho
        var cartResponse = await ApiClient.GetAsync("/api/player/cart",
            cancellationToken: TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.OK, cartResponse.StatusCode);

        var cart = await ReadServiceResultAsync<CartResponseDto>(cartResponse);
        Assert.NotNull(cart);

        // 3. Consulta biblioteca
        var libraryResponse = await ApiClient.GetAsync(
            "/api/player/library?PageNumber=1&PageSize=10",
            cancellationToken: TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.OK, libraryResponse.StatusCode);

        var library = await ReadServiceResultAsync<PagedResult<LibraryGameResponseDto>>(libraryResponse);
        Assert.NotNull(library);
    }

    [Fact]
    public async Task FluxoCompleto_AtualizarSenhaEConsultarDados_DeveFuncionarCorretamente()
    {
        // Arrange
        var oldPassword = "Test@1234";
        var newPassword = "NewTest@5678";
        var registerDto = new UserCreateRequestDto
        {
            FirstName = _faker.Name.FirstName(),
            LastName = _faker.Name.LastName(),
            DisplayName = _faker.Internet.UserName(),
            Email = _faker.Internet.Email(),
            Password = oldPassword,
            ConfirmPassword = oldPassword,
            Birthday = _faker.Date.Past(30, DateTime.Now.AddYears(-18))
        };

        // 1. Registra jogador
        var registerResponse = await ApiClient.PostAsJsonAsync("/api/auth/register-player", registerDto,
            cancellationToken: TestContext.Current.CancellationToken);
        var authResult = await ReadServiceResultAsync<UserAuthResponseDto>(registerResponse);

        SetAuthorizationHeader(authResult.Token);

        // 2. Atualiza senha
        var updateDto = new UserUpdateRequestDto
        {
            FirstName = registerDto.FirstName,
            LastName = registerDto.LastName,
            DisplayName = registerDto.DisplayName,
            Email = registerDto.Email,
            Birthday = registerDto.Birthday,
            Password = oldPassword,
            NewPassword = newPassword,
            ConfirmNewPassword = newPassword
        };

        var updateResponse = await ApiClient.PutAsJsonAsync("/api/player/password", updateDto,
            cancellationToken: TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.NoContent, updateResponse.StatusCode);

        // 3. Faz login com nova senha
        ClearAuthorizationHeader();
        var loginDto = new UserLoginRequestDto
        {
            Email = registerDto.Email,
            Password = newPassword
        };
        var loginResponse = await ApiClient.PostAsJsonAsync("/api/auth/login", loginDto,
            cancellationToken: TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);

        var newAuthResult = await ReadServiceResultAsync<UserAuthResponseDto>(loginResponse);
        SetAuthorizationHeader(newAuthResult.Token);

        // 4. Verifica acesso aos dados do jogador
        var libraryResponse = await ApiClient.GetAsync(
            "/api/player/library?PageNumber=1&PageSize=10",
            cancellationToken: TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.OK, libraryResponse.StatusCode);
    }

    #endregion
}
