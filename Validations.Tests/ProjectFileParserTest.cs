using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DevelopmentProcessValidation.Abstractions;
using NSubstitute;
using NUnit.Framework;
using VisualStudio.Files.Abstractions;

namespace DevelopmentProcessValidation.Validations.Tests
{
    public class ProjectFileParserTest
    {
        private ProjectFileParser _parser;
        [SetUp]
        public void SetUp()
        {
            _parser = new ProjectFileParser();
        }

        private IEnumerable<IProjectFilePackageReference> ParsePackageReferences(IProject project)
        {
            return _parser.ParsePackageReferences(project).ToList();
        }

        private FileInfo File(string relativePath)
        {
            var path = Path.Combine(TestContext.CurrentContext.TestDirectory, relativePath);
            return new FileInfo(path);
        }
        
        [Test]
        public void ParsePackageReferencesMustThrowArgumentNullReferenceExceptionWhenProjectIsNull()
        {
            var parser = new ProjectFileParser();
            var exception = Assert.Throws<ArgumentNullException>(() => ParsePackageReferences(null));
            Assert.That(exception.Message, Does.Contain("project"));
        }
        
        [Test]
        public void ParsePackageReferencesMustProjectToParsedProject()
        {
            var project = Substitute.For<IProject>();
            project.File.Returns(File(@"TestData\ProjectWithImportPackageReferences.csproj"));
            var result = ParsePackageReferences(project);
            
            Assert.Multiple(() =>
            {
                Assert.That(result.Count(), Is.EqualTo(1));
                var reference = result.Single();
                
                Assert.That(reference.Project, Is.SameAs(project));
            });
        }
        
        [Test]
        public void ParsePackageReferencesReturnNoReferencesWhenProjectFileContainsNoReference()
        {
            var project = Substitute.For<IProject>();
            
            

            project.File.Returns(File(@"TestData\ProjectWithNoPackageReferences.csproj"));
            
            Assert.That(ParsePackageReferences(project), Is.Empty);
        }
        
        [Test]
        public void ParsePackageReferencesReturnReferencesWhenProjectFileContainsImportReference()
        {
            var project = Substitute.For<IProject>();
            project.File.Returns(File(@"TestData\ProjectWithImportPackageReferences.csproj"));
            
            var references = ParsePackageReferences(project);
            
            Assert.Multiple(() =>
                {
                    Assert.That(references.Count(), Is.EqualTo(1));

                    var reference = references.Single();
                    Assert.That(reference.Id, Is.EqualTo("Some.Fake.Package"));
                    Assert.That(reference.Version, Is.EqualTo("3.2.1-beta0123"));
                }
            );
        }
        
        [Test]
        public void ParsePackageReferencesReturnReferencesWhenProjectFileContainsErrorReference()
        {
            var project = Substitute.For<IProject>();
            project.File.Returns(File(@"TestData\ProjectWithErrorPackageReferences.csproj"));
            
            var references = ParsePackageReferences(project);
            
            Assert.Multiple(() =>
                {
                    Assert.That(references.Count(), Is.EqualTo(1));

                    var reference = references.Single();
                    Assert.That(reference.Id, Is.EqualTo("Some.Fake.Package"));
                    Assert.That(reference.Version, Is.EqualTo("3.2.1"));
                }
            );
        }
        
        [Test]
        public void ParsePackageReferencesReturnReferencesWhenProjectFileContainsHinthPathReference()
        {
            var project = Substitute.For<IProject>();
            project.File.Returns(File(@"TestData\ProjectWithHintPathPackageReferences.csproj"));
            
            var references = ParsePackageReferences(project);
            
            Assert.Multiple(() =>
                {
                    Assert.That(references.Count(), Is.EqualTo(1));

                    var reference = references.Single();
                    Assert.That(reference.Id, Is.EqualTo("Some.Fake.Package"));
                    Assert.That(reference.Version, Is.EqualTo("3.2"));
                }
            );
        }
        
        [Test]
        public void ParsePackageReferencesReturnAllReferencesWhenProjectFileContainsMultipleReferences()
        {
            var project = Substitute.For<IProject>();
            project.File.Returns(File(@"TestData\ProjectWithMultiplePackageReferences.csproj"));
            
            var references = ParsePackageReferences(project);
            Assert.That(references.Count(), Is.EqualTo(3));
        }
        
        [Test]
        public void ParsePackageReferencesReturnAllTheRightPackageIdWhenVisualStudioTextTemplatingIsReferenced()
        {
            var project = Substitute.For<IProject>();
            project.File.Returns(File(@"TestData\ProjectWithMicrosoftVisualStudioTextTemplatingHintpathPackageReferences.csproj"));
            
            var references = ParsePackageReferences(project);
            Assert.Multiple(() =>
            {
                Assert.That(references.Count(), Is.EqualTo(4));
                Assert.That(
                    references.Any(r =>
                        r.Id.Equals("Microsoft.VisualStudio.TextTemplating.15.0") && r.Version.Equals("15.4.27004")),
                    Is.True);
                Assert.That(
                    references.Any(r =>
                        r.Id.Equals("Microsoft.VisualStudio.TextTemplating.Interfaces.10.0") && r.Version.Equals("10.0.30319")),
                    Is.True);
                Assert.That(
                    references.Any(r =>
                        r.Id.Equals("Microsoft.VisualStudio.TextTemplating.Interfaces.11.0") && r.Version.Equals("11.0.50727")),
                    Is.True);
                Assert.That(
                    references.Any(r =>
                        r.Id.Equals("Microsoft.VisualStudio.TextTemplating.Interfaces.15.0") && r.Version.Equals("15.4.27004")),
                    Is.True);
            });
        }
    }
}