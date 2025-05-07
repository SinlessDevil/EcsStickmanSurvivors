using System.Collections.Generic;
using System.IO;
using System.Linq;
using Entitas.CodeGeneration.Plugins;
using Jenny;

namespace KSyndicate.CustomGenerators.Plugins
{
  public class SingleValueComponentPropertyGenerator : ICodeGenerator
  {
    private const string ValueBaseString =
      @"public ${ComponentType} ${validComponentName} { get { return (${ComponentType})GetComponent(${Index}); } }";

    private const string PropertyStart =
      "public ${ComponentType} ${validComponentName} { get { return (${ComponentType})GetComponent(${Index}); } }\n    public";

    private const string EntityRequestPropertyStart =
      "public ${ComponentType} ${validComponentName} { get { return (${ComponentType})GetComponent(${Index}); } }\n    public";

    private const string PropertyEnd = " { get { return ${validComponentName}.Value; } }";
    private const string EntityRequestPropertyEnd = " { get { return ${validComponentName}.Value.Entity; } }";

    private const string AddSignature = "public void Add${ComponentName}(${newMethodParameters})";
    private const string AddBuilderSignature = "public ${EntityType} Add${ComponentName}(${newMethodParameters})";
    private const string AddEnding = "AddComponent(index, component);\n    }";
    private const string AddBuilderEnding = "AddComponent(index, component);\n        return this;\n    }";

    private const string ReplaceSignature = "public void Replace${ComponentName}(${newMethodParameters}) {\n";

    private const string ReplaceBuilderSignature = "public ${EntityType} Replace${ComponentName}(${newMethodParameters}) {\n";

    private const string ReplaceEnding = "ReplaceComponent(index, component);\n    }";
    private const string ReplaceBuilderEnding = "ReplaceComponent(index, component);\n        return this;\n    }";

    private const string RemoveSignature = "public void Remove${ComponentName}() {\n";
    private const string RemoveBuilderSignature = "public ${EntityType} Remove${ComponentName}() {\n";
    private const string RemoveEnding = "RemoveComponent(${Index});\n    }";
    private const string RemoveBuilderEnding = "RemoveComponent(${Index});\n        return this;\n    }";

    private readonly ComponentEntityApiGenerator _baseGenerator = new ComponentEntityApiGenerator();
    public string Name => "Component (Entity API) + Single Value Component Property";
    public int Order => 0;
    public bool RunInDryMode => true;

    public CodeGenFile[] Generate(CodeGeneratorData[] data)
    {
      CodeGenFile[] codeGenFiles = _baseGenerator.Generate(data);
      ComponentData[] components = data.OfType<ComponentData>().ToArray();

      for (var index = 0; index < components.Length; index++)
      {
        ComponentData component = components[index];
        MemberData[] members = component.GetMemberData();

        foreach (CodeGenFile file in FileByComponentNameIn(codeGenFiles, component))
        {
          if (members.IsSingleValueComponent())
            AddPropertyCode(members.First(), file, component);

          if (!ComponentIsFlag(members))
            MakeBuilderSignatures(file, component);
        }
      }

      return codeGenFiles;
    }

    private static IEnumerable<CodeGenFile> FileByComponentNameIn(CodeGenFile[] codeGenFiles, ComponentData component)
    {
      foreach (CodeGenFile file in codeGenFiles)
      {
        string contextName = file.ContextName();
        string[] componentContextNames = component.GetContextNames();

        string componentNameStripped = component.ComponentName()
          .Replace("Component", "");

        if (componentContextNames.Any(x => x == contextName) && Path.GetFileName(file.FileName) == $"{contextName}{componentNameStripped}Component.cs")
          yield return file;
      }
    }

    private static bool ComponentIsFlag(MemberData[] members)
    {
      return members.Length == 0;
    }

    private static void AddPropertyCode(MemberData member, CodeGenFile codeGenFile, ComponentData component)
    {
      string componentName = component.ComponentName();

      var typeAndName = $" {PropertyReturn()} {componentName.UppercaseFirst()}";
      var withProperty = $"{PropertyCallStart()}{typeAndName}{PropertyCallEnd()}";

      ReplaceWithResolvedNames(codeGenFile, component, ValueBaseString, withProperty);

      string PropertyCallStart()
      {
        return IsEntityRequest() ? EntityRequestPropertyStart : PropertyStart;
      }

      string PropertyCallEnd()
      {
        return IsEntityRequest() ? EntityRequestPropertyEnd : PropertyEnd;
      }

      string PropertyReturn()
      {
        return IsEntityRequest() ? "${EntityType}" : member.type;
      }

      bool IsEntityRequest()
      {
        return member.type.Contains("EntityRequest") && !member.type.Contains("List");
      }
    }

    private static void MakeBuilderSignatures(CodeGenFile codeGenFile, ComponentData component)
    {
      ReplaceWithResolvedNames(codeGenFile, component, AddSignature, AddBuilderSignature);
      ReplaceWithResolvedNames(codeGenFile, component, AddEnding, AddBuilderEnding);

      ReplaceWithResolvedNames(codeGenFile, component, ReplaceSignature, ReplaceBuilderSignature);
      ReplaceWithResolvedNames(codeGenFile, component, ReplaceEnding, ReplaceBuilderEnding);

      ReplaceWithResolvedNames(codeGenFile, component, RemoveSignature, RemoveBuilderSignature);
      ReplaceWithResolvedNames(codeGenFile, component, RemoveEnding, RemoveBuilderEnding);
    }

    private static void ReplaceWithResolvedNames(CodeGenFile codeGenFile, ComponentData component, string baseSignature, string betterSignature)
    {
      string contextName = codeGenFile.ContextName();

      string baseWithResolvedNames = baseSignature.Replace(component, contextName);
      string betterWithResolvedNames = betterSignature.Replace(component, contextName);

      codeGenFile.FileContent = codeGenFile.FileContent.Replace(baseWithResolvedNames, betterWithResolvedNames);
    }
  }
}