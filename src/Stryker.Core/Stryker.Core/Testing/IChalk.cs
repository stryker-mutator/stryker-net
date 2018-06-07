using System;
using System.Collections.Generic;
using System.Text;

namespace Stryker.Core.Testing
{
    public interface IChalk
    {
        void Red(string text);
        void Yellow(string text);
        void Green(string text);
        void DarkGray(string text);
        void Default(string text);
    }
}
