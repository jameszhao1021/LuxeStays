//using Microsoft.EntityFrameworkCore.Design;
//using Microsoft.EntityFrameworkCore;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace LuxeStays.Infrastructure.Data
//{
//    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
//    {
//        public ApplicationDbContext CreateDbContext(string[] args)
//        {
//            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

//            var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
//            Console.WriteLine($"DATABASE_URL = {databaseUrl}");

//            if (!string.IsNullOrEmpty(databaseUrl) && databaseUrl.StartsWith("postgres://"))
//            {
//                optionsBuilder.UseNpgsql(ConvertHerokuConnectionString(databaseUrl));
//            }
//            else
//            {
//                var defaultConn = "Server=LAPTOP-2L9OJD43\\JAMES;Database=LuxeStays;Encrypt=True;TrustServerCertificate=True;Trusted_Connection=True;MultipleActiveResultSets=True";
//                optionsBuilder.UseNpgsql(defaultConn);
//            }

//            return new ApplicationDbContext(optionsBuilder.Options);
//        }


//        private string ConvertHerokuConnectionString(string databaseUrl)
//        {
//            var uri = new Uri(databaseUrl);
//            var userInfo = uri.UserInfo.Split(':');
//            return $"Host={uri.Host};Port={uri.Port};Database={uri.AbsolutePath.TrimStart('/')};Username={userInfo[0]};Password={userInfo[1]};SSL Mode=Require;Trust Server Certificate=true";
//        }
//    }

//}
