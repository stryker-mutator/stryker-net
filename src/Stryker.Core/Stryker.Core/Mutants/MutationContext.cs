// --------------------------------------------------------------------------------------------------------------------
//  <copyright file="MutationContext.cs" company="Stryker-Mutator.Net">
//   Copyright 2019 Cyrille DUPUYDAUBY
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//       http://www.apache.org/licenses/LICENSE-2.0
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using CSharpExtensions = Microsoft.CodeAnalysis.CSharp.CSharpExtensions;

namespace Stryker.Core.Mutants
{
    internal class MutationContext
    {
        public bool InStaticValue { get; set; }

        public MutationContext UpdateContext(SyntaxNode node)
        {
            switch (node)
            {
                case FieldDeclarationSyntax fieldDeclaration:
                    if (fieldDeclaration.Modifiers.Any(x => CSharpExtensions.Kind((SyntaxToken) x) == SyntaxKind.StaticKeyword))
                    {
                        return new MutationContext {InStaticValue = true};
                    }
                    break;
                case ConstructorDeclarationSyntax constructorDeclaration:
                    if (constructorDeclaration.Modifiers.Any(x => x.Kind() == SyntaxKind.StaticKeyword))
                    {
                        return new MutationContext {InStaticValue = true};
                    }
                    break;
            }

            return this;
        }
    }
}