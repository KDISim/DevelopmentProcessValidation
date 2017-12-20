using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using DevelopmentProcessValidation.Abstractions;

namespace DevelopmentProcessValidation.Validations
{
    public class PackagesConfigValidationResult : IPackagesConfigValidationResult
    {
        private readonly IEnumerable<IProjectFilePackageReference> _invalidReferences;
        
        public bool IsValid => !_invalidReferences.Any();

        public IEnumerable<IProjectFilePackageReference> InvalidReferences => _invalidReferences;

        internal PackagesConfigValidationResult(IEnumerable<IProjectFilePackageReference> invalidReferences)
        {
            _invalidReferences = invalidReferences ?? throw new ArgumentNullException(nameof(invalidReferences));
        }
    }
}