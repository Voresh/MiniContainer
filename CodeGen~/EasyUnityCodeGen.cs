using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

[Generator]
public class EasyUnityCodeGen : ISourceGenerator {
    private class SyntaxReceiver : ISyntaxReceiver {
        public List<ConstructorDeclarationSyntax> Constructors = new List<ConstructorDeclarationSyntax>();
        public List<TypeDeclarationSyntax> DefaultConstructorTypes = new List<TypeDeclarationSyntax>();

        public void OnVisitSyntaxNode(SyntaxNode syntaxNode) {
            if (syntaxNode is TypeDeclarationSyntax typeDeclarationSyntax) {
                if (!typeDeclarationSyntax.Keyword.IsKind(SyntaxKind.ClassKeyword))
                    return;
                if (typeDeclarationSyntax.Modifiers.Any(SyntaxKind.AbstractKeyword))
                    return;
                if (typeDeclarationSyntax.Modifiers.Any(SyntaxKind.StaticKeyword))
                    return;
                if (typeDeclarationSyntax.Modifiers.Any(SyntaxKind.PrivateKeyword))
                    return;
                if (typeDeclarationSyntax.Modifiers.Any(SyntaxKind.ProtectedKeyword))
                    return;
                if (!typeDeclarationSyntax.Modifiers.Any(SyntaxKind.PublicKeyword))
                    return;
                var constructorDeclarationSyntax = typeDeclarationSyntax
                    .ChildNodes()
                    .OfType<ConstructorDeclarationSyntax>()
                    .Where(_ => _.Modifiers.Any(SyntaxKind.PublicKeyword))
                    .OrderByDescending(_ => _.ParameterList.Parameters.Count)
                    .FirstOrDefault();
                if (constructorDeclarationSyntax != null) {
                    Constructors.Add(constructorDeclarationSyntax);
                }
                else {
                    var noDefaultPrivateConstructor = typeDeclarationSyntax
                        .ChildNodes()
                        .OfType<ConstructorDeclarationSyntax>()
                        .All(_ => _.ParameterList.Parameters.Count != 0 || _.Modifiers.Any(SyntaxKind.PublicKeyword));
                    if (noDefaultPrivateConstructor)
                        DefaultConstructorTypes.Add(typeDeclarationSyntax);
                }
            }
        }
    }

