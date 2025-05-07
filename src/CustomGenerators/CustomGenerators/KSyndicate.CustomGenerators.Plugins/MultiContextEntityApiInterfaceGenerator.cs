using System.Collections.Generic;
using System.IO;
using System.Linq;
using Entitas.CodeGeneration.Plugins;
using Jenny;

namespace KSyndicate.CustomGenerators.Plugins
{
  public class MultiContextEntityApiInterfaceGenerator : AbstractGenerator
  {
    public override string Name => "Multi Context Entity API Interface Generator";

    private const string CombinedInterfaceTemplate = @"
public partial interface IMultiContextEntity<out TEntity> :  
    ${Interfaces}
    where TEntity : Entitas.IEntity
{
}
";

    private const string EntityImplementationTemplate = @"
public partial class ${EntityName} : IMultiContextEntity<${EntityName}> { }
";

    public override CodeGenFile[] Generate(CodeGeneratorData[] data)
    {
      List<ComponentData> multiContextComponents = data
        .OfType<ComponentData>()
        .Where(d => d.GetContextNames().Length > 1)
        .ToList();

      List<string> interfaces = multiContextComponents
        .Select(GetInterfaceName)
        .ToList();

      string combinedInterface = CombinedInterfaceTemplate.Replace("${Interfaces}", string.Join(",\n    ", interfaces));

      var files = new List<CodeGenFile>
      {
        new CodeGenFile(
          "Components" + Path.DirectorySeparatorChar +
          "Interfaces" + Path.DirectorySeparatorChar +
          "IMultiContextEntity.cs", combinedInterface, GetType().FullName)
      };

      files.AddRange(GenerateEntityFiles(multiContextComponents));

      return files.ToArray();
    }
    
    private string GetInterfaceName(ComponentData data) 
    {
      return data.IsFlag() 
        ? $"I{data.ComponentName()}Entity" 
        : $"I{data.ComponentName()}Entity<TEntity>";
    }

    private IEnumerable<CodeGenFile> GenerateEntityFiles(IEnumerable<ComponentData> multiContextComponents)
    {
      var entityFiles = new List<CodeGenFile>();
      
      List<ContextList> contextLists = multiContextComponents
        .Select(c => new ContextList(c.GetContextNames()))
        .ToList();
      
      HashSet<string> commonContexts = FindCommonContexts(contextLists);

      foreach (string contextName in commonContexts)
      {
        string content = EntityImplementationTemplate.Replace("${EntityName}", contextName + "Entity");
        entityFiles.Add(new CodeGenFile(contextName + "Entity.cs", content, GetType().FullName));
      }

      return entityFiles;
    }

    private HashSet<string> FindCommonContexts(List<ContextList> contextLists)
    {
      HashSet<string> commonContexts = new HashSet<string>(contextLists[0].ContextNames);
      
      foreach (ContextList contextList in contextLists.Skip(1)) 
        commonContexts.IntersectWith(contextList.ContextNames);

      return commonContexts;
    }
  }
}