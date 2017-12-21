using DevelopmentProcessValidation.Validations;
using VisualStudio.Files.Core.Factories;

namespace DevelopmentProcessValidation.TestFramework.Factories
{
    public static class PackagesConfigValidationFactory
    {
        public static PackagesConfigValidation Create()
        {
            var solutionReader = SolutionReaderFactory.Create();
            var projectFileParser = new ProjectFileParser();
            
            return new PackagesConfigValidation(solutionReader, projectFileParser);
        }
    }
}