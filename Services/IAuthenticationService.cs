using System;

namespace ForVlad.Services
{
    public interface IAuthenticationService
    {
        bool Authenticate(string username, string password);
    }
}
