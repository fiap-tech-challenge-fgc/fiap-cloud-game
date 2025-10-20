using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace FCG.Api.Data
{
    /// <summary>
    /// Fornece uma camada de acesso a dados (DAL) genérica para operações CRUD e de consulta usando o Entity Framework Core.
    /// </summary>
    /// <typeparam name="T">O tipo da entidade para a qual este DAL opera. Deve ser uma classe.</typeparam>
    public class DAL<T> where T : class
    {
        protected readonly DbContext context;

        /// <summary>
        /// Inicializa uma nova instância da classe DAL.
        /// </summary>
        /// <param name="context">A instância do DbContext a ser usada para operações de banco de dados.</param>
        public DAL(DbContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Verifica se existe pelo menos uma entidade que satisfaz a condição especificada.
        /// </summary>
        /// <param name="condicao">A expressão de condição para filtrar as entidades.</param>
        /// <returns><c>true</c> se um item existir; caso contrário, <c>false</c>.</returns>
        public bool Exists(Expression<Func<T, bool>> condicao)
        {
            return context.Set<T>().Any(condicao);
        }

        /// <summary>
        /// Verifica se uma entidade com a chave primária especificada existe.
        /// </summary>
        /// <param name="chavePrimaria">O valor da chave primária a ser procurado.</param>
        /// <returns><c>true</c> se o item for encontrado; caso contrário, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">Lançada se <paramref name="chavePrimaria"/> for nula.</exception>
        public bool Exists(object chavePrimaria)
        {
            if (chavePrimaria == null)
                throw new ArgumentNullException(nameof(chavePrimaria));

            var set = context.Set<T>();
            return set.Find(chavePrimaria) != null;
        }

        /// <summary>
        /// Retorna todas as entidades do tipo <typeparamref name="T"/> do banco de dados de forma síncrona.
        /// </summary>
        /// <returns>Um <see cref="IEnumerable{T}"/> contendo todas as entidades.</returns>
        public IEnumerable<T> List()
        {
            return context.Set<T>().ToList();
        }

        /// <summary>
        /// Retorna de forma assíncrona todas as entidades do tipo <typeparamref name="T"/> do banco de dados.
        /// </summary>
        /// <returns>Uma <see cref="Task"/> que representa a operação assíncrona. O resultado da tarefa contém uma <see cref="List{T}"/> com todas as entidades.</returns>
        public async Task<List<T>> ListAsync()
        {
            return await context.Set<T>().ToListAsync();
        }

        /// <summary>
        /// Retorna de forma assíncrona todas as entidades, incluindo propriedades de navegação especificadas (Eager Loading).
        /// </summary>
        /// <param name="includes">Uma lista de expressões lambda para especificar as propriedades de navegação a serem incluídas na consulta.</param>
        /// <returns>Uma <see cref="Task"/> que representa a operação assíncrona. O resultado da tarefa contém uma <see cref="List{T}"/> com as entidades e suas propriedades relacionadas carregadas.</returns>
        public async Task<List<T>> ListAsync(params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = context.Set<T>();
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
            return await query.ToListAsync();
        }

        /// <summary>
        /// Adiciona uma nova entidade ao banco de dados de forma síncrona.
        /// </summary>
        /// <param name="item">A entidade a ser adicionada.</param>
        public void Add(T item)
        {
            context.Set<T>().Add(item);
            context.SaveChanges();
        }

        /// <summary>
        /// Adiciona uma nova entidade ao banco de dados de forma assíncrona.
        /// </summary>
        /// <param name="item">A entidade a ser adicionada.</param>
        public async Task AddAsync(T item)
        {
            await context.Set<T>().AddAsync(item);
            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                var inner = ex.InnerException;
                while (inner != null)
                {
                    Console.WriteLine(inner.Message);
                    inner = inner.InnerException;
                }
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Atualiza uma entidade existente no banco de dados de forma síncrona.
        /// </summary>
        /// <param name="item">A entidade com os valores atualizados.</param>
        public void Update(T item)
        {
            context.Entry(item).State = EntityState.Modified;
            context.SaveChanges();
        }

        /// <summary>
        /// Atualiza uma entidade existente no banco de dados de forma assíncrona.
        /// </summary>
        /// <param name="item">A entidade com os valores atualizados.</param>
        public async Task UpdateAsync(T item)
        {
            context.Entry(item).State = EntityState.Modified;
            await context.SaveChangesAsync();
        }

        /// <summary>
        /// Remove uma entidade do banco de dados de forma síncrona.
        /// </summary>
        /// <param name="item">A entidade a ser removida.</param>
        public void Delete(T item)
        {
            context.Set<T>().Remove(item);
            context.SaveChanges();
        }

        /// <summary>
        /// Remove uma entidade do banco de dados de forma assíncrona.
        /// </summary>
        /// <param name="item">A entidade a ser removida.</param>
        public async Task DeleteAsync(T item)
        {
            context.Set<T>().Remove(item);
            await context.SaveChangesAsync();
        }

        /// <summary>
        /// Encontra a primeira entidade que satisfaz a condição ou retorna nulo se nenhuma for encontrada.
        /// </summary>
        /// <param name="condicao">A expressão de condição para a busca.</param>
        /// <returns>A primeira entidade encontrada ou <c>null</c>.</returns>
        public T? Find(Expression<Func<T, bool>> condicao)
        {
            return context.Set<T>().FirstOrDefault(condicao);
        }

        /// <summary>
        /// Encontra todas as entidades que satisfazem uma condição.
        /// </summary>
        /// <param name="condicao">A expressão de condição para filtrar as entidades.</param>
        /// <returns>Um <see cref="IEnumerable{T}"/> com as entidades encontradas.</returns>
        public IEnumerable<T>? FindList(Expression<Func<T, bool>> condicao)
        {
            return context.Set<T>().Where(condicao).ToList();
        }

        /// <summary>
        /// Encontra de forma assíncrona a primeira entidade que satisfaz a condição, incluindo propriedades de navegação (Eager Loading).
        /// </summary>
        /// <param name="condicao">A expressão de condição para a busca.</param>
        /// <param name="includes">Expressões para incluir propriedades de navegação.</param>
        /// <returns>Uma <see cref="Task"/> que representa a operação. O resultado contém a entidade encontrada ou <c>null</c>.</returns>
        public async Task<T?> FindAsync(Expression<Func<T, bool>> condicao, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = context.Set<T>().Where(condicao);
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
            return await query.FirstOrDefaultAsync();
        }

        /// <summary>
        /// Encontra de forma assíncrona uma lista de entidades que satisfazem a condição, incluindo propriedades de navegação (Eager Loading).
        /// </summary>
        /// <param name="condicao">A expressão de condição para filtrar as entidades.</param>
        /// <param name="includes">Expressões para incluir propriedades de navegação.</param>
        /// <returns>Uma <see cref="Task"/> que representa a operação. O resultado contém uma <see cref="List{T}"/> de entidades encontradas.</returns>
        public async Task<List<T>> FindListAsync(Expression<Func<T, bool>> condicao, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = context.Set<T>().Where(condicao);
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
            return await query.ToListAsync();
        }

        /// <summary>
        /// Carrega explicitamente uma propriedade de navegação de referência para uma entidade já carregada (carregamento explícito).
        /// </summary>
        /// <param name="item">A entidade para a qual a propriedade será carregada.</param>
        /// <param name="propriedade">A expressão que especifica a propriedade de navegação a ser carregada.</param>
        public void LoadProperty(T item, Expression<Func<T, object>> propriedade)
        {
            context.Entry(item).Reference(propriedade!).Load();
        }

        /// <summary>
        /// Carrega explicitamente e de forma assíncrona uma propriedade de navegação de referência para uma entidade já carregada.
        /// </summary>
        /// <param name="item">A entidade para a qual a propriedade será carregada.</param>
        /// <param name="propriedade">A expressão que especifica a propriedade de navegação a ser carregada.</param>
        public async Task LoadPropertyAsync(T item, Expression<Func<T, object>> propriedade)
        {
            await context.Entry(item).Reference(propriedade!).LoadAsync();
        }

        /// <summary>
        /// Carrega explicitamente uma propriedade de navegação de referência para cada entidade em uma coleção.
        /// </summary>
        /// <param name="itens">A coleção de entidades.</param>
        /// <param name="propriedade">A expressão que especifica a propriedade de navegação a ser carregada.</param>
        public void LoadPropertyInList(IEnumerable<T> itens, Expression<Func<T, object>> propriedade)
        {
            foreach (var item in itens)
            {
                context.Entry(item).Reference(propriedade!).Load();
            }
        }

        /// <summary>
        /// Carrega explicitamente e de forma assíncrona uma propriedade de navegação de referência para cada entidade em uma coleção.
        /// </summary>
        /// <param name="itens">A coleção de entidades.</param>
        /// <param name="propriedade">A expressão que especifica a propriedade de navegação a ser carregada.</param>
        public async Task LoadPropertyInListAsync(IEnumerable<T> itens, Expression<Func<T, object>> propriedade)
        {
            foreach (var item in itens)
            {
                await context.Entry(item).Reference(propriedade!).LoadAsync();
            }
        }
    }
}