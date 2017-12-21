using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using DevelopmentProcessValidation.Abstractions;
using VisualStudio.Files.Abstractions;

namespace DevelopmentProcessValidation.Validations
{
    public class ProjectFileParser : IProjectFileParser
    {
        private const string PackageReferencePattern = @"packages\\(?<id>.+?(\.(\d+\.0))?)\.(?<version>(\d+\.)+\d+(-.+?)?)\\";
        
        public IEnumerable<IProjectFilePackageReference> ParsePackageReferences(IProject project)
        {
            if(project == null) {throw new ArgumentNullException(nameof(project));}
            var regex = new Regex(PackageReferencePattern);
            var xml = XElement.Load(project.File.FullName);

            var elementsToCheck = xml.Descendants().Where(e => e.Name.LocalName.Equals("Import") || e.Name.LocalName.Equals("Error") || e.Name.LocalName.Equals("HintPath"));

            foreach (var element in elementsToCheck)
            {
                var match = regex.Match(element.ToString());
                if (match.Success)
                {
                    yield return new ProjectFilePackageReference
                    {
                        Project = project,
                        Id = match.Groups["id"].Value,
                        Version = match.Groups["version"].Value,
                        Element = element.ToString()
                    };
                }
            }  
        }
    }
}