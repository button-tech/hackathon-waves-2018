namespace WavesBot
{
    using System;
    using System.Linq;
    using System.Net.Http;
    using Configuration;
    using Data;
    using DryIoc;
    using DryIoc.Microsoft.DependencyInjection;
    using IAL;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Services;
    using StackExchange.Redis;
    using TraceWriter = ViewLib.Helpers.TraceWriter;

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private static IServiceProvider Container { get; set; }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);


#if DEBUG
            services.AddDbContext<UserDataDbContext>(ServiceLifetime.Transient);
            var conn = "SqlDB";
            services.Configure<SqlServerConfiguration>(options => { options.ConnectionString = conn; });
#elif DEV || STAGE
services.AddEntityFrameworkNpgsql()
                .AddDbContext<UserDataDbContext>(ServiceLifetime.Transient);
            services.Configure<SqlServerConfiguration>(options =>
            {
                options.ConnectionString = Environment.GetEnvironmentVariable(EnvironmentVariables.Postgre);
            });
#endif

            var container = new Container(rules => rules.WithoutThrowOnRegisteringDisposableTransient());

            var botConfiguration = new BotConfiguration
            {
                BotToken = Environment.GetEnvironmentVariable(EnvironmentVariables.BotToken),
                LoggerBotToken = Environment.GetEnvironmentVariable(EnvironmentVariables.LoggerBotToken)
            };
            container.UseInstance(botConfiguration);

            var blockChainConfiguration = new BlockChainConfiguration
            {
                BlockChainAddress = Environment.GetEnvironmentVariable(EnvironmentVariables.BlockChainAddress)
            };
            container.UseInstance(blockChainConfiguration);


            IocModule.Load(container);
            ViewLib.IocModule.Load(container);

            var apiClient = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(2),
            };

            container.UseInstance(apiClient);


            container.UseInstance<Func<DateTimeOffset>>(() => DateTimeOffset.UtcNow);

            var redisHost = Environment.GetEnvironmentVariable(EnvironmentVariables.RedisHost);
            var redisPort = Environment.GetEnvironmentVariable(EnvironmentVariables.RedisPort);

            var textWriter = new TraceWriter();

#if RELEASE || STAGE
            var redisPassword = Environment.GetEnvironmentVariable(EnvironmentVariables.RedisPassword);
            container.UseInstance<Func<ConnectionMultiplexer>>(
                () => ConnectionMultiplexer.Connect($"{redisHost}:{redisPort},allowAdmin=true,password={redisPassword}",
                    textWriter));
#elif DEBUG || DEV
            var redisIp = System.Net.Dns.GetHostEntryAsync(redisHost).Result.AddressList.Last();
            container.UseInstance<Func<ConnectionMultiplexer>>(() =>
                ConnectionMultiplexer.Connect($"{redisIp}:{redisPort}", textWriter));
#endif
            var serviceProvider = container.WithDependencyInjectionAdapter(services, throwIfUnresolved: type => true);

#if RELEASE || STAGE || DEBUG || DEV || TECHWORKS
            BurnUp(serviceProvider);

            var stateSynchronizer = container.Resolve<StateSynchronizer>();
            SynchronizerInit(stateSynchronizer);
#endif

            UserDataDbContext DbContextFactory() => serviceProvider.Resolve<UserDataDbContext>();
            serviceProvider.UseInstance((Func<UserDataDbContext>) DbContextFactory);

            Container = serviceProvider.BuildServiceProvider();
            return Container;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, UserDataDbContext dbContext)
        {
#if RELEASE
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders =
                    Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedFor |
                    Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedProto
            });
#endif
            var address = Environment.GetEnvironmentVariable(EnvironmentVariables.BlockChainAddress);
            app.UseCors(builder => builder.WithOrigins("*") //address
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowAnyOrigin()
            );


            app.UseMiddleware<ErrorHandlingMiddleware>();

            InitDb(dbContext);

            app.UseMvc();
        }

        private static void BurnUp(IContainer container)
        {
            var types = container.GetServiceRegistrations()
                .Where(x => !x.ServiceType.Name.EndsWith(@"ViewModel"))
                .Select(x => x.ServiceType);

            foreach (var key in types)
            {
                try
                {
                    container.Resolve(key);
                }
                catch (Exception)
                {
#if DEBUG || DEV || STAGE
                    // ReSharper disable once LocalizableElement
                    Console.WriteLine($"Init {key.Name}");
#endif
                }
            }

            // ReSharper disable once LocalizableElement
            Console.WriteLine($"✅ Bot instance is Burn Up");
        }

        private async void InitDb(UserDataDbContext dbContext)
        {
            await dbContext.Database.EnsureCreatedAsync();
            //await dbContext.Database.MigrateAsync();
        }

        //Инициализируем главный сервис навигации
        private async void SynchronizerInit(StateSynchronizer synchronizer)
        {
            await synchronizer.Init();
        }
    }
}