    public void Execute(GeneratorExecutionContext context) {
        bool TypeIsMonoBehaviour(TypeDeclarationSyntax typeDeclarationSyntax) {
            var semanticModel = context.Compilation.GetSemanticModel(typeDeclarationSyntax.SyntaxTree);
            var fieldSymbol = semanticModel.GetDeclaredSymbol(typeDeclarationSyntax) as ITypeSymbol;
            if (fieldSymbol != null && fieldSymbol.BaseType != null) {
                var baseType = fieldSymbol.BaseType;
                while (baseType != null) {
                    if (baseType.Name == "MonoBehaviour")
                        return true;
                    baseType = baseType.BaseType;
                }
            }
            return false;
        }

        INamespaceSymbol GetNamespaceSymbol(ITypeSymbol typeSymbol) {
            var nextTypeSymbol = typeSymbol;
            var namespaceSymbol = nextTypeSymbol.ContainingNamespace;
            while (namespaceSymbol == null) {
                nextTypeSymbol = typeSymbol.ContainingType;
                if (nextTypeSymbol == null)
                    break;
                namespaceSymbol = nextTypeSymbol.ContainingNamespace;
            }
            return namespaceSymbol;
        }

        string GetTypeFullNamespace(TypeDeclarationSyntax typeDeclarationSyntax) {
            var semanticModel
                = context.Compilation.GetSemanticModel(typeDeclarationSyntax.SyntaxTree);
            var typeSymbol
                = semanticModel.GetDeclaredSymbol(typeDeclarationSyntax) as ITypeSymbol;
            if (typeSymbol == null)
                return null;
            var namespaceSymbol = GetNamespaceSymbol(typeSymbol);
            if (namespaceSymbol == null)
                return null;
            return GetFullNamespaceFromSymbol(namespaceSymbol);
        }

        string GetParameterName(ParameterSyntax parameterSyntax) {
            var semanticModel
                = context.Compilation.GetSemanticModel(parameterSyntax.SyntaxTree);
            var parameterSymbol
                = semanticModel.GetDeclaredSymbol(parameterSyntax) as IParameterSymbol;
            if (parameterSymbol == null)
                return null;
            var typeSymbol = parameterSymbol.Type;
            var namedTypeSymbol = typeSymbol as INamedTypeSymbol;
            var name = typeSymbol.Name;
            if (namedTypeSymbol != null) {
                var genericText = string.Empty;
                for (var index = 0; index < namedTypeSymbol.TypeArguments.Length; index++) {
                    if (index == 0)
                        genericText += "<";
                    var genericTypeSymbol = namedTypeSymbol.TypeArguments[index];
                    genericText += genericTypeSymbol.Name;
                    if (index == namedTypeSymbol.TypeArguments.Length - 1)
                        genericText += ">";
                    else
                        genericText += ", ";
                }
                name += genericText;
            }
            return name;
        }

        string GetParameterFullNamespace(ParameterSyntax parameterSyntax) {
            var semanticModel
                = context.Compilation.GetSemanticModel(parameterSyntax.SyntaxTree);
            var parameterSymbol
                = semanticModel.GetDeclaredSymbol(parameterSyntax) as IParameterSymbol;
            if (parameterSymbol == null)
                return null;
            var namespaceSymbol = GetNamespaceSymbol(parameterSymbol.Type);
            if (namespaceSymbol == null)
                return null;
            return GetFullNamespaceFromSymbol(namespaceSymbol);
        }

        string GetFullNamespaceFromSymbol(INamespaceSymbol namespaceSymbol) {
            var stringBuilder = new StringBuilder();
            var nextNamespaceSymbol = namespaceSymbol;
            stringBuilder.Clear();
            while (nextNamespaceSymbol != null) {
                if (!string.IsNullOrEmpty(nextNamespaceSymbol.Name)) {
                    if (!SymbolEqualityComparer.Default.Equals(nextNamespaceSymbol, namespaceSymbol)) {
                        stringBuilder.Insert(0, '.');
                    }
                    stringBuilder.Insert(0, nextNamespaceSymbol.Name);
                }
                nextNamespaceSymbol = nextNamespaceSymbol.ContainingNamespace;
            }
            return stringBuilder.ToString();
        }

        string GetFullTypeDeclarationSyntaxIdentifier(TypeDeclarationSyntax typeDeclarationSyntax) {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append(typeDeclarationSyntax.Identifier);
            var nextTypeDeclarationSyntax = typeDeclarationSyntax;
            while (nextTypeDeclarationSyntax.Parent is TypeDeclarationSyntax) {
                nextTypeDeclarationSyntax = (TypeDeclarationSyntax)nextTypeDeclarationSyntax.Parent;
                stringBuilder.Insert(0, '.');
                stringBuilder.Insert(0, nextTypeDeclarationSyntax.Identifier);
            }
            return stringBuilder.ToString();
        }

        if (!(context.SyntaxReceiver is SyntaxReceiver syntaxReceiver))
            return;
        var instanceConstructorType 
            = context.Compilation.GetTypeByMetadataName("EasyUnity.InstanceConstructors.InstanceConstructor");
        if (instanceConstructorType == null)
            return; // di framework not referenced
        var sourceModuleName = context.Compilation.SourceModule.Name;
        if (sourceModuleName.StartsWith("UnityEngine.") 
            || sourceModuleName.StartsWith("UnityEditor.") 
            || sourceModuleName.StartsWith("Unity."))
            return;
        var className = $"{context.Compilation.AssemblyName.Replace("-", string.Empty).Replace(".", string.Empty)}_GeneratedInstanceConstructor";
        //filter
        syntaxReceiver.Constructors.RemoveAll(_ => {
            var type = (TypeDeclarationSyntax)_.Parent;
            if (TypeIsMonoBehaviour(type))
                return true; //no MonoBehaviours
            var semanticModel
                = context.Compilation.GetSemanticModel(type.SyntaxTree);
            var typeSymbol
                = semanticModel.GetDeclaredSymbol(type);
            return typeSymbol != null && typeSymbol.IsGenericType; //no generic types
        });
        syntaxReceiver.DefaultConstructorTypes.RemoveAll(_ => {
            if (TypeIsMonoBehaviour(_))
                return true; //no MonoBehaviours
            var semanticModel
                = context.Compilation.GetSemanticModel(_.SyntaxTree);
            var typeSymbol
                = semanticModel.GetDeclaredSymbol(_);
            return typeSymbol != null && typeSymbol.IsGenericType; //no generic types
        });
        //filter
        var sourceBuilder = new StringBuilder(
            @"using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;");
        var addedNamespaces = new HashSet<string> {
            "System",
            "System.Collections.Generic",
            "System.Runtime.CompilerServices"
        };
        foreach (var constructorDeclarationSyntax in syntaxReceiver.Constructors) {
            var typeDeclarationSyntax = constructorDeclarationSyntax.Parent as TypeDeclarationSyntax;
            if (typeDeclarationSyntax == null)
                continue;
            var parameterStringBuilder = new StringBuilder();
            foreach (var parameterSyntax in constructorDeclarationSyntax.ParameterList.Parameters) {
                var namespaceString = GetParameterFullNamespace(parameterSyntax);
                if (string.IsNullOrEmpty(namespaceString))
                    continue;
                if (addedNamespaces.Contains(namespaceString))
                    continue;
                sourceBuilder.Append('\n');
                sourceBuilder.Append("using ");
                sourceBuilder.Append(namespaceString);
                sourceBuilder.Append(';');
                addedNamespaces.Add(namespaceString);
            }
            var namespaceDeclarationSyntax = typeDeclarationSyntax.Parent as NamespaceDeclarationSyntax;
            if (namespaceDeclarationSyntax == null)
                continue;
            if (addedNamespaces.Contains(namespaceDeclarationSyntax.Name.ToString()))
                continue;
            sourceBuilder.Append('\n');
            sourceBuilder.Append("using ");
            sourceBuilder.Append(namespaceDeclarationSyntax.Name);
            sourceBuilder.Append(';');
            addedNamespaces.Add(namespaceDeclarationSyntax.Name.ToString());
        }
        foreach (var typeDeclarationSyntax in syntaxReceiver.DefaultConstructorTypes) {
            var namespaceString = GetTypeFullNamespace(typeDeclarationSyntax);
            if (string.IsNullOrEmpty(namespaceString))
                continue;
            if (addedNamespaces.Contains(namespaceString))
                continue;
            sourceBuilder.Append('\n');
            sourceBuilder.Append("using ");
            sourceBuilder.Append(namespaceString);
            sourceBuilder.Append(';');
            addedNamespaces.Add(namespaceString);
        }
        sourceBuilder.Append(
            @"

namespace EasyUnity.InstanceConstructors {");
        sourceBuilder.AppendLine($"    public class {className} : InstanceConstructor {{");
        sourceBuilder.AppendLine(@"        public static Dictionary<Type, Func<Container, object>> Constructors = new Dictionary<Type, Func<Container, object>> {");
        for (var index = 0; index < syntaxReceiver.Constructors.Count; index++) {
            var constructorDeclarationSyntax = syntaxReceiver.Constructors[index];
            var typeDeclarationSyntax = (TypeDeclarationSyntax)constructorDeclarationSyntax.Parent;
            var fullName = GetFullTypeDeclarationSyntaxIdentifier(typeDeclarationSyntax);
            var namespaceText = GetTypeFullNamespace(typeDeclarationSyntax);
            if (!string.IsNullOrEmpty(namespaceText))
                fullName = $"{namespaceText}.{fullName}";
            if (!string.IsNullOrEmpty(fullName)) { //todo: not always good
                if (constructorDeclarationSyntax.ParameterList.Parameters.Select(GetParameterName).All(_ => !string.IsNullOrEmpty(_))) { //todo: not always good
                    if (index != 0)
                        sourceBuilder.Append('\n');
                    sourceBuilder.Append($"            {{ typeof({fullName}), container => new {fullName}(");
                    foreach (var parameter in constructorDeclarationSyntax.ParameterList.Parameters) {
                        var parameterFullName = GetParameterName(parameter);
                        var parameterNamespaceText = GetParameterFullNamespace(parameter);
                        if (!string.IsNullOrEmpty(parameterNamespaceText))
                            parameterFullName = $"{parameterNamespaceText}.{parameterFullName}";
                        sourceBuilder.Append($"container.Resolve<{parameterFullName}>()");
                        if (parameter != constructorDeclarationSyntax.ParameterList.Parameters.Last())
                            sourceBuilder.Append(", ");
                    }
                    sourceBuilder.Append(")");
                    sourceBuilder.Append(" }");
                    sourceBuilder.Append(",");
                }
            }
        }
        foreach (var typeDeclarationSyntax in syntaxReceiver.DefaultConstructorTypes) {
            var fullName = GetFullTypeDeclarationSyntaxIdentifier(typeDeclarationSyntax);
            var namespaceText = GetTypeFullNamespace(typeDeclarationSyntax);
            if (!string.IsNullOrEmpty(namespaceText))
                fullName = $"{namespaceText}.{fullName}";
            if (!string.IsNullOrEmpty(fullName)) { //todo: not always good
                sourceBuilder.Append($"\n            {{ typeof({fullName}), container => new {fullName}(");
                sourceBuilder.Append(")");
                sourceBuilder.Append(" }");
                sourceBuilder.Append(",");
            }
        }
        sourceBuilder.Append(
            @"
        };

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool TryGetInstance(Type type, Container container, out object instance) {
            if (Constructors.TryGetValue(type, out var constructor)) {
                instance = constructor.Invoke(container);
                return true;
            }
            instance = null;
            return false;
        }
    }
}
");
        context.AddSource(className, SourceText.From(sourceBuilder.ToString(), Encoding.UTF8));
    }

    public void Initialize(GeneratorInitializationContext context) {
        context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());
    }
}
