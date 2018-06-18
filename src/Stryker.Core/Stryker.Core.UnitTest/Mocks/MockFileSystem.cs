//using System;
//using System.Collections.Generic;
//using System.IO.Abstractions;
//using System.Linq;

//namespace Stryker.Core.UnitTest.Mocks
//{
//    public class MockFileSystem : IFileSystem
//    {
//        private MockDirectory Root { get; set; }

//        public MockFileSystem(Dictionary<string, MockFile> files)
//        {
//            Root = new MockDirectory("C");

//        }

//        public FileBase File => new MockFileBase(Root);

//        public DirectoryBase Directory => new MockDirectoryBase(Root);

//        public IFileInfoFactory FileInfo => throw new NotImplementedException();

//        public PathBase Path => throw new NotImplementedException();

//        public IDirectoryInfoFactory DirectoryInfo => throw new NotImplementedException();

//        public IDriveInfoFactory DriveInfo => throw new NotImplementedException();
//    }
//}
