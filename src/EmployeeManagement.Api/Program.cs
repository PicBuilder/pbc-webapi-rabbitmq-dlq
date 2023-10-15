using Microsoft.EntityFrameworkCore;
using Employee.EventBus.Data;
using Employee.EventBus.Data.Interfaces;
using MassTransit;
using Employee.EventBus.Consumers;

namespace EmployeeManagement.Api
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'ApplicationDbContext' not found.")));

            builder.Services.AddScoped<IApplicationDbContext, ApplicationDbContext>();
            builder.Services.AddScoped<ApplicationDbContextInitialiser>();

            // Add services to the container.
            #region MassTransitInit
            builder.Services.AddMassTransit(config =>
            {
                config.AddConsumer<EmployeeDtoEventConsumer>();

                config.UsingRabbitMq((ctx, cfg) =>
                {
                    cfg.Host(builder.Configuration["EventBusSettings:HostAddress"]);

                    cfg.ReceiveEndpoint("employee_q", c =>
                    {
                        c.PrefetchCount = 16;

                        c.ConfigureConsumer<EmployeeDtoEventConsumer>(ctx);

                    });

                    cfg.UseMessageRetry(retryConfigurator =>
                    {
                        retryConfigurator.Interval(3, TimeSpan.FromSeconds(1));
                        retryConfigurator.Handle<Exception>();
                    });

                });
            });

            builder.Services.AddScoped<EmployeeDtoEventConsumer>();
            #endregion


            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
                await app.InitialiseDatabaseAsync();
            }

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}