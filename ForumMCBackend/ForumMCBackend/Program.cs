using AutoMapper;
using ForumMCBackend.Db;
using ForumMCBackend.Models;
using ForumMCBackend.Models.DTOs;
using ForumMCBackend.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

using (var context = new SQLiteContext())
{
    var dbCreated = context.Database.EnsureCreated();
    if (dbCreated)
    {
        context.Accounts.Add(new Account
        {
            UserName = "Admin",
            Password = BCrypt.Net.BCrypt.HashPassword("Admin"),
            Role = AccountRoles.ADMIN,
        });

        context.Categories.Add(new Category { Title = "Cars" });
        context.Categories.Add(new Category { Title = "Space" });
        context.Categories.Add(new Category { Title = "Cooking" });

        context.SaveChanges();
    }
}

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var mapper = new MapperConfiguration((cfg) => {
        cfg.CreateMap<Account, AccountDTO>();
        cfg.CreateMap<Topic, TopicDTO>();
        cfg.CreateMap<Message, MessageDTO>();
    }).CreateMapper();

builder.Services.AddSingleton<IMapper>(mapper);

builder.Services.AddScoped<SQLiteCategoriesRepository>();
builder.Services.AddScoped<ICategoriesRepository>(x => x.GetRequiredService<SQLiteCategoriesRepository>());
builder.Services.AddScoped<SQLiteAccountsRepository>();
builder.Services.AddScoped<IAccountsRepository>(x => x.GetRequiredService<SQLiteAccountsRepository>());
builder.Services.AddScoped<SQLiteMessagesRepository>();
builder.Services.AddScoped<IMessagesRepository>(x => x.GetRequiredService<SQLiteMessagesRepository>());
builder.Services.AddScoped<SQLiteTopicsRepository>();
builder.Services.AddScoped<ITopicsRepository>(x => x.GetRequiredService<SQLiteTopicsRepository>());

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidAudience = builder.Configuration["Jwt:Audience"],
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UsePathBase(new PathString("/api"));
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// TODO: use to add role restriction middleware
// app.Use((context, next) =>
// {
//     var stream = context.Request.Headers["authorization"];
//     var handler = new JwtSecurityTokenHandler();
//     var jsonToken = handler.ReadToken(stream);
//     var tokenS = jsonToken as JwtSecurityToken;

//     var accountId = tokenS.Claims.First(claim => claim.Type == "UserId").Value;

//     var dbContext = context.RequestServices.GetRequiredService<SQLiteContext>();
//     var account = dbContext.Accounts.SingleOrDefault(entity => entity.Id == int.Parse(accountId));
//     //context.User = account;
//     Console.WriteLine(context.User);

//     return next(context);
// });
app.Run();
