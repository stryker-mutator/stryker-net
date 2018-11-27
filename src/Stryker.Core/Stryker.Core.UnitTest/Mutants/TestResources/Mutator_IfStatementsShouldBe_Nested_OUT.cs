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
            if (Stryker.ActiveMutationHelper.ActiveMutation==6)
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
                if (Stryker.ActiveMutationHelper.ActiveMutation==5)
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
                    if (Stryker.ActiveMutationHelper.ActiveMutation==4)
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
                        if (Stryker.ActiveMutationHelper.ActiveMutation==3)
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
                            if (Stryker.ActiveMutationHelper.ActiveMutation==2)
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
                                if (Stryker.ActiveMutationHelper.ActiveMutation==1)
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
                                    if (Stryker.ActiveMutationHelper.ActiveMutation==0)
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
            if (Stryker.ActiveMutationHelper.ActiveMutation==7)
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
