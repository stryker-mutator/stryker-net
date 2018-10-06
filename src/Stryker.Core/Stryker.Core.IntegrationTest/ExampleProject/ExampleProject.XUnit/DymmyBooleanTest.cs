using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace ExampleProject.XUnit
{
    public class DymmyBooleanTest
    {
        [Fact]
        public void IsTrue_ShouldReturnTrue()
        {
            var sut = new DummyBoolean();

            Assert.True(sut.IsTrue());
        }

        [Fact]
        public void IsFalse_ShouldReturnFalse()
        {
            var sut = new DummyBoolean();

            Assert.False(sut.IsFalse());
        }
    }
}
