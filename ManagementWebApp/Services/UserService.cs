namespace ManagementWebApp.Services
{
    public interface IUserService
    {
        int getUserId();
    }
    public class UserService : IUserService
    {
        public int getUserId()
        {
            return 1;
        }
    }
}
