namespace DevelopmentProcessValidation.Abstractions
{
    public interface IPackagesConfigValidation
    {
        IPackagesConfigValidationResult ValidateSolution(string path);
    }
}