using minimalapi.domain.dto;
using minimalapi.domain.entity;

namespace minimalapi.domain.interfaces;

public interface IAdminService
{
    /// <summary>
    /// Authenticate an admin user.
    /// </summary>
    /// <param name="login">Login credentials.</param>
    /// <returns>Authenticated admin user.</returns>
    Admin Authenticate(LoginDTO login);
    int CreateAdmin(Admin admin);
}