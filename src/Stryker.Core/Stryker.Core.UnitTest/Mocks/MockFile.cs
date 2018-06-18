using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Security.AccessControl;
using System.Text;

namespace Stryker.Core.UnitTest.Mocks
{
    public class MockFile
    {
        public string Content { get; set; }
        public string Path { get; set; }

        public MockFile(string path, string content)
        {
            Path = path;
            Content = content;
        }
    }
}
