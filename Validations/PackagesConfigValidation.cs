using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using DevelopmentProcessValidation.Abstractions;
using VisualStudio.Files.Abstractions;

namespace DevelopmentProcessValidation.Validations
{
    public class PackagesConfigValidation : IPackagesConfigValidation
    {
        private readonly ISolutionReader _solutionReader;
        private readonly IProjectFileParser _projectFileParser;
        
        public PackagesConfigValidation(ISolutionReader solutionReader, IProjectFileParser projectFileParser)
        {
            _solutionReader = solutionReader ?? throw new ArgumentNullException(nameof(solutionReader));
            _projectFileParser =
                projectFileParser ?? throw new ArgumentNullException(nameof(projectFileParser));
        }

        public IPackagesConfigValidationResult ValidateSolution(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException($"Path cannot be null, empty or contain only whitespaces ({path})", nameof(path));
            }

            var solution = _solutionReader.ReadFromFile(path);

            var invallidReferences = solution.Projects.SelectMany(ValidateProject);
            return new PackagesConfigValidationResult(invallidReferences);
        }

        private IEnumerable<IProjectFilePackageReference> ValidateProject(IProject project)
        {
            var references = _projectFileParser.ParsePackageReferences(project);
            
            return references.Where(reference =>
                !project.Packages.Any(package => Equals(package.Id, reference.Id) && Equals(package.Version, reference.Version)));
        }
    }
}