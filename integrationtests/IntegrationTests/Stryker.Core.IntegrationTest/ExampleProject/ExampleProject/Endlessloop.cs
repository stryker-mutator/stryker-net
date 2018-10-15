using System;
using System.Collections.Generic;
using System.Text;

namespace ExampleProject
{
    public class EndlessLoop
    {
        public void SomeLoop()
        {
            while (1 < 0)
            {
                ;
            }
        }
    }
}
