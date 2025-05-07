using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Entitas.CodeGeneration.Plugins;
using Jenny;

namespace KSyndicate.CustomGenerators.Plugins
{
  public class SingleValueComponentContextPropertyGenerator : AbstractGenerator
  {
    private const string ValueBaseString = @"public ${ComponentType} ${validComponentName} { get { return ${validComponentName}Entity.${validComponentName}; } }";
    private const string ValueAndPropertyStringStart = "public ${ComponentType} ${validComponentName} { get { return ${validComponentName}Entity.${validComponentName}; } }\n    public";
    private const string ValueAndPropertyStringEnd = " { get { return ${validComponentName}.Value; } }";

    private readonly ComponentContextApiGenerator _baseGenerator = new ComponentContextApiGenerator();
    public override string Name => "Component (Context API) + Single Value Component Property";

    public override CodeGenFile[] Generate(CodeGeneratorData[] data)
    {
      CodeGenFile[] codeGenFiles = _baseGenerator.Generate(data);
      ComponentData[] components = data.OfType<ComponentData>().ToArray();

      foreach (ComponentData component in components)
      {
        MemberData[] members = component.GetMemberData();

        if (component.IsUniqueAndSingleValueComponent(members))
          foreach (CodeGenFile file in component.CorrespondingFiles(codeGenFiles))
            AddPropertyCode(members.First(), file, component);
      }

      return codeGenFiles;
    }

    private static void AddPropertyCode(MemberData member, CodeGenFile codeGenFile, ComponentData component)
    {
      var typeAndName = $" {member.type} {component.ComponentName().UppercaseFirst()}";
      var withProperty = $"{ValueAndPropertyStringStart}{typeAndName}{ValueAndPropertyStringEnd}";

      ReplaceWithResolvedNames(codeGenFile, component, ValueBaseString, withProperty);
    }

    private static void ReplaceWithResolvedNames(CodeGenFile codeGenFile, ComponentData component, string baseSignature, string builderSignature)
    {
      string baseWithResolvedNames = baseSignature.Replace(component, codeGenFile.ContextName().Replace("Context", ""));
      string builderWithResolvedNames = builderSignature.Replace(component, codeGenFile.ContextName().Replace("Context", ""));

      codeGenFile.FileContent = codeGenFile.FileContent.Replace(baseWithResolvedNames, builderWithResolvedNames);
    }
  }

  public static class CleanCodeExtensions
  {
    public static bool IsUniqueAndSingleValueComponent(this ComponentData component, MemberData[] members)
    {
      return component.IsUnique() && members.Length == 1 && string.Compare(members[0].name, "Value", StringComparison.InvariantCultureIgnoreCase) == 0;
    }

    public static CodeGenFile CorrespondingFile(this ComponentData component, CodeGenFile[] codeGenFiles)
    {
      return codeGenFiles.FirstOrDefault(f => Path.GetFileName(f.FileName) == $"{component.GetContextNames().First()}{component.ComponentName()}Component.cs");
    }

    public static IEnumerable<CodeGenFile> CorrespondingFiles(this ComponentData component, CodeGenFile[] codeGenFiles)
    {
      foreach (string contextName in component.GetContextNames())
        yield return codeGenFiles
          .FirstOrDefault(f => Path.GetFileName(f.FileName) == $"{contextName}{component.ComponentName()}Component.cs");
    }
  }
}