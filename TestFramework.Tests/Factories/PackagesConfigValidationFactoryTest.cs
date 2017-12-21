using DevelopmentProcessValidation.TestFramework.Factories;
using NUnit.Framework;

namespace DevelopmentProcessValidation.TestFramework.Tests.Factories
{
    public class PackagesConfigValidationFactoryTest
    {
        [Test]
        public void CreateMustReturnAnInstance()
        {
            var instance = PackagesConfigValidationFactory.Create();
            Assert.That(instance, Is.Not.Null);
        }
        
    }
}