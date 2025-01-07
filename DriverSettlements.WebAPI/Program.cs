using DriverSettlements.Database;
using DriverSettlements.Database.Configuration;
using DriverSettlements.WebAPI.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<DatabaseConfiguration>(
    builder.Configuration.GetSection(DatabaseConfiguration.SectionName));
builder.Services.Configure<ExternalReportsConfiguration>(
    builder.Configuration.GetSection(ExternalReportsConfiguration.SectionName));
builder.Services.Configure<PhotosConfiguration>(
    builder.Configuration.GetSection(PhotosConfiguration.SectionName));
builder.Services.AddDbContext<DatabaseContext>();

builder.Logging.AddLog4Net();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

    app.UseSwagger();
    app.UseSwaggerUI();

//    app.UseRouting();
}
app.UseStaticFiles();
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
