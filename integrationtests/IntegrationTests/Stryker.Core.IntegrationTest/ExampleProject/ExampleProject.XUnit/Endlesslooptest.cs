using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace ExampleProject.XUnit
{
    public class EndlessLoopTest
    {
        [Fact]
        public void Loop()
        {
            var target = new EndlessLoop();
            target.SomeLoop();
        }
    }
}
