using CloudinaryDotNet;
using dotenv.net;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using PeakLift.Data;
using PeakLift.Repository;
using PeakLift.Services;
using static PeakLift.Data.ModelBuilderExtensions;

namespace PeakLift
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<Context>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
                        //sqlOptions => sqlOptions.EnableRetryOnFailure()
            );


            builder.Services.AddControllers();

            // Cross-Origin 
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowOrigin", policy =>
                {
                    policy.WithOrigins("http://localhost:4200")
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });


            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                // KeyCloak
                c.CustomSchemaIds(type => type.ToString());
                var securityScheme = new OpenApiSecurityScheme
                {
                    Name = "KEYCLOAK",
                    Type = SecuritySchemeType.OAuth2,
                    In = ParameterLocation.Header,
                    BearerFormat = "JWT",
                    Scheme = "bearer",
                    Flows = new OpenApiOAuthFlows
                    {
                        AuthorizationCode = new OpenApiOAuthFlow
                        {
                            AuthorizationUrl = new Uri(builder.Configuration["Jwt:AuthorizationUrl"]),
                            TokenUrl = new Uri(builder.Configuration["Jwt:TokenUrl"]),
                            Scopes = new Dictionary<string, string> { }
                        }
                    },
                    Reference = new OpenApiReference
                    {
                        Id = JwtBearerDefaults.AuthenticationScheme,
                        Type = ReferenceType.SecurityScheme
                    }
                };
                c.AddSecurityDefinition(securityScheme.Reference.Id, securityScheme);
                c.AddSecurityRequirement(new OpenApiSecurityRequirement{
                                                {securityScheme, new string[] { }}
                                            });
            });

            // KeyCloak
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            }).AddJwtBearer(o =>
            {
                o.Authority = builder.Configuration["Jwt:Authority"];
                o.Audience = builder.Configuration["Jwt:Audience"];
                o.RequireHttpsMetadata = false;

                o.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidateAudience = true,
                    NameClaimType = "preferred_username",
                    RoleClaimType = System.Security.Claims.ClaimTypes.Role
                };

                o.Events = new JwtBearerEvents
                {
                    OnTokenValidated = context =>
                    {
                        var identity = context.Principal.Identity as System.Security.Claims.ClaimsIdentity;

                        var realmAccess = context.Principal.FindFirst("realm_access")?.Value;

                        if (realmAccess != null)
                        {
                            var parsed = System.Text.Json.JsonDocument.Parse(realmAccess);

                            if (parsed.RootElement.TryGetProperty("roles", out var roles))
                            {
                                foreach (var role in roles.EnumerateArray())
                                {
                                    identity.AddClaim(new System.Security.Claims.Claim(
                                        System.Security.Claims.ClaimTypes.Role,
                                        role.GetString()
                                    ));
                                }
                            }
                        }

                        return Task.CompletedTask;
                    }
                };
            });

            DotEnv.Load(options: new DotEnvOptions(probeForEnv: true));

            var cloudinary = new Cloudinary(Environment.GetEnvironmentVariable("CLOUDINARY_URL"));
            cloudinary.Api.Secure = true;

            // register in DI
            builder.Services.AddSingleton(cloudinary);
            builder.Services.AddScoped<CloudinaryService>();

            builder.Services.AddScoped<IAppUnitOfWork, AppUnitOfWork>();
            builder.Services.AddScoped<IBodyGroupRepository, BodyGroupRepository>();
            builder.Services.AddScoped<IExerciseRepository, ExerciseRepository>();
            builder.Services.AddScoped<IWorkoutRepository, WorkoutRepository>();
            builder.Services.AddScoped<IWorkoutExerciseRepository, WorkoutExerciseRepository>();
            builder.Services.AddScoped<IWorkoutSetRepository, WorkoutSetRepository>();

            builder.Services.AddScoped<IBodyPartsService, BodyPartsService>();
            builder.Services.AddScoped<IExercisesService, ExercisesService>();
            builder.Services.AddScoped<IWorkoutsService, WorkoutsService>();
            builder.Services.AddScoped<IDashboardService, DashboardService>();
            builder.Services.AddScoped<IAnalyticsService, AnalyticsService>();

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<Context>();
                await DbInitializer.SeedAsync(context);
            }


            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "GymAPI");
                    c.OAuthClientId(builder.Configuration["Jwt:ClientId"]);
                    c.OAuthClientSecret(builder.Configuration["Jwt:ClientSecret"]);
                    c.OAuthRealm(builder.Configuration["Jwt:Realm"]);
                    c.OAuthAppName("KEYCLOAK");
                });
            }

            app.UseHttpsRedirection();

            app.UseCors("AllowOrigin");

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
