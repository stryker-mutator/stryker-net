using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Security.AccessControl;
using System.Text;

namespace Stryker.Core.UnitTest.Mocks
{
    public class MockFileBase : FileBase
    {
        public string Content { get; set; }
        public string Path { get; set; }

        public MockFileBase(string path, string content)
        {
            Path = path;
            Content = content;
        }

        public override void AppendAllLines(string path, IEnumerable<string> contents)
        {
            throw new NotImplementedException();
        }

        public override void AppendAllLines(string path, IEnumerable<string> contents, Encoding encoding)
        {
            throw new NotImplementedException();
        }

        public override void AppendAllText(string path, string contents)
        {
            throw new NotImplementedException();
        }

        public override void AppendAllText(string path, string contents, Encoding encoding)
        {
            throw new NotImplementedException();
        }

        public override StreamWriter AppendText(string path)
        {
            throw new NotImplementedException();
        }

        public override void Copy(string sourceFileName, string destFileName)
        {
            throw new NotImplementedException();
        }

        public override void Copy(string sourceFileName, string destFileName, bool overwrite)
        {
            throw new NotImplementedException();
        }

        public override Stream Create(string path)
        {
            throw new NotImplementedException();
        }

        public override Stream Create(string path, int bufferSize)
        {
            throw new NotImplementedException();
        }

        public override Stream Create(string path, int bufferSize, FileOptions options)
        {
            throw new NotImplementedException();
        }

        public override StreamWriter CreateText(string path)
        {
            throw new NotImplementedException();
        }

        public override void Delete(string path)
        {
            throw new NotImplementedException();
        }

        public override bool Exists(string path)
        {
            throw new NotImplementedException();
        }

        public override FileSecurity GetAccessControl(string path)
        {
            throw new NotImplementedException();
        }

        public override FileSecurity GetAccessControl(string path, AccessControlSections includeSections)
        {
            throw new NotImplementedException();
        }

        public override FileAttributes GetAttributes(string path)
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

        public override void Move(string sourceFileName, string destFileName)
        {
            throw new NotImplementedException();
        }

        public override Stream Open(string path, FileMode mode)
        {
            throw new NotImplementedException();
        }

        public override Stream Open(string path, FileMode mode, FileAccess access)
        {
            throw new NotImplementedException();
        }

        public override Stream Open(string path, FileMode mode, FileAccess access, FileShare share)
        {
            throw new NotImplementedException();
        }

        public override Stream OpenRead(string path)
        {
            throw new NotImplementedException();
        }

        public override StreamReader OpenText(string path)
        {
            throw new NotImplementedException();
        }

        public override Stream OpenWrite(string path)
        {
            throw new NotImplementedException();
        }

        public override byte[] ReadAllBytes(string path)
        {
            throw new NotImplementedException();
        }

        public override string[] ReadAllLines(string path)
        {
            throw new NotImplementedException();
        }

        public override string[] ReadAllLines(string path, Encoding encoding)
        {
            throw new NotImplementedException();
        }

        public override string ReadAllText(string path)
        {
            throw new NotImplementedException();
        }

        public override string ReadAllText(string path, Encoding encoding)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<string> ReadLines(string path)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<string> ReadLines(string path, Encoding encoding)
        {
            throw new NotImplementedException();
        }

        public override void SetAccessControl(string path, FileSecurity fileSecurity)
        {
            throw new NotImplementedException();
        }

        public override void SetAttributes(string path, FileAttributes fileAttributes)
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

        public override void WriteAllBytes(string path, byte[] bytes)
        {
            throw new NotImplementedException();
        }

        public override void WriteAllLines(string path, IEnumerable<string> contents)
        {
            throw new NotImplementedException();
        }

        public override void WriteAllLines(string path, IEnumerable<string> contents, Encoding encoding)
        {
            throw new NotImplementedException();
        }

        public override void WriteAllLines(string path, string[] contents)
        {
            throw new NotImplementedException();
        }

        public override void WriteAllLines(string path, string[] contents, Encoding encoding)
        {
            throw new NotImplementedException();
        }

        public override void WriteAllText(string path, string contents)
        {
            throw new NotImplementedException();
        }

        public override void WriteAllText(string path, string contents, Encoding encoding)
        {
            throw new NotImplementedException();
        }
    }
}
