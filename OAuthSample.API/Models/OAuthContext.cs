using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OAuthSample.API.Models
{
    public class OAuthContext : DbContext
    {

        public OAuthContext(DbContextOptions<OAuthContext> options) : base(options)
        {

        }
        public DbSet<Customer> Customers { get; set; }
    }
}
