//using Microsoft.CodeAnalysis;
//using Microsoft.CodeAnalysis.CSharp.Syntax;

//namespace Stryker.Core.Mutants.MutationHandlers
//{
//    public abstract class MutationHandler
//    {
//        protected MutationHandler _successor;

//        public MutationHandler SetSuccessor(MutationHandler successor)
//        {
//            _successor = successor;
//            return this;
//        }

//        public abstract SyntaxNode HandleInsertMutation(StatementSyntax original, StatementSyntax mutated, int mutantId);
//        public abstract SyntaxNode HandleRemoveMutation(SyntaxNode node);
//    }
//}
