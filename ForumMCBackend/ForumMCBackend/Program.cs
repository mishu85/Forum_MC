using AutoMapper;
using ForumMCBackend.Db;
using ApplicationCore.Entities;
using ApplicationCore.Entities.DTOs;
using ForumMCBackend.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using ForumMCBackend.Middleware;

using (var context = new SQLiteContext())
{
    var dbCreated = context.Database.EnsureCreated();
    if (dbCreated)
    {
        context.Accounts.Add(new Account
        (
            "Admin",
            BCrypt.Net.BCrypt.HashPassword("Admin"),
            AccountRoles.ADMIN
        ));

        context.Categories.Add(new Category("Cars"));
        context.Categories.Add(new Category("Space"));
        context.Categories.Add(new Category("Cooking"));

        context.SaveChanges();
    }
}

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "_myAllowSpecificOrigins",
                      builder =>
                      {
                          builder.WithOrigins("http://localhost:3000");
                      });
});
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

builder.Services.AddScoped<ICategoriesRepository, SQLiteCategoriesRepository>();
builder.Services.AddScoped<IAccountsRepository, SQLiteAccountsRepository>();
builder.Services.AddScoped<IMessagesRepository, SQLiteMessagesRepository>();
builder.Services.AddScoped<ITopicsRepository, SQLiteTopicsRepository>();

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

app.UseMiddleware<ExceptionMiddleware>();

app.UseRouting();

app.UseCors(x => x.AllowAnyHeader()
      .AllowAnyMethod()
      .WithOrigins("http://localhost:3000"));

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
