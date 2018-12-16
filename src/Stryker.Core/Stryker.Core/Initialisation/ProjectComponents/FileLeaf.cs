﻿using Stryker.Core.Mutants;
using System;
using System.Collections.Generic;

namespace Stryker.Core.Initialisation.ProjectComponent
{
    public class FileLeaf : ProjectComponent
    {
        public string SourceCode { get; set; }
        public string FullPath { get; set; }

        private IEnumerable<Mutant> _mutants { get; set; }
        public override IEnumerable<Mutant> Mutants {
            get => _mutants;
            set => _mutants = value;
        }

        public override void Display(int depth)
        {
            DisplayFile(depth, this);
        }

        public override void Add(ProjectComponent component)
        {
            // no children can be added to a file instance
            throw new NotImplementedException();
        }

        public override IEnumerable<FileLeaf> GetAllFiles()
        {
            yield return this;
        }
    }
}
