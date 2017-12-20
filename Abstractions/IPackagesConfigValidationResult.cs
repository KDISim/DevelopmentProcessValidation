using System.Collections.Generic;

namespace DevelopmentProcessValidation.Abstractions
{
    public interface IPackagesConfigValidationResult
    {
        bool IsValid { get; }
        IEnumerable<IProjectFilePackageReference> InvalidReferences { get; }
    }
}