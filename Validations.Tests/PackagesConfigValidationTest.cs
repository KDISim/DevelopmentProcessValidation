using System;
using System.Collections.Generic;
using System.Linq;
using DevelopmentProcessValidation.Abstractions;
using NSubstitute;
using NUnit.Framework;
using VisualStudio.Files.Abstractions;

namespace DevelopmentProcessValidation.Validations.Tests
{
    [TestFixture]
    public class PackagesConfigValidationTest
    {
        private const string DefaultPath = @"c:\path\to\some\solution.sln";
        
        private ISolutionReader _solutionReaderSubstitute;
        private IProjectFileParser _projectFileParserSubstitute;
        
        private PackagesConfigValidation _validation;

        private IEnumerable<IProjectFilePackageReference> NoPackageReferences()
        {
            return new List<IProjectFilePackageReference>();
        }
        
        private IEnumerable<IProjectFilePackageReference> PackageReferences(params IProjectFilePackageReference[] packageReferences)
        {
            return packageReferences;
        }
         
        private IProjectFilePackageReference PackageReference(string id, string version)
        {
            var substitute = Substitute.For<IProjectFilePackageReference>();
            substitute.Id.Returns(id);
            substitute.Version.Returns(version);

            return substitute;
        }

        private IEnumerable<IPackageReference> NoPackages()
        {
            return new List<IPackageReference>();
        }
        
        private IEnumerable<IPackageReference> Packages(params IPackageReference[] packageReferences)
        {
            return packageReferences;
        }
         
        private IPackageReference Package(string id, string version)
        {
            var substitute = Substitute.For<IPackageReference>();
            substitute.Id.Returns(id);
            substitute.Version.Returns(version);

            return substitute;
        }

        private void ArrangeTestData(IEnumerable<IProjectFilePackageReference> references, IEnumerable<IPackageReference> packages)
        {
            var solution = Substitute.For<ISolution>();
            var project = Substitute.For<IProject>();
           
            solution.Projects.Returns(new List<IProject>{project});
            _solutionReaderSubstitute.ReadFromFile(Arg.Any<string>()).Returns(solution);
            
            _projectFileParserSubstitute.ParsePackageReferences(Arg.Any<IProject>()).Returns(references);            
            project.Packages.Returns(packages);
        }

        private IPackagesConfigValidationResult InvokeSolutionIsValid(string path)
        {
            var result = _validation.ValidateSolution(path);
            
            //force IEnumerable execution
            result.InvalidReferences.ToList();

            return result;
        }
        
        private IPackagesConfigValidationResult InvokeSolutionIsValid()
        {
            return InvokeSolutionIsValid(DefaultPath);
        }
        
        [SetUp]
        public void SetUp()
        {
            _solutionReaderSubstitute = Substitute.For<ISolutionReader>();
            _projectFileParserSubstitute = Substitute.For<IProjectFileParser>();
            _validation = new PackagesConfigValidation(_solutionReaderSubstitute, _projectFileParserSubstitute);
        }
        
        [Test]
        public void ConstructorMustThrowArgumentNullExceptionWhenReaderIsNull()
        {
            var exception = Assert.Throws<ArgumentNullException>(() => new PackagesConfigValidation(null, _projectFileParserSubstitute));
            Assert.That(exception.Message, Does.Contain("solutionReader"));
        }
        
        [Test]
        public void ConstructorMustThrowArgumentNullExceptionWhenProjectFileParserIsNull()
        {
            var exception = Assert.Throws<ArgumentNullException>(() => new PackagesConfigValidation(_solutionReaderSubstitute, null));
            Assert.That(exception.Message, Does.Contain("projectFileParser"));
        }
        
        [Test]
        public void ValidateSolutionMustThrowArgumentExceptionWhenPathIsNull()
        {
            var exception = Assert.Throws<ArgumentException>(() => _validation.ValidateSolution(null));
            Assert.That(exception.Message.StartsWith("Path cannot be null, empty or contain only whitespaces"));
        }
        
        [Test]
        public void ValidateSolutionMustReadSolutionFromTheProvidedPath()
        {
            InvokeSolutionIsValid();
            _solutionReaderSubstitute.Received().ReadFromFile(DefaultPath);
        }

        [Test]
        public void ValidateSolutionMustReturnTrueWhenSolutionContainsNoProjects()
        {
            var solution = Substitute.For<ISolution>();
            _solutionReaderSubstitute.ReadFromFile(Arg.Any<string>()).Returns(solution);

            solution.Projects.Returns(new List<IProject>());
            var result = InvokeSolutionIsValid();
            
            Assert.That(result.IsValid, Is.True);
        }
        
        [Test]
        public void ValidateSolutionMustParsePackageReferencesFromProjectFileWhenSolutionContainsProjects()
        {
            var solution = Substitute.For<ISolution>();
            var project = Substitute.For<IProject>();
            
            solution.Projects.Returns(new List<IProject>{project});
            
            _solutionReaderSubstitute.ReadFromFile(Arg.Any<string>()).Returns(solution);

            InvokeSolutionIsValid();
            
            Assert.That(_projectFileParserSubstitute.ReceivedCalls().Count(), Is.EqualTo(1));
        }
        
