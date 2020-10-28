using System;
using System.Collections.Generic;
using System.Text;
using Stryker.Core.ProjectComponents;

namespace Stryker.Core.Initialisation
{
    public interface IProjectComponentsBuilder
    {
        IProjectComponent Build();
    }
}
