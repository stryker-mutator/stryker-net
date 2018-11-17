﻿using System;
using System.Collections.Generic;
using System.Text;

namespace StrykerNet.UnitTest.Mutants.TestResources
{
    public class TestClass
    {
        void TestMethod()
        {
            int i = 0;
            if (Stryker.Environment.ActiveMutation==0)
            {
                i = i - 1;
            } else {
                i = i + 1;
            }
            if (Stryker.Environment.ActiveMutation==2)
            {
                string SomeLocalFunction()
                {
                    var test3 = 2 + 5;
                    return $"test{1 - test3}";
                }
            }
            else
            {
                if (Stryker.Environment.ActiveMutation==1)
                {
                    string SomeLocalFunction()
                    {
                        var test3 = 2 - 5;
                        return $"test{1 + test3}";
                    }
                }
                else
                {
                    string SomeLocalFunction()
                    {
                        var test3 = 2 + 5;
                        return $"test{1 + test3}";
                    }
                }
            };
            Console.WriteLine(SomeLocalFunction());
        }
    }
}