        [Test]
        public void ValidateSolutionMustReturnTrueWhenProjectFileContainsNoPackageReferences()
        {
            var solution = Substitute.For<ISolution>();
            var project = Substitute.For<IProject>();
            
            solution.Projects.Returns(new List<IProject>{project});
            
            _solutionReaderSubstitute.ReadFromFile(Arg.Any<string>()).Returns(solution);
            _projectFileParserSubstitute.ParsePackageReferences(Arg.Any<IProject>())
                .Returns(new List<IProjectFilePackageReference>());

            var result = InvokeSolutionIsValid();
            Assert.That(result.IsValid, Is.True);
        }
        
        [Test]
        public void ValidateSolutionMustReturnTrueWhenProjectFileContainsPackageReferencesThatAreAlsoPackagesConfig()
        {
            ArrangeTestData(
                PackageReferences(
                    PackageReference("some.fake.package", "1.3.3.7")
                ),
                Packages(
                    Package("some.fake.package", "1.3.3.7")
                )
            );
            var result = InvokeSolutionIsValid();
            Assert.That(result.IsValid, Is.True);
        }
        
        [Test]
        public void ValidateSolutionMustReturnFalseWhenProjectFileContainsPackageReferencesThatAreNotPackagesConfig()
        {
            ArrangeTestData(
                PackageReferences(
                    PackageReference("some.fake.package", "1.3.3.7")
                ),
                Packages(
                    Package("some.other.fake.package", "1.3.3.7")
                )
            );
            
            var result = InvokeSolutionIsValid();
            Assert.That(result.IsValid, Is.False);
        }
        
        [Test]
        public void ValidateSolutionMustReturnFalseWhenProjectFileContainsPackageReferencesAndThereIsNoPackagesConfig()
        {
            ArrangeTestData(
                PackageReferences(
                    PackageReference("some.fake.package", "1.3.3.7")
                ),
                NoPackages()
            );
            
            var result = InvokeSolutionIsValid();
            Assert.That(result.IsValid, Is.False);
        }
        
        [Test]
        public void ValidateSolutionMustReturnTrueWhenProjectFileContainsNoPackageReferencesAndThereArePackagesInPackagesConfig()
        {
            ArrangeTestData(
                NoPackageReferences(),
                Packages(
                    Package("some.fake.package", "1.3.3.7")
                )
            );
            
            var result = InvokeSolutionIsValid();
            Assert.That(result.IsValid, Is.True);
        }
        
        [Test]
        public void ValidateSolutionMustReturnTrueWhenProjectFileContainsMultiplePackageReferencesToTheSamePackageAndPackagesIsInPackagesConfig()
        {
            ArrangeTestData(
                PackageReferences(
                    PackageReference("some.fake.package", "1.3.3.7"),
                    PackageReference("some.fake.package", "1.3.3.7"),
                    PackageReference("some.fake.package", "1.3.3.7"),
                    PackageReference("some.fake.package", "1.3.3.7")
                ),
                Packages(
                    Package("some.fake.package", "1.3.3.7")
                )
            );
            
            var result = InvokeSolutionIsValid();
            Assert.That(result.IsValid, Is.True);
        }
        
        [Test]
        public void ValidateSolutionMustReturnFalseWhenProjectFileContainsMultiplePackageReferencesToTheSamePackageWithDifferentVersionsAndPackagesIsInPackagesConfig()
        {
            ArrangeTestData(
                PackageReferences(
                    PackageReference("some.fake.package", "1.3.3.7"),
                    PackageReference("some.fake.package", "1.3.3.6"),
                    PackageReference("some.fake.package", "1.3.2.7"),
                    PackageReference("some.fake.package", "2.3.3.7")
                ),
                Packages(
                    Package("some.fake.package", "1.3.3.7")
                )
            );
            
            var result = InvokeSolutionIsValid();
            Assert.That(result.IsValid, Is.False);
        }
        
        [Test]
        public void ValidateSolutionMustReturnFalseWhenProjectFileContainsMultiplePackageReferencesAndOneIsNotPackagesConfig()
        {
            ArrangeTestData(
                PackageReferences(
                    PackageReference("some.fake.package", "1.3.3.7"),
                    PackageReference("some.package", "2.3.7"),
                    PackageReference("some.package", "3.3.3"),
                    PackageReference("another.fake.package", "3.3.7"),
                    PackageReference("another.package", "4.7")
                ),
                Packages(
                    Package("some.fake.package", "1.3.3.7"),
                    Package("some.package", "2.3.7"),
                    Package("some.package", "3.3.3"),
                    Package("another.package", "4.7")
                )
            );
            
            var result = InvokeSolutionIsValid();
            Assert.That(result.IsValid, Is.False);
        }
    }
}