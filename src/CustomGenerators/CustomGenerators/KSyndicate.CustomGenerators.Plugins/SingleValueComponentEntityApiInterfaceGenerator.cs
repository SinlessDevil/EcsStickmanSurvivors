using System.IO;
using System.Linq;
using Entitas.CodeGeneration.Plugins;
using Jenny;

namespace KSyndicate.CustomGenerators.Plugins
{
  public class SingleValueComponentEntityApiInterfaceGenerator : AbstractGenerator
  {
    public override string Name => "Single Value Component (Entity API Interface)";

    private const string PropertyCode = "${PropertyCode}";
    private const string StandardProperty = "${ComponentType} ${validComponentName} { get; } \n";

    private const string StandardTemplate = "public partial interface I${ComponentName}Entity<TEntity> : Entitas.IEntity where TEntity : Entitas.IEntity  " +
                                             "{\n\n    " +
                                             "${PropertyCode}\n    " +
                                             "${ComponentType} ${validComponentName} { get; }\n    " +
                                             "bool has${ComponentName} { get; }\n\n    " +
                                             "TEntity Add${ComponentName}(${newMethodParameters});\n    " +
                                             "TEntity Replace${ComponentName}(${newMethodParameters});\n    " +
                                             "TEntity Remove${ComponentName}();\n" +
                                             "}\n";
    
    private const string FlagTemplate = "public partial interface I${ComponentName}Entity " +
                                         "{\n   " +
                                         " bool ${prefixedComponentName} { get; set; }\n" +
                                         "}\n";

    private const string ValueComponentImplementationTemplate = "public partial class ${EntityType} : I${ComponentName}Entity<${EntityType}> { }\n";
    private const string FlagImplementationTemplate = "public partial class ${EntityType} : I${ComponentName}Entity { }\n";
    
    public override CodeGenFile[] Generate(CodeGeneratorData[] data) =>
      data
        .OfType<ComponentData>()
        .Where(d => d.ShouldGenerateMethods())
        .Where(d => d.GetContextNames().Length > 1)
        .SelectMany(Generate)
        .ToArray();

    private CodeGenFile[] Generate(ComponentData data) =>
      new[] { GenerateInterface(data) }
        .Concat(data.GetContextNames().Select(contextName => GenerateEntityInterfaceImplementation(contextName, data)))
        .ToArray();

    private CodeGenFile GenerateInterface(ComponentData data)
    {
      string template = data.IsFlag() ? FlagTemplate : StandardTemplate;
      string fileContent = ReplacePropertyIfSingleValue(template, data).Replace(data, string.Empty);

      if (!data.IsFlag())
        fileContent = AddNonGenericInterface(fileContent, data);

      return new CodeGenFile(
        "Components" + Path.DirectorySeparatorChar +
        "Interfaces" + Path.DirectorySeparatorChar +
        "I" + data.ComponentName() + "Entity.cs",
        fileContent,
        GetType().FullName
      );
    }

    private string AddNonGenericInterface(string fileContent, ComponentData data)
    {
      string nonGenericInterface = $"public interface I{data.ComponentName()}Entity : I{data.ComponentName()}Entity<Entitas.IEntity> {{}}\n";
      return fileContent + "\n" + nonGenericInterface;
    }

    private static string ReplacePropertyIfSingleValue(string template, ComponentData data)
    {
      MemberData[] members = data.GetMemberData();
      if (!members.IsSingleValueComponent())
        return template.Replace(PropertyCode, StandardProperty);

      string modifiedProperty = $"{members[0].type} {data.ComponentName().UppercaseFirst()} {{ get; }}";
      
      return template.Replace(PropertyCode, modifiedProperty);
    }

    private CodeGenFile GenerateEntityInterfaceImplementation(string contextName, ComponentData data)
    {
      string content = data.IsFlag()
        ? FlagImplementationWithReplacements(contextName, data)
        : ValueComponentImplementation(contextName, data).Replace(data, contextName);

      return new CodeGenFile(
        contextName + Path.DirectorySeparatorChar +
        "Components" + Path.DirectorySeparatorChar +
        data.ComponentNameWithContext(contextName).AddComponentSuffix() + ".cs",
        content,
        GetType().FullName
      );
    }
    
    private string ValueComponentImplementation(string contextName, ComponentData data)
    {
      string entityName = contextName + "Entity";
      string componentName = data.ComponentName();
      string methods = "";
      string parameters = "${newMethodParameters}";

      methods += $"    Entitas.IEntity I{componentName}Entity<Entitas.IEntity>.Add{componentName}({parameters})\n    {{\n        return Add{componentName}(newValue);\n    }}\n\n";
      methods += $"    Entitas.IEntity I{componentName}Entity<Entitas.IEntity>.Replace{componentName}({parameters})\n    {{\n        return Replace{componentName}(newValue);\n    }}\n\n";
      methods += $"    Entitas.IEntity I{componentName}Entity<Entitas.IEntity>.Remove{componentName}()\n    {{\n        return Remove{componentName}();\n    }}\n";

      return $"public partial class {entityName} : I{componentName}Entity<{entityName}>, I{componentName}Entity\n{{\n{methods}}}\n";
    }

    private static string FlagImplementationWithReplacements(string contextName, ComponentData data)
    {
      return ReplacePropertyIfSingleValue(FlagImplementationTemplate.Replace(data, contextName), data);
    }

    private static string ImplementationWithReplacements(string contextName, ComponentData data)
    {
      string template = data.IsFlag()
        ? FlagImplementationTemplate
        : ValueComponentImplementationTemplate;
      
      return ReplacePropertyIfSingleValue(template.Replace(data, contextName), data);
    }
  }
}