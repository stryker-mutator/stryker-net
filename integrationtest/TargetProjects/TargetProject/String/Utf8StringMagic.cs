using System;
using System.Linq;

namespace ExampleProject.String
{
    public class Utf8StringMagic
    {
        public ReadOnlySpan<byte> HelloWorld()
        {
            return "Hello"u8 + " "u8 + "World!"u8;
        }

        public void Referenced(out ReadOnlySpan<byte> test)
        {
            test = "world"u8;
        }

        public void ReferencedEmpty(out ReadOnlySpan<byte> test)
        {
            test = ""u8;
        }

        public bool IsNullOrEmpty(ReadOnlySpan<byte> myString)
        {
            if (myString.IsEmpty)
            {
                return true;
            }
            return false;
        }
    }
}
