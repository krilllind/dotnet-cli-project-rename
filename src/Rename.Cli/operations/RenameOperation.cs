namespace Rename.Cli.Operations;

internal class RenameOperation : IRenameOperation
{
    public (string oldValue, string newValue) RenameProjectFile(FileInfo existingProject, string newProjectNameWithExtension)
    {
        string newFullFileName = Path.Combine(existingProject.Directory.FullName, newProjectNameWithExtension);
        File.Move(existingProject.FullName, newFullFileName);

        return (existingProject.Name, newProjectNameWithExtension);
    }

    public (string oldValue, string newValue) RenameDirectory(FileInfo existingProject, string newProjectNameWithExtension)
    {
        string newDirectoryFullName = GetNewProjectDirectoryPath(existingProject.Directory, newProjectNameWithExtension);
        string newDirectoryName = newDirectoryFullName.Substring(newDirectoryFullName.LastIndexOf('\\') + 1);

        Directory.Move(existingProject.Directory.FullName, newDirectoryFullName);

        return (existingProject.Directory.Name, newDirectoryName);
    }

    public async Task UpdateProjectReferenceInSolutionFile(FileInfo solutionFile, IDictionary<string, string> renamedProjects, CancellationToken cancellationToken)
    {
        string[] lines = await File.ReadAllLinesAsync(solutionFile.FullName, cancellationToken);

        using StreamWriter writer = new StreamWriter(solutionFile.FullName);

        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i];

            if (line.StartsWith("Project("))
            {
                foreach ((string oldPath, string newPath) in renamedProjects)
                {
                    line = line.Replace(oldPath, newPath, StringComparison.InvariantCultureIgnoreCase);
                }
            }

            await writer.WriteLineAsync(line);
        }
    }

    
    private static string GetNewProjectDirectoryPath(DirectoryInfo? existingProjectFolder, string newProjectName)
    {
        string newProjectFolderName = newProjectName.Substring(0, newProjectName.Length - Globals.ProjectExtension.Length);

        return Path.Combine(existingProjectFolder.Parent.FullName, newProjectFolderName);
    }
}
