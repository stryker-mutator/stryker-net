using System;
using System.Collections.Generic;
using System.Text;

namespace StrykerNet.UnitTest.Mutants.TestResources
{
    class TestClass
    {
        void TestMethod()
        {
            int i = 0;
            if (Stryker.Environment.ID==6)
                    {
                if (i + 8 == 8)
                {
                    i = i + 1;
                    if (i + 8 == 9)
                    {
                        i = i + 1;
                    };
                }
                else
                {
                    i = i + 3;
                    if (i == i + i - 8)
                    {
                        i = i - 1;
                    };
                }
            }
            else
            {
                if (Stryker.Environment.ID==5)
                        {
                    if (i + 8 == 8)
                    {
                        i = i + 1;
                        if (i + 8 == 9)
                        {
                            i = i + 1;
                        };
                    }
                    else
                    {
                        i = i + 3;
                        if (i == i - i - 8)
                        {
                            i = i + 1;
                        };
                    }
                }
                else
                {
                    if (Stryker.Environment.ID==4)
                            {
                        if (i + 8 == 8)
                        {
                            i = i + 1;
                            if (i + 8 == 9)
                            {
                                i = i + 1;
                            };
                        }
                        else
                        {
                            i = i - 3;
                            if (i == i + i - 8)
                            {
                                i = i + 1;
                            };
                        }
                    }
                    else
                    {
                        if (Stryker.Environment.ID==3)
                                {
                            if (i + 8 == 8)
                            {
                                i = i + 1;
                                if (i + 8 == 9)
                                {
                                    i = i - 1;
                                };
                            }
                            else
                            {
                                i = i + 3;
                                if (i == i + i - 8)
                                {
                                    i = i + 1;
                                };
                            }
                        }
                        else
                        {
                            if (Stryker.Environment.ID==2)
                                    {
                                if (i + 8 == 8)
                                {
                                    i = i + 1;
                                    if (i - 8 == 9)
                                    {
                                        i = i + 1;
                                    };
                                }
                                else
                                {
                                    i = i + 3;
                                    if (i == i + i - 8)
                                    {
                                        i = i + 1;
                                    };
                                }
                            }
                            else
                            {
                                if (Stryker.Environment.ID==1)
                                        {
                                    if (i + 8 == 8)
                                    {
                                        i = i - 1;
                                        if (i + 8 == 9)
                                        {
                                            i = i + 1;
                                        };
                                    }
                                    else
                                    {
                                        i = i + 3;
                                        if (i == i + i - 8)
                                        {
                                            i = i + 1;
                                        };
                                    }
                                }
                                else
                                {
                                    if (Stryker.Environment.ID==0)
                                            {
                                        if (i - 8 == 8)
                                        {
                                            i = i + 1;
                                            if (i + 8 == 9)
                                            {
                                                i = i + 1;
                                            };
                                        }
                                        else
                                        {
                                            i = i + 3;
                                            if (i == i + i - 8)
                                            {
                                                i = i + 1;
                                            };
                                        }
                                    }
                                    else
                                    {
                                        if (i + 8 == 8)
                                        {
                                            i = i + 1;
                                            if (i + 8 == 9)
                                            {
                                                i = i + 1;
                                            };
                                        }
                                        else
                                        {
                                            i = i + 3;
                                            if (i == i + i - 8)
                                            {
                                                i = i + 1;
                                            };
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            if (Stryker.Environment.ID==7)
                    {
                i = i - i;
            }
            else
            {
                i = i + i;
            }
        }
    }
}
