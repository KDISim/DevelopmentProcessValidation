A set of validations to catch common mistakes that may occur during the development process. This library uses [VisualStudio.Files](https://github.com/sanderaernouts/VisualStudio.Files) to parse the solution and project files.

# PackagesConfigValidation
Parses all .csproj files in the solution and validates that there are no references to files that belong to a NuGet package that is not present in the packages.config file for that project.

## Usage
```c#
var solutionReader = SolutionReaderFactory.Create();
var projectFileParser = new ProjectFileParser();

var validation = new PackagesConfigValidation(solutionReader, projectFileParser);
var result = validation.ValidateSolution("pathToSolutionFile.sln");
```

# Build
Use the build.cmd script to build from the command line. This script uses the dotnet cli to build the solution and to run the unit tests

## Example
```
> build.cmd
```

# Create local package
To create a NuGet package for this solution use the pack.cmd script. This script uses the dotnet cli to build the solution and creates a NuGet package out of that in the package directory in the root of this repository.

## Example
```
> pack.cmd 1.0.0-alpha1
```
