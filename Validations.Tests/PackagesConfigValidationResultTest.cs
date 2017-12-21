using System;
using System.Collections.Generic;
using DevelopmentProcessValidation.Abstractions;
using NUnit.Framework;

namespace DevelopmentProcessValidation.Validations.Tests
{
    [TestFixture]
    public class PackagesConfigValidationResultTest
    {
        [Test]
        public void ConstructorMustThrowArgumentNullExceptionWhenInvalidReferencesIsNull()
        {
            var exception = Assert.Throws<ArgumentNullException>(() => new PackagesConfigValidationResult(null));
            Assert.That(exception.Message, Does.Contain("invalidReferences"));
        }
        
        [Test]
        public void IsValidMustReturnTrueWhenInvalidReferencesIsEmpty()
        {
            var references = new List<IProjectFilePackageReference>();
            var result = new PackagesConfigValidationResult(references);

            Assert.That(result.IsValid, Is.True);
        }
        
        [Test]
        public void IsValidMustReturnFalseWhenInvalidReferencesIsNotEmpty()
        {
            var references = new List<IProjectFilePackageReference>
            {
                new ProjectFilePackageReference()
            };
            
            var result = new PackagesConfigValidationResult(references);

            Assert.That(result.IsValid, Is.False);
        }
        
        [Test]
        public void InvalidReferencesMustReturnReferencesPassedIntoConstructor()
        {
            var references = new List<IProjectFilePackageReference>
            {
                new ProjectFilePackageReference()
            };
            
            var result = new PackagesConfigValidationResult(references);

            Assert.That(result.InvalidReferences, Is.SameAs(references));
        }
    }
}