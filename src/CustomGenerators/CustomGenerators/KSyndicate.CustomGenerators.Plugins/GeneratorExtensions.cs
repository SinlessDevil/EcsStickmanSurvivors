using System;
using System.IO;
using Entitas.CodeGeneration.Plugins;
using Jenny;

namespace KSyndicate.CustomGenerators.Plugins
{
  public static class GeneratorExtensions
  {
    public const string CONTEXT_SUFFIX = "Context";
    public const string ENTITY_SUFFIX = "Entity";
    public const string COMPONENT_SUFFIX = "Component";
    public const string SYSTEM_SUFFIX = "System";
    public const string MATCHER_SUFFIX = "Matcher";
    public const string LISTENER_SUFFIX = "Listener";
    
    public static string ContextName(this CodeGenFile codeGenFile) =>
      codeGenFile.FileName.Contains(Path.DirectorySeparatorChar.ToString())
        ? codeGenFile.FileName.Substring(0, codeGenFile.FileName.IndexOf(Path.DirectorySeparatorChar))
        : "";

    public static bool IsSingleValueComponent(this MemberData[] members) =>
      members.Length == 1 &&
      string.Compare(members[0].name, "Value", StringComparison.InvariantCulture) == 0;

    public static string UppercaseFirst(this string str) => string.IsNullOrEmpty(str) ? str : char.ToUpper(str[0]).ToString() + str.Substring(1);
    
    public static string AddComponentSuffix(this string str) => GeneratorExtensions.AddSuffix(str, "Component");
    
    private static string AddSuffix(string str, string suffix) => !GeneratorExtensions.HasSuffix(str, suffix) ? str + suffix : str;
    
    private static string RemoveSuffix(string str, string suffix) => !GeneratorExtensions.HasSuffix(str, suffix) ? str : str.Substring(0, str.Length - suffix.Length);

    private static bool HasSuffix(string str, string suffix) => str.EndsWith(suffix, StringComparison.Ordinal);
    
    public static bool IsFlag(this ComponentData data) => data.GetMemberData().Length == 0;
  }
}