
Console.WriteLine("Provide path to project which is to be renamed: ");
string? existingProjectPathWithExtension = Console.ReadLine();

if (!File.Exists(existingProjectPathWithExtension) && Directory.Exists(existingProjectPathWithExtension))
{
    existingProjectPathWithExtension = Directory.GetFiles(existingProjectPathWithExtension)
        .FirstOrDefault(fileName => fileName.EndsWith(".csproj"));

    if (existingProjectPathWithExtension is null)
    {
        Console.WriteLine("Could not find .csproj file in given path");
        return;
    }
}

Console.WriteLine("Provide new project name: ");
string newProjectNameWithExtension = Console.ReadLine()!;

if (!newProjectNameWithExtension.EndsWith(".csproj"))
{
    newProjectNameWithExtension += ".csproj";
}


DirectoryInfo projectFolder = Directory.GetParent(existingProjectPathWithExtension);
string newFolderPath = GetNewProjectFolderPath(projectFolder, newProjectNameWithExtension);

// Rename project file
File.Move(existingProjectPathWithExtension, $"{projectFolder}/{newProjectNameWithExtension}");


// Rename directory
Directory.Move(projectFolder.FullName, newFolderPath);


Console.WriteLine($"Rename completed for project: {existingProjectPathWithExtension}");


static string GetNewProjectFolderPath(DirectoryInfo? existingProjectFolder, string newProjectName)
{
    string newProjectFolderName = newProjectName.Substring(0, newProjectName.Length - ".csproj".Length);

    string newFolderName = $"{existingProjectFolder.Parent.FullName}/{newProjectFolderName}";

    return newFolderName;
}
