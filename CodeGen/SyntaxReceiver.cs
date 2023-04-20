using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiniContainer.CodeGen
{
    public class SyntaxReceiver : ISyntaxReceiver
    {
        public readonly List<ConstructorDeclarationSyntax> Constructors
            = new List<ConstructorDeclarationSyntax>();
        public readonly List<TypeDeclarationSyntax> DefaultConstructorTypes
            = new List<TypeDeclarationSyntax>();

        public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
        {
            if (syntaxNode is TypeDeclarationSyntax typeDeclarationSyntax)
            {
                if (!typeDeclarationSyntax.Keyword.IsKind(SyntaxKind.ClassKeyword)
                    && !typeDeclarationSyntax.Keyword.IsKind(SyntaxKind.StructKeyword))
                    return;
                if (typeDeclarationSyntax.Modifiers.Any(SyntaxKind.AbstractKeyword))
                    return;
                if (typeDeclarationSyntax.Modifiers.Any(SyntaxKind.StaticKeyword))
                    return;
                if (!typeDeclarationSyntax.Modifiers.Any(SyntaxKind.PublicKeyword))
                    return;
                if (IsMonoBehaviour(typeDeclarationSyntax))
                    return;
                if (IsGenericType(typeDeclarationSyntax))
                    return;
                var constructorDeclarationSyntax
                    = GetConstructorForInjection(typeDeclarationSyntax, out var defaultPrivateConstructorExists);
                if (constructorDeclarationSyntax != null)
                    Constructors.Add(constructorDeclarationSyntax);
                else if (!defaultPrivateConstructorExists)
                    DefaultConstructorTypes.Add(typeDeclarationSyntax);
            }
        }

        private static ConstructorDeclarationSyntax GetConstructorForInjection(TypeDeclarationSyntax typeDeclarationSyntax, out bool defaultPrivateConstructorExists)
        {
            ConstructorDeclarationSyntax resultConstructor = null;
            defaultPrivateConstructorExists = false;
            foreach (var syntaxNode in typeDeclarationSyntax.ChildNodes())
            {
                if (!(syntaxNode is ConstructorDeclarationSyntax candidateConstructor))
                    continue;
                if (candidateConstructor.ParameterList.Parameters.Count == 0
                    && !candidateConstructor.Modifiers.Any(SyntaxKind.PublicKeyword))
                {
                    defaultPrivateConstructorExists = true;
                }
                if (!candidateConstructor.Modifiers.Any(SyntaxKind.PublicKeyword))
                    continue;
                if (resultConstructor == null
                    || resultConstructor.ParameterList.Parameters.Count < candidateConstructor.ParameterList.Parameters.Count)
                    resultConstructor = candidateConstructor;
            }
            return resultConstructor;
        }

        private static bool IsMonoBehaviour(TypeDeclarationSyntax typeDeclarationSyntax)
        {
            if (typeDeclarationSyntax.BaseList == null)
                return false;
            foreach (var baseTypeSyntax in typeDeclarationSyntax.BaseList.Types)
                if (baseTypeSyntax.Type.ToString() == "MonoBehaviour")
                    return true;
            return false;
        }

        private static bool IsGenericType(TypeDeclarationSyntax typeSyntax)
        {
            return typeSyntax.TypeParameterList?.Parameters.Count > 0;
        }
    }
}
