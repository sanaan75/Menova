using System.Reflection;
using Entities;
using Entities.Caches;
using Microsoft.EntityFrameworkCore;
using Persistence;
using Persistence.Contexts;
using Services;
using Services.Apis;
using Web;
using Web.APIs;
using Web.MinimalApis;
using Web.RateLimits;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5090","127.0.0.1") // آدرس React خودت
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

services.AddDbContext<AppDbContext>(opts => { opts.UseSqlServer(builder.Configuration["ConnectionStrings:MainConnection"]); });
services.AddDbContext<StatisticContext>(opts => { opts.UseSqlServer(builder.Configuration["ConnectionStrings:StatisticConnection"]); });

services.AddControllers(options => { options.Filters.Add<ApiExceptionFilter>(); });

services.RegisterHttpClientInstances(GetAssembliesToBeRegisteredInIocContainer());

services.RegisterAssemblyPublicNonGenericClasses(GetAssembliesToBeRegisteredInIocContainer())
    .Where(i => !i.IsAssignableTo<ISingleInstance>())
    .AsPublicImplementedInterfaces(ServiceLifetime.Scoped);

services.RegisterAssemblyPublicNonGenericClasses(GetAssembliesToBeRegisteredInIocContainer())
    .Where(i => i.IsAssignableTo<ISingleInstance>())
    .AsPublicImplementedInterfaces(ServiceLifetime.Singleton);

services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();
services.AddTransient<IContainer, AspNetCoreContainer>();

services.AddSingleton(typeof(ICache<,>), typeof(Cache<,>));
services.AddSingleton(typeof(IFullCache<,>), typeof(FullCache<,>));
services.AddSingleton(typeof(IPartialCache<,>), typeof(PartialCache<,>));
services.AddSingleton<IRateLimitService, RateLimitService>();
builder.Services.AddScoped<ITokenValidator, TokenValidator>();

services.AddScoped<LoggingMiddleware>();

services.AddHttpContextAccessor();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseCors("AllowFrontend"); // ← اینجا

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<ApiTokenMiddleware>();
app.UseMiddleware<LoggingMiddleware>();

app.MapControllers();

app.MapAllEndpoints();

app.Run();

Assembly[] GetAssembliesToBeRegisteredInIocContainer()
{
    return new[]
    {
        typeof(IDb).Assembly,
        typeof(_CommonEntitiesDummy).Assembly,
        typeof(_CommonServicesDummy).Assembly,
        typeof(_WebCommonDummy).Assembly,
        typeof(_PersistenceCommonDummy).Assembly,
        typeof(_EntitiesDummy).Assembly,
        typeof(_ServicesDummy).Assembly,
        typeof(_PersistenceDummy).Assembly,
        typeof(_WebDummy).Assembly
    };
}