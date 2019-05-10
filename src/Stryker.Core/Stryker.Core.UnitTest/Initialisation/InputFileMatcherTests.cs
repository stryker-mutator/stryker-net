using DotNet.Globbing;
using Shouldly;
using Stryker.Core.Initialisation;
using Stryker.Core.Options;
using Stryker.Core.ProjectComponents;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Xunit;

namespace Stryker.Core.UnitTest.Initialisation
{
    public class InputFileMatcherTests
    {
        [Fact]
        public void ShouldMatchExcludedFile()
        {
            var target = new InputFileMatcher();

            var root = new FolderComposite
            {
                Name = "ProjectRoot",
                Children = new Collection<ProjectComponent>
                {
                    new FileLeaf
                    {
                        Name = "SomeFile.cs",
                        RelativePath = "ProjectRoot/SomeFile.cs"
                    }
                }
            };

            var matcher = new List<PathOption>() { new PathOption { Exclude = true, Matcher = Glob.Parse("ProjectRoot/SomeFile.cs") } };

            target.MatchInputFiles(root, matcher, "ProjectRoot");

            root.ExcludedFiles.Count().ShouldBe(1);
        }

        [Fact]
        public void ShouldMatchExcludedFileUsingGlob()
        {
            var target = new InputFileMatcher();

            var root = new FolderComposite
            {
                Name = "ProjectRoot",
                Children = new Collection<ProjectComponent>
                {
                    new FileLeaf
                    {
                        Name = "SomeFile.cs",
                        RelativePath = "ProjectRoot/SomeFile.cs"
                    }
                }
            };

            var matcher = new List<PathOption>() { new PathOption { Exclude = true, Matcher = Glob.Parse("ProjectRoot/*.cs") } };

            target.MatchInputFiles(root, matcher, "ProjectRoot");

            root.ExcludedFiles.Count().ShouldBe(1);
        }

        [Fact]
        public void ShouldMatchIncludedFile()
        {
            var target = new InputFileMatcher();

            var root = new FolderComposite
            {
                Name = "ProjectRoot",
                Children = new Collection<ProjectComponent>
                {
                    new FileLeaf
                    {
                        Name = "SomeFile.cs",
                        RelativePath = "ProjectRoot/SomeFile.cs"
                    },
                    new FileLeaf
                    {
                        Name = "SomeFile.cs",
                        RelativePath = "ProjectRoot/SomeOtherFile.cs"
                    }
                }
            };

            var matcher = new List<PathOption>() { new PathOption { Exclude = false, Matcher = Glob.Parse("ProjectRoot/SomeFile.cs") } };

            target.MatchInputFiles(root, matcher, "ProjectRoot");

            root.ExcludedFiles.Count().ShouldBe(1);
        }

        [Fact]
        public void ShouldMatchIncludedFileRecursive()
        {
            var target = new InputFileMatcher();

            var root = new FolderComposite
            {
                Name = "ProjectRoot",
                Children = new Collection<ProjectComponent>
                {
                    new FileLeaf
                    {
                        Name = "SomeFile.cs",
                        RelativePath = "ProjectRoot/SomeFile.cs"
                    },
                    new FileLeaf
                    {
                        Name = "SomeFile.cs",
                        RelativePath = "ProjectRoot/SomeOtherFile.cs"
                    },
                    new FolderComposite
                    {
                        Name = "TestDir",
                        Children = new Collection<ProjectComponent>
                        {
                            new FileLeaf
                            {
                                Name = "SomeFile.cs",
                                RelativePath = "ProjectRoot/TestDir/SomeFile.cs"
                            },
                        }
                    }
                }
            };

            var matcher = new List<PathOption>() { new PathOption { Exclude = false, Matcher = Glob.Parse("ProjectRoot/SomeFile.cs") } };

            target.MatchInputFiles(root, matcher, "ProjectRoot");

            root.ExcludedFiles.Count().ShouldBe(2);
        }

        [Fact]
        public void ShouldMatchExcludedFileRecursiveWithInclude()
        {
            var target = new InputFileMatcher();

            var root = new FolderComposite
            {
                Name = "ProjectRoot",
                Children = new Collection<ProjectComponent>
                {
                    new FileLeaf
                    {
                        Name = "SomeFile.cs",
                        RelativePath = "ProjectRoot/SomeFile.cs"
                    },
                    new FileLeaf
                    {
                        Name = "SomeFile.cs",
                        RelativePath = "ProjectRoot/SomeOtherFile.cs"
                    },
                    new FolderComposite
                    {
                        Name = "TestDir",
                        Children = new Collection<ProjectComponent>
                        {
                            new FileLeaf
                            {
                                Name = "SomeFile.cs",
                                RelativePath = "ProjectRoot/TestDir/SomeFile.cs"
                            },
                            new FileLeaf
                            {
                                Name = "SomeOtherFile.cs",
                                RelativePath = "ProjectRoot/TestDir/SomeOtherFile.cs"
                            }
                        }
                    }
                }
            };

            var matcher = new List<PathOption>() {
                new PathOption { Exclude = true, Matcher = Glob.Parse("ProjectRoot/TestDir/**/*.cs") },
                new PathOption { Exclude = false, Matcher = Glob.Parse("ProjectRoot/TestDir/SomeFile.cs") }
            };

            target.MatchInputFiles(root, matcher, "ProjectRoot");

            root.ExcludedFiles.Count().ShouldBe(1);
        }
    }
}
