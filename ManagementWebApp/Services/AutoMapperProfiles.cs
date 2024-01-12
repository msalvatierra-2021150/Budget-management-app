using AutoMapper;
using ManagementWebApp.Models;

namespace ManagementWebApp.Services
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles() {
            CreateMap<Accounts, AccountsCreationViewModel>();
        }
    }
}
