using Dapper;
using ManagementWebApp.Models;
using Microsoft.Data.SqlClient;

namespace ManagementWebApp.Services
{
    public interface IAccountTypesRepository
    {
        Task Create(AccountTypes accountType);
        Task Delete(int id, int userId);
        Task Edit(AccountTypes accountTypes);
        Task<IEnumerable<AccountTypes>> GetAll(int userId);
        Task<AccountTypes> GetById(int id, int userId);
        Task Order(IEnumerable<AccountTypes> accountTypes);
        Task<bool> VerifyExistance(AccountTypes accountType);
    }
    public class AccountTypesRepository : IAccountTypesRepository
    {
        private readonly string connecctionString;

        public AccountTypesRepository(IConfiguration configuration) { 
            connecctionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task Create(AccountTypes accountType)
        {
            using var connection = new SqlConnection(connecctionString);
            var id = await connection.QuerySingleAsync<int>("SP_INSERT_AccountType",
                new {userId = accountType.UserId, name = accountType.Name},
                commandType: System.Data.CommandType.StoredProcedure);
        }

        public async Task<bool> VerifyExistance(AccountTypes accountType)
        {
            using var connection = new SqlConnection(connecctionString);
            var exists = await connection.QuerySingleOrDefaultAsync<int>("SELECT 1 FROM AccountTypes WHERE Name = @Name AND UserId = @UserId", accountType);
            return exists == 1;
        }

        public async Task<IEnumerable<AccountTypes>> GetAll(int userId)
        {
            using var connection = new SqlConnection(connecctionString);
            var accountTypes = await connection.QueryAsync<AccountTypes>("SELECT * FROM AccountTypes WHERE UserId = @UserId ORDER BY OrderNum", new { UserId = userId });
            return accountTypes;
        }

        public async Task<AccountTypes> GetById(int id, int userId) 
        {
            using var connection = new SqlConnection(connecctionString);
            return await connection.QuerySingleOrDefaultAsync<AccountTypes>("SELECT * FROM AccountTypes WHERE Id = @Id AND UserId = @userId", new { id, userId});
        }

        public async Task Edit(AccountTypes accountType)
        {
            using var connection = new SqlConnection(connecctionString);
            await connection.ExecuteAsync("UPDATE AccountTypes SET Name = @Name, UserId = @UserId, OrderNum = @OrderNum WHERE Id = @Id;", accountType);
        }


        public async Task Delete(int id, int userId)
        {
            using var connection = new SqlConnection(connecctionString);
            await connection.ExecuteAsync("DELETE FROM AccountTypes WHERE Id = @Id AND UserId = @UserId", new { id, userId });
        }

        public async Task Order(IEnumerable<AccountTypes> accountTypes)
        {
            var query = "UPDATE AccountTypes SET OrderNum = @OrderNum WHERE Id = @Id";
            using var connection = new SqlConnection(connecctionString);
            await connection.ExecuteAsync(query, accountTypes);
        }
    }
}
