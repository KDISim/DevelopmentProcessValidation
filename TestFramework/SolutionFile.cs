using System;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace DevelopmentProcessValidation.TestFramework
{
    internal static class SolutionFile
    {
        internal static FileInfo Locate()
        {
            var info = new DirectoryInfo(TestContext.CurrentContext.TestDirectory);
            return Locate(info);
        }
        
        internal static FileInfo Locate(DirectoryInfo directoryInfo)
        {
            var slnFiles = directoryInfo.EnumerateFiles("*.sln");

            if (!slnFiles.Any())
            {
                return Locate(directoryInfo.Parent);
            }
            
            if (slnFiles.Count() > 1)
            {
                throw new NotSupportedException();
            }

            return slnFiles.Single();
        }
    }
}