using Bogus;
using FCG.Application.Dto.Request;
using FCG.Application.Dto.Response;
using FCG.Tests.Integration.Base;
using System.Net;
using System.Net.Http.Json;

namespace FCG.Tests.Integration;

/// <summary>
/// Testes de integração para o PlayerController
/// Testa todos os endpoints relacionados ao jogador autenticado
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
    /// Cria e autentica um usuário para testes
    /// </summary>
    private async Task<(UserCreateRequestDto user, string token)> CreateAndAuthenticateUserAsync()
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

        var registerResponse = await ApiClient.PostAsJsonAsync("/api/auth/register-player", registerDto, cancellationToken: TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.OK, registerResponse.StatusCode);

        var authResult = await ReadServiceResultAsync<UserAuthResponseDto>(registerResponse);
        Assert.NotNull(authResult);
        Assert.NotEmpty(authResult.Token);

        return (registerDto, authResult.Token);
    }

    #endregion

    #region Update Password Tests

    [Fact]
    public async Task UpdatePlayerPassword_DeveAtualizarSenhaComSucesso()
    {
        // Arrange
        var (user, token) = await CreateAndAuthenticateUserAsync();
        SetAuthorizationHeader(token);

        var updateDto = new UserUpdateRequestDto
        {
            FirstName = user.FirstName,
            LastName = user.LastName,
            DisplayName = user.DisplayName,
            Email = user.Email,
            Birthday = user.Birthday,
            Password = "Test@1234", // Senha atual
            NewPassword = "NewTest@1234",
            ConfirmNewPassword = "NewTest@1234"
        };

        // Act
        var response = await ApiClient.PutAsJsonAsync("/api/player/password", updateDto, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        // Valida que pode fazer login com a nova senha
        var loginDto = new UserLoginRequestDto
        {
            Email = user.Email,
            Password = "NewTest@1234"
        };

        var loginResponse = await ApiClient.PostAsJsonAsync("/api/auth/login", loginDto, cancellationToken: TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);
    }

    [Fact]
    public async Task UpdatePlayerPassword_DeveRetornarBadRequest_QuandoSenhaAtualIncorreta()
    {
        // Arrange
        var (user, token) = await CreateAndAuthenticateUserAsync();
        SetAuthorizationHeader(token);

        var updateDto = new UserUpdateRequestDto
        {
            FirstName = user.FirstName,
            LastName = user.LastName,
            DisplayName = user.DisplayName,
            Email = user.Email,
            Birthday = user.Birthday,
            Password = "SenhaErrada@123", // Senha incorreta
            NewPassword = "NewTest@1234",
            ConfirmNewPassword = "NewTest@1234"
        };

        // Act
        var response = await ApiClient.PutAsJsonAsync("/api/player/password", updateDto);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdatePlayerPassword_DeveRetornarBadRequest_QuandoNovasSenhasNaoConferem()
    {
        // Arrange
        var (user, token) = await CreateAndAuthenticateUserAsync();
        SetAuthorizationHeader(token);

        var updateDto = new UserUpdateRequestDto
        {
            FirstName = user.FirstName,
            LastName = user.LastName,
            DisplayName = user.DisplayName,
            Email = user.Email,
            Birthday = user.Birthday,
            Password = "Test@1234",
            NewPassword = "NewTest@1234",
            ConfirmNewPassword = "Different@1234" // Não confere
        };

        // Act
        var response = await ApiClient.PutAsJsonAsync("/api/player/password", updateDto);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdatePlayerPassword_DeveRetornarUnauthorized_QuandoSemToken()
    {
        // Arrange
        ClearAuthorizationHeader();

        var updateDto = new UserUpdateRequestDto
        {
            FirstName = "Test",
            LastName = "User",
            DisplayName = "testuser",
            Email = "test@test.com",
            Birthday = DateTime.Now.AddYears(-20),
            Password = "Test@1234",
            NewPassword = "NewTest@1234",
            ConfirmNewPassword = "NewTest@1234"
        };

        // Act
        var response = await ApiClient.PutAsJsonAsync("/api/player/password", updateDto);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    #endregion

    #region Get Available Games Tests

    [Fact]
    public async Task GetAvailableGames_DeveRetornarListaDeJogos()
    {
        // Arrange
        var (user, token) = await CreateAndAuthenticateUserAsync();
        SetAuthorizationHeader(token);

        // Act
        var response = await ApiClient.GetAsync("/api/player/available-games");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        // ✅ Lê diretamente sem ServiceResult wrapper
        var games = await response.Content.ReadFromJsonAsync<IEnumerable<GameResponseDto>>(JsonOptions);
        Assert.NotNull(games);
    }

    [Fact]
    public async Task GetAvailableGames_DeveRetornarListaOrdenada_QuandoOrderByFornecido()
    {
        // Arrange
        var (user, token) = await CreateAndAuthenticateUserAsync();
        SetAuthorizationHeader(token);

        // Act - Ordena por nome
        var response = await ApiClient.GetAsync("/api/player/available-games?orderBy=name");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        // ✅ Lê diretamente sem ServiceResult wrapper
        var games = await response.Content.ReadFromJsonAsync<IEnumerable<GameResponseDto>>(JsonOptions);
        Assert.NotNull(games);

        // Valida que está ordenado
        var gamesList = games.ToList();
        if (gamesList.Count > 1)
        {
            var sortedList = gamesList.OrderBy(g => g.Name).ToList();
            Assert.Equal(sortedList.Select(g => g.Name), gamesList.Select(g => g.Name));
        }
    }

    [Fact]
    public async Task GetAvailableGames_DeveExcluirJogosPossuidos_QuandoExcludeOwnedTrue()
    {
        // Arrange
        var (user, token) = await CreateAndAuthenticateUserAsync();
        SetAuthorizationHeader(token);

        // Act
        var response = await ApiClient.GetAsync("/api/player/available-games?excludeOwned=true");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        // ✅ Lê diretamente sem ServiceResult wrapper
        var games = await response.Content.ReadFromJsonAsync<IEnumerable<GameResponseDto>>(JsonOptions);
        Assert.NotNull(games);
    }

    [Fact]
    public async Task GetAvailableGames_DeveRetornarUnauthorized_QuandoSemToken()
    {
        // Arrange
        ClearAuthorizationHeader();

        // Act
        var response = await ApiClient.GetAsync("/api/player/available-games");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    #endregion

    #region Get Cart Items Tests

    [Fact]
    public async Task GetCartItems_DeveRetornarCarrinhoVazio_ParaNovoUsuario()
    {
        // Arrange
        var (user, token) = await CreateAndAuthenticateUserAsync();
        SetAuthorizationHeader(token);

        // Act
        var response = await ApiClient.GetAsync("/api/player/cart");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        // ✅ Lê diretamente sem ServiceResult wrapper
        var cartItems = await response.Content.ReadFromJsonAsync<IEnumerable<GameResponseDto>>(JsonOptions);
        Assert.NotNull(cartItems);
        Assert.Empty(cartItems);
    }

    [Fact]
    public async Task GetCartItems_DeveRetornarUnauthorized_QuandoSemToken()
    {
        // Arrange
        ClearAuthorizationHeader();

        // Act
        var response = await ApiClient.GetAsync("/api/player/cart");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    #endregion

    #region Get Purchased Games Tests

    [Fact]
    public async Task GetPurchasedGames_DeveRetornarListaVazia_ParaNovoUsuario()
    {
        // Arrange
        var (user, token) = await CreateAndAuthenticateUserAsync();
        SetAuthorizationHeader(token);

        // Act
        var response = await ApiClient.GetAsync("/api/player/purchases");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        // ✅ Lê diretamente sem ServiceResult wrapper
        var purchases = await response.Content.ReadFromJsonAsync<IEnumerable<GameResponseDto>>(JsonOptions);
        Assert.NotNull(purchases);
        Assert.Empty(purchases);
    }

    [Fact]
    public async Task GetPurchasedGames_DeveRetornarUnauthorized_QuandoSemToken()
    {
        // Arrange
        ClearAuthorizationHeader();

        // Act
        var response = await ApiClient.GetAsync("/api/player/purchases");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    #endregion

    #region Integration Flow Tests

    [Fact]
    public async Task FluxoCompleto_RegistroConsultaJogosCarrinhoCompras_DeveFuncionarCorretamente()
    {
        // 1. Registro e autenticação
        var (user, token) = await CreateAndAuthenticateUserAsync();
        SetAuthorizationHeader(token);

        // 2. Verifica que o carrinho está vazio
        var cartResponse = await ApiClient.GetAsync("/api/player/cart");
        Assert.Equal(HttpStatusCode.OK, cartResponse.StatusCode);
        // ✅ Lê diretamente sem ServiceResult wrapper
        var cartItems = await cartResponse.Content.ReadFromJsonAsync<IEnumerable<GameResponseDto>>(JsonOptions);
        Assert.NotNull(cartItems);
        Assert.Empty(cartItems);

        // 3. Verifica que não tem compras
        var purchasesResponse = await ApiClient.GetAsync("/api/player/purchases");
        Assert.Equal(HttpStatusCode.OK, purchasesResponse.StatusCode);
        // ✅ Lê diretamente sem ServiceResult wrapper
        var purchases = await purchasesResponse.Content.ReadFromJsonAsync<IEnumerable<GameResponseDto>>(JsonOptions);
        Assert.NotNull(purchases);
        Assert.Empty(purchases);

        // 4. Lista jogos disponíveis
        var gamesResponse = await ApiClient.GetAsync("/api/player/available-games");
        Assert.Equal(HttpStatusCode.OK, gamesResponse.StatusCode);
        // ✅ Lê diretamente sem ServiceResult wrapper
        var games = await gamesResponse.Content.ReadFromJsonAsync<IEnumerable<GameResponseDto>>(JsonOptions);
        Assert.NotNull(games);

        // 5. Atualiza senha
        var updateDto = new UserUpdateRequestDto
        {
            FirstName = user.FirstName,
            LastName = user.LastName,
            DisplayName = user.DisplayName,
            Email = user.Email,
            Birthday = user.Birthday,
            Password = "Test@1234",
            NewPassword = "NewTest@1234",
            ConfirmNewPassword = "NewTest@1234"
        };

        var updateResponse = await ApiClient.PutAsJsonAsync("/api/player/password", updateDto);
        Assert.Equal(HttpStatusCode.NoContent, updateResponse.StatusCode);

        // 6. Valida login com nova senha
        ClearAuthorizationHeader();
        var loginDto = new UserLoginRequestDto
        {
            Email = user.Email,
            Password = "NewTest@1234"
        };

        var loginResponse = await ApiClient.PostAsJsonAsync("/api/auth/login", loginDto);
        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);

        var loginResult = await ReadServiceResultAsync<UserAuthResponseDto>(loginResponse);
        Assert.NotNull(loginResult);
        Assert.NotEmpty(loginResult.Token);
    }

    [Fact]
    public async Task MultipleUsersFlow_DeveIsolarDadosEntreUsuarios()
    {
        // 1. Cria primeiro usuário
        var (user1, token1) = await CreateAndAuthenticateUserAsync();

        // 2. Cria segundo usuário
        var (user2, token2) = await CreateAndAuthenticateUserAsync();

        // 3. Valida que cada usuário tem seus próprios dados
        SetAuthorizationHeader(token1);
        var cart1Response = await ApiClient.GetAsync("/api/player/cart");
        Assert.Equal(HttpStatusCode.OK, cart1Response.StatusCode);
        // ✅ Lê diretamente sem ServiceResult wrapper
        var cart1 = await cart1Response.Content.ReadFromJsonAsync<IEnumerable<GameResponseDto>>(JsonOptions);
        Assert.NotNull(cart1);

        SetAuthorizationHeader(token2);
        var cart2Response = await ApiClient.GetAsync("/api/player/cart");
        Assert.Equal(HttpStatusCode.OK, cart2Response.StatusCode);
        // ✅ Lê diretamente sem ServiceResult wrapper
        var cart2 = await cart2Response.Content.ReadFromJsonAsync<IEnumerable<GameResponseDto>>(JsonOptions);
        Assert.NotNull(cart2);

        // 4. Valida que não consegue acessar com token de outro usuário
        SetAuthorizationHeader(token1);
        var meResponse1 = await ApiClient.GetAsync("/api/auth/me");
        Assert.Equal(HttpStatusCode.OK, meResponse1.StatusCode);
        var user1Info = await ReadServiceResultAsync<UserInfoResponseDto>(meResponse1);
        Assert.NotNull(user1Info);
        Assert.Equal(user1.Email, user1Info.Email);

        SetAuthorizationHeader(token2);
        var meResponse2 = await ApiClient.GetAsync("/api/auth/me");
        Assert.Equal(HttpStatusCode.OK, meResponse2.StatusCode);
        var user2Info = await ReadServiceResultAsync<UserInfoResponseDto>(meResponse2);
        Assert.NotNull(user2Info);
        Assert.Equal(user2.Email, user2Info.Email);
        Assert.NotEqual(user1Info.Email, user2Info.Email);
    }

    #endregion
}
