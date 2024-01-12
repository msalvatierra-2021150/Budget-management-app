using Dapper;
using ManagementWebApp.Models;
using Microsoft.Data.SqlClient;

namespace ManagementWebApp.Services
{
    public interface ICategoryRepository
    {
        Task Create(Category category);
        Task Delete(int id, int userId);
        Task EditCategory(Category category);
        Task<IEnumerable<Category>> Get(int userId);
        Task<Category> getById(int id, int userId);
    }
    public class CategoryRepository : ICategoryRepository
    {
        private readonly string connectionString;

        public CategoryRepository(IConfiguration configuration) {
            this.connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task Create (Category category)
        {
            using var connection = new SqlConnection(connectionString);
            var id = await connection.QueryFirstAsync<int>("INSERT INTO Category (Name, TransactionTypeId, UserId)" +
                "VALUES(@Name, @TransactionTypeId, @UserId);" +
                "SELECT SCOPE_IDENTITY();", category);
            category.Id = id;
        }

        public async Task<IEnumerable<Category>> Get(int userId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<Category>("SELECT * FROM Category " +
                "WHERE UserId = @userId", new {UserId = userId });
        }

        public async Task<Category> getById(int id, int userId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryFirstOrDefaultAsync<Category>("SELECT * FROM Category " +
                "WHERE Id = @id AND @UserId = userId", new {id, userId });
        }

        public async Task EditCategory(Category category)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync("UPDATE Category " +
                "SET Name = @Name, TransactionTypeId = @TransactionTypeId " +
                "WHERE Id = @Id", category);
        }

        public async Task Delete(int id, int userId)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync("DELETE FROM Category WHERE Id = @Id AND UserId = @UserId", new { id, userId });
        }
    }
}
