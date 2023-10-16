using Microsoft.EntityFrameworkCore;
using Employee.EventBus.Data;
using Employee.EventBus.Data.Interfaces;
using MassTransit;
using Employee.EventBus.Consumers;
using RabbitMQ.Client;

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
                    // Connection
                    cfg.Host(builder.Configuration["EventBusSettings:HostAddress"]);

                    // Primary Queue Configuration
                    cfg.ReceiveEndpoint("employee_q", c =>
                    {
                        c.PrefetchCount = 16;

                        // Set up the Dead Letter Exchange and routing key on the original queue
                        c.SetQueueArgument("x-dead-letter-exchange", "dead_letter_exchange");
                        c.SetQueueArgument("x-dead-letter-routing-key", "employee_dlq");

                        // Retry policy configuration
                        c.UseMessageRetry(retryConfigurator =>
                        {
                            retryConfigurator.Interval(1, TimeSpan.FromSeconds(1)); // Adjust retry attempts and interval as necessary
                            retryConfigurator.Handle<Exception>(); // This can be more specific based on the exceptions you expect
                        });

                        c.ConfigureConsumer<EmployeeDtoEventConsumer>(ctx);
                    });

                    // Dead Letter Queue Configuration
                    cfg.ReceiveEndpoint("employee_dlq", e =>
                    {
                        // Other configurations for DLQ like PrefetchCount, etc.
                        // Ensure you have monitoring or alerts for messages in this queue.

                        e.Bind("dead_letter_exchange", s =>
                        {
                            s.ExchangeType = ExchangeType.Direct;
                            s.RoutingKey = "employee_dlq";
                        });
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