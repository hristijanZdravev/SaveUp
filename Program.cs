
using Microsoft.EntityFrameworkCore;
using SaveUp.Data;
using SaveUp.DTOs;
using SaveUp.Models;
using SaveUp.Repository;
using SaveUp.Repository.Interfaces;
using SaveUp.Services;
using SaveUp.Services.Implementations;

namespace SaveUp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<Context>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
                        //sqlOptions => sqlOptions.EnableRetryOnFailure()
            );

            // Add services to the container.
            builder.Services.AddScoped<IFeeCalculationHistoryRepository,FeeCalculationHistoryRepository>();
            builder.Services.AddScoped<IFeeRuleRepository, FeeRuleRepository>();
            builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
            builder.Services.AddScoped<ICommonRepository<Currency, CurrencyDTO>, CurrencyRepository>();
            builder.Services.AddScoped<ICommonRepository<TransactionType, TransactionTypeDTO>, TransactionTypeRepository>();
            builder.Services.AddScoped<ICommonRepository<Client, ClientDTO>, ClientRepository>();

            builder.Services.AddSingleton<IRuleEngineService, RuleEngineService>();
            builder.Services.AddScoped<IFeeCalculationService, FeeCalculationService>();


            builder.Services.AddControllers();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAngular", policy =>
                {
                    policy.WithOrigins("http://localhost:4200")
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });


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

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.UseCors("AllowAngular");

            app.MapControllers();

            app.Run();
        }
    }
}
