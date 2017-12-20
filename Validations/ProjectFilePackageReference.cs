using DevelopmentProcessValidation.Abstractions;
using VisualStudio.Files.Abstractions;

namespace DevelopmentProcessValidation.Validations
{
    internal class ProjectFilePackageReference : IProjectFilePackageReference
    {
        public IProject Project { get; internal set; }
        public string Element { get; internal set; }
        public string Id { get; internal set; }
        public string Version { get; internal set; }
    }
}