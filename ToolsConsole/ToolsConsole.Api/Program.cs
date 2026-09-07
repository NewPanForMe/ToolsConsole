using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using ToolsConsole.Api.Middlewares;
using ToolsConsole.Api.Security;
using ToolsConsole.Application.Interfaces;
using ToolsConsole.Application.Services;
using ToolsConsole.Infrastructure;
using ToolsConsole.Infrastructure.Seed;

var builder = WebApplication.CreateBuilder(args);

// 仅使用控制台日志，避免在受限环境(无 Windows 事件日志权限)写 EventLog
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
});

// ---- DDD 各层服务注册 ----
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IDbConnService, DbConnService>();
builder.Services.AddScoped<IUserService, UserService>();

// ---- JWT 认证 ----
var jwtOptions = builder.Configuration.GetSection("JwtSettings").Get<JwtOptions>() ?? new JwtOptions();
if (string.IsNullOrWhiteSpace(jwtOptions.Key) || jwtOptions.Key.Length < 32)
{
    throw new InvalidOperationException("JwtSettings:Key 未配置或长度不足（建议 >= 32 字符），请检查 appsettings.json");
}

builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("JwtSettings"));
builder.Services.AddSingleton<JwtTokenGenerator>();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Key)),
            ValidateIssuer = true,
            ValidIssuer = jwtOptions.Issuer,
            ValidateAudience = true,
            ValidAudience = jwtOptions.Audience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(1),
        };
    });
builder.Services.AddAuthorization();

// ---- CORS（前端开发服务器）----
var corsOrigins = builder.Configuration.GetSection("Cors:Origins").Get<string[]>()
    ?? new[] { "http://localhost:5173" };
builder.Services.AddCors(options => options.AddPolicy("web", policy =>
    policy.WithOrigins(corsOrigins).AllowAnyHeader().AllowAnyMethod()));

builder.Services.AddOpenApi();

var app = builder.Build();

// 统一挂载前置路径 /ToolsConsole：请求 /ToolsConsole/xxx → 路由 /xxx（不带前缀的路径仍然兼容）
app.UsePathBase("/ToolsConsole");

app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options
            .WithTitle("ToolsConsole API")
            .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
    });
}

app.UseCors("web");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Map("/",()=>Results.Redirect("~/scalar")).ExcludeFromDescription(); ;

// 建库建表 + 播种 admin/123；失败直接终止，便于第一时间发现数据库连接问题
await DbInitializer.InitializeAsync(app.Services);

app.Run();
