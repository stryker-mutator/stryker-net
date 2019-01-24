using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace ExampleProject.XUnit
{
    public class StringMagicTest
    {
        [Fact]
        void DemoShouldReturnConst()
        {
            Assert.True(StringMagic.Demo(out var val));
            Assert.Equal("toto", val);
        }

        [Fact]
        void AddShouldBeStrange()
        {
            Assert.Equal("abcd", StringMagic.AddTwoStrings("cd", "ab"));
            Assert.Equal("abcd", StringMagic.AddTwoStrings("abc", "d"));
        }
    }
}
