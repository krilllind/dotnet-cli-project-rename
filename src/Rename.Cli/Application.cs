using Rename.Cli.Operations;

namespace Rename.Cli;

public class Application
{
    private readonly IRenameOperation renameOperation;

    public Application(IRenameOperation renameOperation)
    {
        this.renameOperation = renameOperation;
    }

    public async Task RunAsync()
    {
        Console.WriteLine("Provide path to project which is to be renamed: ");
        FileInfo? existingProject = GetFileFromPath(Console.ReadLine(), Globals.ProjectExtension);

        if (existingProject is null)
        {
            Console.WriteLine($"Could not find a .csproj file in given path");
            return;
        }

        Console.WriteLine("Provide new project name: ");
        string? newProjectNameWithExtension = Console.ReadLine();

        if (string.IsNullOrEmpty(newProjectNameWithExtension))
        {
            Console.WriteLine("Project name must be provided");
            return;
        }

        if (!newProjectNameWithExtension.EndsWith(Globals.ProjectExtension))
        {
            newProjectNameWithExtension += Globals.ProjectExtension;
        }

        Console.WriteLine("Provide path to Solution File (Optional): ");
        FileInfo? solutionFile = GetFileFromPath(Console.ReadLine(), Globals.SolutionExtension);

        var projectUpdate = renameOperation.RenameProjectFile(existingProject, newProjectNameWithExtension);
        var directoryUpdate = renameOperation.RenameDirectory(existingProject, newProjectNameWithExtension);

        if (solutionFile?.Exists ?? false)
        {
            await UpdateSolutionFile(solutionFile, projectUpdate, directoryUpdate);
        }

        // TODO: Update project references in .csproj files

        // TODO: Update "using" & "namespace" definitions

        Console.WriteLine($"Rename completed for project: {existingProject.Name}");
    }

    private static FileInfo? GetFileFromPath(string? path, string fileExtension)
    {
        if (File.Exists(path))
        {
            return new FileInfo(path);
        }
        else if (!Directory.Exists(path))
        {
            return null;
        }

        string? directoryFile = Directory.GetFiles(path)
            .FirstOrDefault(fileName => fileName.EndsWith(fileExtension));

        if (directoryFile is not null)
        {
            return new FileInfo(directoryFile);
        }

        return null;
    }

    private Task UpdateSolutionFile(FileInfo solutionFile, (string oldValue, string newValue) projectUpdate, (string oldValue, string newValue) directoryUpdate)
    {
        string oldProjectRef = Path.Combine(directoryUpdate.oldValue, projectUpdate.oldValue);
        string newProjectRef = Path.Combine(directoryUpdate.newValue, projectUpdate.newValue);

        string oldProjectName = projectUpdate.oldValue.Replace(Globals.ProjectExtension, string.Empty);
        string newProjectName = projectUpdate.newValue.Replace(Globals.ProjectExtension, string.Empty);

        Dictionary<string, string> renamedProjects = new Dictionary<string, string>()
        {
            { oldProjectRef, newProjectRef },
            { oldProjectName, newProjectName }
        };

        return renameOperation.UpdateProjectReferenceInSolutionFile(solutionFile, renamedProjects, CancellationToken.None);
    }
}