namespace Rename.Cli.Operations;

public interface IRenameOperation
{
    (string oldValue, string newValue) RenameProjectFile(FileInfo existingProject, string newProjectNameWithExtension);
    (string oldValue, string newValue) RenameDirectory(FileInfo existingProject, string newProjectNameWithExtension);
    Task UpdateProjectReferenceInSolutionFile(FileInfo solutionFile, IDictionary<string, string> renamedProjects, CancellationToken cancellationToken);
}