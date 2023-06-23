using CompanyEmployees.IDP.Entities;
using IdentityServer4.ResponseHandling;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System;
using System.Security.Claims;
using IdentityModel;

namespace CompanyEmployees.IDP
{
    public class SeedUserData
    {
        public static void EnsureSeedData(string connectionString)
        {
            var service = new ServiceCollection();
            service.AddLogging();
            service.AddDbContext<UserContext>(opt=>
            opt.UseSqlServer(connectionString));

            service.AddIdentity<User, IdentityRole>(o =>
            {
                o.Password.RequireDigit = false;
                o.Password.RequireNonAlphanumeric = false;
            }).AddEntityFrameworkStores<UserContext>()
            .AddDefaultTokenProviders();

            using(var serviceProvider = service.BuildServiceProvider())
            {
                using(var scope = serviceProvider
                    .GetRequiredService<IServiceScopeFactory>().CreateScope())
                {
                    CreateUser(scope, "John", "Doe", 
                        "John Doe's Boulevard 323", "USA",
                        "97a3aa4a-7a89-47f3-9814-74497fb92ccb", "JohnPassword",
                        "Administrator", "john@mail.com");

                    CreateUser(scope, "Jane", "Doe", 
                        "Jane Doe's Avenue 214", "USA", 
                        "64aca900-7bc7-4645-b291-38f1b7b5963c", "JanePassword", 
                        "Visitor", "jane@mail.com");
                }
            }
        }

        private static void CreateUser(IServiceScope scope, string name, string lastName,
            string address, string country, string id, string password, string role, string email)
        {
            var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
            var user = userMgr.FindByNameAsync(name).Result;

            if(user is null)
            {
                user = new User
                {
                    UserName = name,
                    Email = email,
                    FirstName = name,
                    LastName = lastName,
                    Country = country,
                    Id = id,
                    Address = address
                };

            }
            var result = userMgr.CreateAsync(user, password).Result;
            CheckResult(result);

            result = userMgr.AddToRoleAsync(user, role).Result;
            CheckResult(result);

            result = userMgr.AddClaimsAsync(user, new Claim[]
            {
                new Claim(JwtClaimTypes.GivenName, user.FirstName),
                new Claim(JwtClaimTypes.FamilyName, user.LastName),
                new Claim(JwtClaimTypes.Role, role),
                new Claim(JwtClaimTypes.Address, user.Address),
                new Claim("country", user.Country)
            }).Result;
            CheckResult(result);
        }

        private static void CheckResult(IdentityResult result) 
        {
            if (!result.Succeeded) 
            { 
                throw new Exception(result.Errors.First().Description);
            }
        }
    }
}
