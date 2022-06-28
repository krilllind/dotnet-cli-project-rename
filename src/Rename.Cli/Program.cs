using Microsoft.Extensions.DependencyInjection;
using Rename.Cli;
using Rename.Cli.Operations;

var collection = new ServiceCollection();

collection.AddTransient<IRenameOperation, RenameOperation>();
collection.AddSingleton<Application>();

var provider = collection.BuildServiceProvider();

var app = provider.GetRequiredService<Application>();

await app.RunAsync();