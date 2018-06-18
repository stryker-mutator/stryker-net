using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Stryker.Core.UnitTest.Mocks
{
    public class MockDirectory
    {
        public IList<MockDirectory> SubDirectories { get; set; }
        public IList<MockFile> Files { get; set; }
        public string Path { get; set; }

        public MockDirectory(string path)
        {
            Path = path;
        }

        public IEnumerable<MockDirectory> GetAllSubDirectories()
        {
            return SubDirectories.SelectMany(x => GetAllSubDirectories());
        }

        public IEnumerable<MockFileBase> GetAllSubFiles()
        {
            return GetAllSubDirectories().SelectMany(x => x.GetAllSubFiles());
        }
    }
}
