using Microsoft.Extensions.DependencyInjection;
using Rename.Cli;

var collection = new ServiceCollection();

collection.AddSingleton<Application>();

var provider = collection.BuildServiceProvider();

var app = provider.GetRequiredService<Application>();

await app.RunAsync();