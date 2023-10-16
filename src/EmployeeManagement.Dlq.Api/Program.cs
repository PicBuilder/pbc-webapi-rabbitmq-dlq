using EmployeeManagement.Dlq.Api.Consumers;
using MassTransit;
using RabbitMQ.Client;

namespace EmployeeManagement.Dlq.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            #region MassTransitInit
            builder.Services.AddMassTransit(config =>
            {
                config.AddConsumer<DlqConsumer>();

                config.UsingRabbitMq((ctx, cfg) =>
                {
                    cfg.Host(builder.Configuration["EventBusSettings:HostAddress"]);

                    cfg.ReceiveEndpoint("employee_dlq", c =>
                    {
                        c.ExchangeType = ExchangeType.Direct;
                        c.PrefetchCount = 16;
                        c.ConfigureConsumer<DlqConsumer>(ctx);

                    });
                });
            });

            builder.Services.AddScoped<DlqConsumer>();
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
            }

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}