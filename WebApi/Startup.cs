namespace WebApi
{
    using Abstraction.IRepositories;
    using Abstraction.IServices;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.OpenApi.Models;

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            // CORS configuration
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAnyOrigin", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });

            // SQL Server configuration
            services.AddDbContext<Data.Data.TradeMarketDbContext>(options =>
                options.UseSqlServer(this.Configuration.GetConnectionString("Market")));
            services.AddScoped<IUnitOfWork, Data.Data.UnitOfWork>();

            // MongoDB configuration
            //var mongoConnectionString = this.Configuration.GetConnectionString("MongoMarket");
            //var mongoClient = new MongoClient(mongoConnectionString);
            //var mongoDatabase = mongoClient.GetDatabase("TradeMarket");
            //services.AddSingleton<IMongoDatabase>(mongoDatabase);
            //services.AddScoped<IUnitOfWork, DalMongoDB.Data.UnitOfWork>();

            services.AddScoped<IProductService, Business.Services.ProductService>();
            services.AddScoped<ICustomerService, Business.Services.CustomerService>();
            services.AddScoped<IReceiptService, Business.Services.ReceiptService>();
            services.AddScoped<IStatisticService, Business.Services.StatisticService>();

            services.AddAutoMapper(typeof(Business.AutomapperProfile).Assembly);

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Trade Market API", Version = "v1" });
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Trade Market API v1"));
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            // Use CORS
            app.UseCors("AllowAnyOrigin");

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}