using ExampleProject.String;
using Xunit;

namespace ExampleProject.XUnit.String
{
    public class Utf8StringMagicTests
    {
        [Fact]
        public void AddTwoStrings()
        {
            var sut = new Utf8StringMagic();
            var actual = sut.HelloWorld();
            Assert.Equal("Hello World!"u8, actual);
        }

        [Fact]
        public void IsNullOrEmpty()
        {
            var sut = new Utf8StringMagic();
            var actual = sut.IsNullOrEmpty("hello"u8);
            Assert.False(actual);
        }

        [Fact]
        public void IsNullOrEmpty_Empty()
        {
            var sut = new Utf8StringMagic();
            var actual = sut.IsNullOrEmpty(""u8);
            Assert.True(actual);
        }

        [Fact]
        public void Referenced()
        {
            var sut = new Utf8StringMagic();
            var input = "hello"u8;
            sut.Referenced(out input);
            Assert.Equal("world"u8, input);
        }

        [Fact]
        public void ReferencedEmpty()
        {
            var sut = new Utf8StringMagic();
            var input = "hello"u8;
            sut.ReferencedEmpty(out input);
            Assert.Equal(""u8, input);
        }
    }
}
