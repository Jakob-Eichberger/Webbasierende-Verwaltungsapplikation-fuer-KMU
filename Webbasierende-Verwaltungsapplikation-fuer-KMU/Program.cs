using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Webbasierende_Verwaltungsapplikation_fuer_KMU
{
    public class Program

    {
        public static void Main(string[] args)
        {
            using (var db = new Infrastructure.Database())
            {
#if DEBUG
                db.Database.EnsureDeleted();
                Console.WriteLine("Deleted");
#endif
                if (db.Database.EnsureCreated())
                {
                    db.GenerateFakeData();
                }
            }
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) => Host.CreateDefaultBuilder(args).ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
    }
}
