//using MediatR;
//using Microsoft.AspNetCore.Builder;
//using Microsoft.AspNetCore.Hosting;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Hosting;
//using Microsoft.IdentityModel.Tokens;
//using System.Linq;
//using Vita.Goals.Application.Abstraction.Categories.Commands;
//using Vita.Goals.Application.Commands.Categories;
//using Vita.Goals.Application.Queries.Configuration;
//using Vita.Goals.Domain.Aggregates.Categories;
//using Vita.Goals.Domain.Aggregates.Goals;
//using Vita.Goals.Persistance.Sql;
//using Vita.Goals.Persistance.Sql.Aggregates.Categories;
//using Vita.Goals.Persistance.Sql.Aggregates.Goals;

//namespace Vita.Goals.Host
//{
//    public class Startup
//    {
//        private const string SpaCorsPolicy = "spa-cors";

//        public IConfiguration Configuration { get; }

//        public Startup(IConfiguration configuration)
//        {
//            Configuration = configuration;
//        }

//        public void ConfigureServices(IServiceCollection services)
//        {
//            services.AddControllers().AddNewtonsoftJson(options =>
//            {
//                options.SerializerSettings.DateTimeZoneHandling = Newtonsoft.Json.DateTimeZoneHandling.Utc;
//                options.SerializerSettings.DateFormatString = "yyyy'-'MM'-'dd'T'HH':'mm':'ssZ";
//            });


//            string[] allowedOrigins = Configuration.GetSection("AllowedOrigins").Get<string[]>();
//            services.AddCors(options =>
//            {
//                options.AddPolicy(name: SpaCorsPolicy, builder =>
//                {
//                    builder.WithOrigins(allowedOrigins.ToArray())
//                           .AllowAnyHeader()
//                           .AllowAnyMethod();
//                });
//            });

//            services.AddAuthentication("Bearer")
//                    .AddJwtBearer("Bearer", options =>
//                    {
//                        options.Authority = Configuration["JWTAuthority"];
//                        options.RequireHttpsMetadata = false;

//                        options.TokenValidationParameters = new TokenValidationParameters
//                        {
//                            ValidateAudience = false
//                        };
//                    });

//            services.AddAuthorization(options =>
//            {
//                options.AddPolicy("ApiScope", policy =>
//                {
//                    policy.RequireAuthenticatedUser();
//                    policy.RequireClaim("scope", "goals");
//                });
//            });

//            AddApplicationBootstrapping(services);
//            AddPersistanceBootstrapping(services);

//            services.AddApplicationInsightsTelemetry(Configuration["APPINSIGHTS_INSTRUMENTATIONKEY"]);

//            services.AddControllers();
//        }

//        private void AddApplicationBootstrapping(IServiceCollection services)
//        {
//            services.AddMediatR(typeof(CreateCategoryCommand), typeof(CreateCategoryCommandHandler));
//            services.AddSingleton<IConnectionStringProvider>(new ConnectionStringProvider(Configuration.GetConnectionString("VitaApiDbContext")));
//        }

//        private void AddPersistanceBootstrapping(IServiceCollection services)
//        {
//            services.AddScoped<ICategoriesRepository, CategoriesRepository>();
//            services.AddScoped<IGoalsRepository, GoalsRepository>();
//            services.AddDbContext<VitaApiDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("VitaApiDbContext")));
//        }

//        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
//        {
//            if (env.IsDevelopment())
//                app.UseDeveloperExceptionPage();


//            app.UseHttpsRedirection();
//            app.UseRouting();

//            app.UseCors(SpaCorsPolicy);

//            app.UseAuthentication();
//            app.UseAuthorization();

//            app.UseEndpoints(endpoints => endpoints.MapControllers());

//            AutoMigrateDB(app);
//        }

//        public void AutoMigrateDB(IApplicationBuilder app)
//        {
//            if (Configuration["AutoMigrateDB"] == null || !bool.Parse(Configuration["AutoMigrateDB"]))
//                return;

//            using var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope();
//            var context = serviceScope.ServiceProvider.GetService<VitaApiDbContext>();
//            context.Database.Migrate();
//        }
//    }
//}
