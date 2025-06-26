using System.Data.Entity;
using minimalapi.domain.db;
using minimalapi.domain.dto;
using minimalapi.domain.entity;
using minimalapi.domain.interfaces;

namespace minimalapi.domain.services;

public class AdminService : IAdminService
{
    private readonly AppDbContext _db;
    public AdminService(AppDbContext dbContext)
    {
        _db = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public Admin Authenticate(LoginDTO login)
    {
        if (login == null)
        {
            throw new ArgumentNullException(nameof(login));
        }

        if (string.IsNullOrEmpty(login.Username) || string.IsNullOrEmpty(login.Password))
        {
            throw new ArgumentException("Username and password must be provided.");
        }

        var admin = _db.Admins.Where(a => a.Name == login.Username && a.Password == login.Password).FirstOrDefault();

        if (admin == null)
        {
            throw new UnauthorizedAccessException("Invalid username or password.");
        }

        return admin;
    }
}