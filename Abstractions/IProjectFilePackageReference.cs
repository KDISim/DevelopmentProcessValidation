using VisualStudio.Files.Abstractions;

namespace DevelopmentProcessValidation.Abstractions
{
    public interface IProjectFilePackageReference
    {
        IProject Project { get; }
        string Element { get; }
        string Id { get; }
        string Version { get; }
    }
}