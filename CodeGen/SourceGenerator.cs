using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace UnityInjectorCodeGen {
    [Generator]
    public class SourceGenerator : ISourceGenerator {
        private static readonly SymbolDisplayFormat _FullNameFormat = new SymbolDisplayFormat(
            typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces,
            genericsOptions: SymbolDisplayGenericsOptions.IncludeTypeParameters);
    
        public void Initialize(GeneratorInitializationContext context) {
            context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());
        }
    
        public void Execute(GeneratorExecutionContext context) {
            if (!(context.SyntaxReceiver is SyntaxReceiver syntaxReceiver))
                return;
            var sourceModuleName = context.Compilation.SourceModule.Name;
            if (sourceModuleName.StartsWith("UnityEngine.") 
                || sourceModuleName.StartsWith("UnityEditor.") 
                || sourceModuleName.StartsWith("Unity."))
                return;
            var instanceConstructorType 
                = context.Compilation.GetTypeByMetadataName("UnityInjector.InstanceConstructors.InstanceConstructor");
            if (instanceConstructorType == null)
                return; // di framework not referenced
            if (context.Compilation.AssemblyName == null)
                return;
            var assemblyName = context.Compilation.AssemblyName
                .Replace("-", string.Empty)
                .Replace(".", string.Empty);
            var className = $"{assemblyName}_GeneratedInstanceConstructor";
            var sb = new StringBuilder(
                @"using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace UnityInjector.InstanceConstructors {
");
            sb.Append("    public class ", className, " : InstanceConstructor {\n");
            sb.AppendLine(@"        public static Dictionary<Type, Func<UnityInjector.Container, object>> Constructors = new Dictionary<Type, Func<UnityInjector.Container, object>> {");
            for (var i = 0; i < syntaxReceiver.Constructors.Count; i++) {
                var constructorDeclarationSyntax = syntaxReceiver.Constructors[i];
                var typeDeclarationSyntax = (TypeDeclarationSyntax)constructorDeclarationSyntax.Parent;
                var fullName = GetTypeFullName(context, typeDeclarationSyntax);
                if (i != 0)
                    sb.Append('\n');
                sb.Append("            { typeof(", fullName, "),  container => new ", fullName, "(");
                for (var j = 0; j < constructorDeclarationSyntax.ParameterList.Parameters.Count; j++) {
                    var parameter = constructorDeclarationSyntax.ParameterList.Parameters[j];
                    var parameterFullName = GetParameterFullName(context, parameter);
                    sb.Append("container.Resolve<", parameterFullName, ">()");
                    if (j != constructorDeclarationSyntax.ParameterList.Parameters.Count - 1)
                        sb.Append(", ");
                }
                sb.Append(") },");
            }
            for (var i = 0; i < syntaxReceiver.DefaultConstructorTypes.Count; i++) {
                var typeDeclarationSyntax = syntaxReceiver.DefaultConstructorTypes[i];
                var fullName = GetTypeFullName(context, typeDeclarationSyntax);
                sb.Append("\n            { typeof(", fullName, "), container => new ", fullName, "() },");
            }
            sb.Append(
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
            context.AddSource(className, SourceText.From(sb.ToString(), Encoding.UTF8));
        }

        private static string GetTypeFullName(GeneratorExecutionContext context, TypeDeclarationSyntax typeDeclarationSyntax) {
            var semanticModel = context.Compilation.GetSemanticModel(typeDeclarationSyntax.SyntaxTree);
            var namedTypeSymbol = semanticModel.GetDeclaredSymbol(typeDeclarationSyntax);
            return namedTypeSymbol?.ToDisplayString(_FullNameFormat);
        }
        private static string GetParameterFullName(GeneratorExecutionContext context, ParameterSyntax parameterSyntax) {
            var semanticModel = context.Compilation.GetSemanticModel(parameterSyntax.SyntaxTree);
            var parameterSymbol = semanticModel.GetDeclaredSymbol(parameterSyntax);
            var typeSymbol = parameterSymbol?.Type;
            return typeSymbol?.ToDisplayString(_FullNameFormat);
        }
    }
}
