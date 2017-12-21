using NUnit.Framework;

namespace DevelopmentProcessValidation.TestFramework.Tests
{
    public class SolutionFileTest
    {
        [Test]
        public void LocateMustFindSolutionInParentDiretory()
        {
            var file = SolutionFile.Locate();
            Assert.That(file.Name, Is.EqualTo("DevelopmentProcessValidation.sln"));
        }
    }
}