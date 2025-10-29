public interface ISeedService
{
    /// <summary>
    /// Executa o seed apenas dos dados do Identity (roles, admin e players).
    /// Precisa do IServiceProvider para acessar UserManager/RoleManager.
    /// </summary>
    Task SeedIdentityAsync(IServiceProvider sp, CancellationToken cancellationToken);

    /// <summary>
    /// Executa o seed apenas dos dados da aplicação (jogos, players, carrinhos, biblioteca, etc.).
    /// </summary>
    Task SeedApplicationAsync(CancellationToken cancellationToken);
}
