using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using System.Net.Mime;
using System.Text.Json;
using WebAPI.Repositories;
using WebAPI.Settings;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));
BsonSerializer.RegisterSerializer(new DateTimeOffsetSerializer(BsonType.String));

builder.Services.AddControllers(options =>
{
	options.SuppressAsyncSuffixInActionNames = false;
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var mongoDbSettings = builder.Configuration.GetSection(nameof(MongoDbSettings)).Get<MongoDbSettings>();
builder.Services.AddSingleton<IMongoClient>(s =>
{
	return new MongoClient(mongoDbSettings.ConnectionString);
});
builder.Services.AddSingleton<IItemsRepo, MongoDbItemsRepo>();
builder.Services.AddHealthChecks()
	.AddMongoDb(
		mongoDbSettings.ConnectionString,
		name: "mongodb",
		timeout: TimeSpan.FromSeconds(3),
		tags: new[] { "ready" });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
	app.UseHttpsRedirection();
}

app.UseAuthorization();

app.MapControllers();
// MongoDb Check for receive reqs
app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
	Predicate = (check) => check.Tags.Contains("ready"),
	ResponseWriter = async (context, report) =>
	{
		var res = JsonSerializer.Serialize(
			new
			{
				status = report.Status.ToString(),
				checks = report.Entries.Select(e => new
				{
					name = e.Key,
					status = e.Value.Status.ToString(),
					exception = e.Value.Exception != null ? e.Value.Exception.Message : "None",
					duration = e.Value.Duration.ToString()
				})
			});
		context.Response.ContentType = MediaTypeNames.Application.Json;
		await context.Response.WriteAsync(res);
	}
});
// API Check for receive reqs
app.MapHealthChecks("/health/live", new HealthCheckOptions
{
	Predicate = (_) => false
});

app.Run();
