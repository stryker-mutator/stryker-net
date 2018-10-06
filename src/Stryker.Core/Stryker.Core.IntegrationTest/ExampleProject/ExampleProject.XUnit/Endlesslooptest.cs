using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace ExampleProject.XUnit
{
    public class Endlesslooptest
    {
        [Fact]
        public void Loop()
        {
            var target = new Endlessloop();
            target.SomeLoop();
        }
    }
}
