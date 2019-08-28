using Cafeine.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cafeine.Shared.Services
{
    public interface IAuthService
    {
        void DeleteAccount(UserAccountModel account);

        Task<UserAccountModel> CreateAccount(bool isDefaultUser);

        Task VerifyAccount();

        Task VerifyAccount(string token);

        Task VerifyAccount(UserAccountModel account);
    }
}
