using Dapper;
using ManagementWebApp.Models;
using Microsoft.Data.SqlClient;

namespace ManagementWebApp.Services
{
    public interface IAccountRepository
    {
        Task Create(Accounts account);
        Task Delete(int id);
        Task Edit(Accounts account);
        Task<IEnumerable<Accounts>> GetAll(int userId);
        Task<Accounts> GetById(int id, int userId);
    }
    public class AccountsRepository: IAccountRepository
    {
        private string connectionString;

        public AccountsRepository(IConfiguration configuration) {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }
        public async Task Create(Accounts account)
        {
            using var  connection = new SqlConnection(connectionString);
            var id = await connection.QueryFirstAsync<int>(
                "INSERT INTO Accounts (Name, AccountTypeId, Balance, Description) " +
                "VALUES (@Name, @AccountTypeId, @Balance, @Description);" + 
                "SELECT SCOPE_IDENTITY();", account);

            account.Id = id;
        }

        public async Task<IEnumerable<Accounts>> GetAll(int userId)
        {
            using var connection = new SqlConnection(connectionString);
            var accounts = await connection.QueryAsync<Accounts>(
                "SELECT A.Id, A.Name, A.Balance, A.AccountTypeId, AT.Name AS AccountType " +
                "FROM Accounts " +
                "AS A INNER JOIN AccountTypes AS AT " +
                "ON A.AccountTypeId = AT.Id " +
                "WHERE AT.UserId = 1 " +
                "ORDER BY AT.OrderNum;");
            return accounts;
        }

        public async Task<Accounts> GetById(int id, int userId)
        {
            using var connection = new SqlConnection(connectionString);
            var account = await connection.QuerySingleAsync<Accounts>(
                "SELECT A.Id, A.Name, A.Balance, A.Description, A.AccountTypeId, A.AccountTypeId " +
                "FROM Accounts " +
                "AS A INNER JOIN AccountTypes AS AT " +
                "ON A.AccountTypeId = AT.Id " +
                "WHERE AT.UserId = 1 AND A.Id = @Id",
                new {id, userId});
            return account;
        }

        public async Task Edit(Accounts account)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync("UPDATE Accounts " +
                "SET Name = @Name, " +
                "AccountTypeId = @AccountTypeId, " +
                "Balance = @Balance, " +
                "Description = @Description " +
                "WHERE Id = @Id;", 
                account);
        }

        public async Task Delete(int id)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync("DELETE FROM Accounts " +
                "WHERE Id = @Id", new { id });
        }
    }
}
