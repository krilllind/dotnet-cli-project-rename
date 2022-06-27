
Console.WriteLine("Provide path to Solution File (Optional): ");
string? solutionFile = Console.ReadLine();

Console.WriteLine("Provide path to project which is to be renamed: ");
string? existingProjectPath = Console.ReadLine();

FileInfo? existingProject = GetExistingProject(existingProjectPath);
if (existingProject is null)
{
    return;
}

Console.WriteLine("Provide new project name: ");
string newProjectNameWithExtension = Console.ReadLine()!;

if (!newProjectNameWithExtension.EndsWith(".csproj"))
{
    newProjectNameWithExtension += ".csproj";
}

string newFolderFullName = GetNewProjectFolderPath(existingProject.Directory, newProjectNameWithExtension);

Dictionary<string, string> renamedProjects = CreateRenameMap(existingProject, newFolderFullName, newProjectNameWithExtension);

// Rename project file
File.Move(existingProject.FullName, $"{existingProject.Directory.FullName}/{newProjectNameWithExtension}");


// Rename directory
Directory.Move(existingProject.Directory.FullName, newFolderFullName);


// Solution file update
if (!string.IsNullOrEmpty(solutionFile) && File.Exists(solutionFile))
{
    string[] lines = await File.ReadAllLinesAsync(solutionFile);

    using (StreamWriter writer = new StreamWriter(solutionFile))
    {
        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i];

            if (line.StartsWith("Project("))
            {
                foreach (KeyValuePair<string, string> item in renamedProjects)
                {
                    line = line.Replace(item.Key, item.Value, StringComparison.InvariantCultureIgnoreCase);
                }
            }

            await writer.WriteLineAsync(line);
        }
    }
}


Console.WriteLine($"Rename completed for project: {existingProjectPath}");


static string GetNewProjectFolderPath(DirectoryInfo? existingProjectFolder, string newProjectName)
{
    string newProjectFolderName = newProjectName.Substring(0, newProjectName.Length - ".csproj".Length);

    string newFolderName = Path.Combine(existingProjectFolder.Parent.FullName, newProjectFolderName);

    return newFolderName;
}

static FileInfo? GetExistingProject(string? existingProjectPath)
{
    if (File.Exists(existingProjectPath))
    {
        return new FileInfo(existingProjectPath);
    }

    if (Directory.Exists(existingProjectPath))
    {
        string directoryProject = Directory.GetFiles(existingProjectPath)
            .FirstOrDefault(fileName => fileName.EndsWith(".csproj"));

        if (directoryProject is null)
        {
            Console.WriteLine("Could not find .csproj file in given path");
            return null;
        }

        return new FileInfo(directoryProject);
    }

    return null;
}


static Dictionary<string, string> CreateRenameMap(FileInfo existingProject, string newFolderFullName, string newProjectNameWithExtension)
{
    // Contains old path -> new path (folder + project)
    Dictionary<string, string> renamedProjects = new Dictionary<string, string>();

    string oldProjectFolder = existingProject.Directory.Name;
    string oldProjectName = existingProject.Name;

    string newProjectFolder = newFolderFullName.Substring(newFolderFullName.LastIndexOf('\\') + 1);

    renamedProjects.Add(Path.Combine(oldProjectFolder, oldProjectName), Path.Combine(newProjectFolder, newProjectNameWithExtension));

    string oldSolutionProjectName = oldProjectName.Replace(".csproj", string.Empty);
    string newSolutionProjectName = newProjectNameWithExtension.Replace(".csproj", string.Empty);

    renamedProjects.Add(oldSolutionProjectName, newSolutionProjectName);

    return renamedProjects;
}