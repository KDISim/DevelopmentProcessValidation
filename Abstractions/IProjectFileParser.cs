using System.Collections.Generic;
using VisualStudio.Files.Abstractions;

namespace DevelopmentProcessValidation.Abstractions
{
    public interface IProjectFileParser
    {
        IEnumerable<IProjectFilePackageReference> ParsePackageReferences(IProject project);
    }
}