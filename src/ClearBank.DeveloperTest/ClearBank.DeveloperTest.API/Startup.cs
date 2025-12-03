using ClearBank.DeveloperTest.API.Filters;
using ClearBank.DeveloperTest.API.Middleware;
using ClearBank.DeveloperTest.Core.Services;
using ClearBank.DeveloperTest.Core.UseCases.Account.GetAccount;
using ClearBank.DeveloperTest.Core.UseCases.Payment.MakePayment;
using ClearBank.DeveloperTest.Infrastructure;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;

namespace ClearBank.DeveloperTest.API
{
    public class Startup
    {
        readonly string AllowSpecificOrigins = "_allowSpecificOrigins";
        public IConfiguration Configuration { get; }
        private readonly IWebHostEnvironment environment;

        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            this.environment = environment;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy(name: AllowSpecificOrigins, builder =>
                {
                    builder.WithOrigins(Configuration["Cors:AllowedOrigins"]?.Split(','));
                });
            });

            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidatorPipelineBehavior<,>));

            AssemblyScanner.FindValidatorsInAssembly(typeof(GetAccountQueryValidator).Assembly)
                .ForEach(item => services.AddScoped(item.InterfaceType, item.ValidatorType));

            services.AddControllers()
                .AddJsonOptions(opt =>
                {
                    opt.JsonSerializerOptions.Converters.Add(new DateTimeConverter());
                    opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                });
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "ClearBank API", Version = "v1" });
                c.OperationFilter<AuthorizeCheckOperationFilter>();
            });

            services.AddMediatR(cfg => {
                cfg.RegisterServicesFromAssembly(typeof(Startup).Assembly);
                cfg.RegisterServicesFromAssembly(typeof(GetAccountQuery).Assembly);
                cfg.RegisterServicesFromAssembly(typeof(MakePaymentRequest).Assembly);
            });

            services.AddRouting(options =>
            {
                options.LowercaseUrls = true;
                options.LowercaseQueryStrings = true;
            });

            services.AddHealthChecks();
            services.AddHttpContextAccessor();
            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<IPaymentValidationService, PaymentValidationService>();
            services.AddInfrastructureBindings(Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            bool disableMigration = Configuration.GetValue<bool>("DisableMigration");
            if (!disableMigration)
            {
                MigrateDatabase(app);
            }

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("v1/swagger.json", "ClearBank API V1");
                });
            }

            app.AddExceptionHandling();

            app.UseRouting();

            app.UseCors(AllowSpecificOrigins);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecks("/api/health");
                endpoints.MapControllers();
            });
        }

        private static void MigrateDatabase(IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ClearBankContext>();
            context.Database.Migrate();
        }
    }
}
