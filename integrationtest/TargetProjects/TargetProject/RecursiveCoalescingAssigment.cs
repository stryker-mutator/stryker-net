using System.Collections.Generic;

namespace ExampleProject
{
    public class Myclass
    {
        public void SwitchExpression()
        {
            List<int> numbersA = null;
            List<int> numbersB = null;
            numbersA ??= numbersB ??= new List<int>();
        }
    }
}
