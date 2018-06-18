using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Security.AccessControl;
using System.Text;

namespace Stryker.Core.UnitTest.Mocks
{
    public class MockDirectoryBase : DirectoryBase
    {
        public IList<MockDirectory> Root { get; set; }

        public MockDirectoryBase(IList<MockDirectory> root)
        {
            Root = root;
        }

        public override DirectoryInfoBase CreateDirectory(string path)
        {
            throw new NotImplementedException();
        }

        public override void Delete(string path)
        {
            throw new NotImplementedException();
        }

        public override void Delete(string path, bool recursive)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<string> EnumerateDirectories(string path)
        {
            return Root.Select(x => x.Path);
        }

        public override IEnumerable<string> EnumerateDirectories(string path, string searchPattern)
        {
            return Root.Where(x => x.Path.Contains(searchPattern)).Select(x => x.Path);
        }

        public override IEnumerable<string> EnumerateDirectories(string path, string searchPattern, SearchOption searchOption)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<string> EnumerateFiles(string path)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<string> EnumerateFiles(string path, string searchPattern)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<string> EnumerateFiles(string path, string searchPattern, SearchOption searchOption)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<string> EnumerateFileSystemEntries(string path)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<string> EnumerateFileSystemEntries(string path, string searchPattern)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<string> EnumerateFileSystemEntries(string path, string searchPattern, SearchOption searchOption)
        {
            throw new NotImplementedException();
        }

        public override bool Exists(string path)
        {
            throw new NotImplementedException();
        }

        public override DirectorySecurity GetAccessControl(string path)
        {
            throw new NotImplementedException();
        }

        public override DirectorySecurity GetAccessControl(string path, AccessControlSections includeSections)
        {
            throw new NotImplementedException();
        }

        public override DateTime GetCreationTime(string path)
        {
            throw new NotImplementedException();
        }

        public override DateTime GetCreationTimeUtc(string path)
        {
            throw new NotImplementedException();
        }

        public override string GetCurrentDirectory()
        {
            throw new NotImplementedException();
        }

        public override string[] GetDirectories(string path)
        {
            throw new NotImplementedException();
        }

        public override string[] GetDirectories(string path, string searchPattern)
        {
            throw new NotImplementedException();
        }

        public override string[] GetDirectories(string path, string searchPattern, SearchOption searchOption)
        {
            throw new NotImplementedException();
        }

        public override string GetDirectoryRoot(string path)
        {
            throw new NotImplementedException();
        }

        public override string[] GetFiles(string path)
        {
            throw new NotImplementedException();
        }

        public override string[] GetFiles(string path, string searchPattern)
        {
            throw new NotImplementedException();
        }

        public override string[] GetFiles(string path, string searchPattern, SearchOption searchOption)
        {
            throw new NotImplementedException();
        }

        public override string[] GetFileSystemEntries(string path)
        {
            throw new NotImplementedException();
        }

        public override string[] GetFileSystemEntries(string path, string searchPattern)
        {
            throw new NotImplementedException();
        }

        public override DateTime GetLastAccessTime(string path)
        {
            throw new NotImplementedException();
        }

        public override DateTime GetLastAccessTimeUtc(string path)
        {
            throw new NotImplementedException();
        }

        public override DateTime GetLastWriteTime(string path)
        {
            throw new NotImplementedException();
        }

        public override DateTime GetLastWriteTimeUtc(string path)
        {
            throw new NotImplementedException();
        }

        public override DirectoryInfoBase GetParent(string path)
        {
            throw new NotImplementedException();
        }

        public override void Move(string sourceDirName, string destDirName)
        {
            throw new NotImplementedException();
        }

        public override void SetAccessControl(string path, DirectorySecurity directorySecurity)
        {
            throw new NotImplementedException();
        }

        public override void SetCreationTime(string path, DateTime creationTime)
        {
            throw new NotImplementedException();
        }

        public override void SetCreationTimeUtc(string path, DateTime creationTimeUtc)
        {
            throw new NotImplementedException();
        }

        public override void SetCurrentDirectory(string path)
        {
            throw new NotImplementedException();
        }

        public override void SetLastAccessTime(string path, DateTime lastAccessTime)
        {
            throw new NotImplementedException();
        }

        public override void SetLastAccessTimeUtc(string path, DateTime lastAccessTimeUtc)
        {
            throw new NotImplementedException();
        }

        public override void SetLastWriteTime(string path, DateTime lastWriteTime)
        {
            throw new NotImplementedException();
        }

        public override void SetLastWriteTimeUtc(string path, DateTime lastWriteTimeUtc)
        {
            throw new NotImplementedException();
        }
    }
}
