using System.Linq;
using System.Text;
using DevelopmentProcessValidation.TestFramework.Factories;
using Microsoft.Build.Construction;
using NUnit.Framework;

namespace DevelopmentProcessValidation.TestFramework
{
    [Category("Process")]
    [Category("PreCompile")]
    public abstract class PreCompileProcessTestBase
    {
        public void ValidateThat_ProjectsDoNotReferencePackagesThatAreNotInPackagesConfigFile()
        {
            var validation = PackagesConfigValidationFactory.Create();
            var solutionFile = SolutionFile.Locate();
            var result = validation.ValidateSolution(solutionFile.FullName);

            Assert.That(result.IsValid, Is.True, () =>
            {
                var builder = new StringBuilder();
                builder.AppendLine("A project should not reference any files that are part of packages that are not in the packages.config file.");
                builder.AppendLine("Make sure that a packages.config file exists for the projects mentoined in the error message below.");
                builder.AppendLine("The solution contains the following projects that reference NuGet packages that are not in their packages.config file:");

                var groupedByProjectName = result.InvalidReferences.GroupBy(x => x.Project.File.Name);

                foreach (var group in groupedByProjectName)
                {
                    builder.AppendLine($"{group.First().Project.File.FullName}");
                    foreach (var reference in group)
                    {
                        builder.AppendLine($"Id: {reference.Id} Version: {reference.Version} ({reference.Element})");
                    }
                }

                return builder.ToString();
            });
        }
    }
}