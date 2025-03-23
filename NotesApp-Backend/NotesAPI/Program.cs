using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using NotesAPI.Data;
using NotesAPI.Services;

var builder = WebApplication.CreateBuilder(args);


var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var jwtKey = jwtSettings.GetValue<string>("Key");
var jwtIssuer = jwtSettings.GetValue<string>("Issuer");
var jwtAudience = jwtSettings.GetValue<string>("Audience");

builder.Services.AddScoped<DapperContext>();
builder.Services.AddScoped<TokenService>(provider => new TokenService(jwtKey));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ✅ CORS POLICY CONFIGURATION - allow any origin for development! I know it is not safe to use for Production
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy
            .AllowAnyOrigin()   
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

// ✅ Configure Authentication & JWT Bearer
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();



app.UseSwagger();
app.UseSwaggerUI();
// ✅ Enable CORS first
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
