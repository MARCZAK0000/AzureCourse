using Azure.Data.Tables;
using Azure.Storage.Blobs;
using Azure.Storage.Queues;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton(new BlobServiceClient(builder.Configuration.
    GetConnectionString("EmulatorConnectionString"), new BlobClientOptions(BlobClientOptions.ServiceVersion.V2019_02_02)));
builder.Services.AddSingleton(new QueueServiceClient(builder.Configuration.
    GetConnectionString("EmulatorConnectionString"), new QueueClientOptions(QueueClientOptions.ServiceVersion.V2019_02_02)));
builder.Services.AddSingleton(new TableServiceClient(builder.Configuration.GetConnectionString("EmulatorConnectionString"), new TableClientOptions(TableClientOptions.ServiceVersion.V2019_02_02)));
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
