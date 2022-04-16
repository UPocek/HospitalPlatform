using Microsoft.EntityFrameworkCore;
using APP.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
builder.Services.AddControllers();
builder.Services.AddDbContext<MyContext>(opt =>
    opt.UseInMemoryDatabase("TestDB"));
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy => { policy.WithOrigins("*"); });
});

var app = builder.Build();

if (builder.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseCors(MyAllowSpecificOrigins);

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();