using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UnityEditor.U2D.Aseprite
{
    internal class AsepriteImporterEditorExternalData : ScriptableObject
    {
        [SerializeField]
        public List<TextureImporterPlatformSettings> platformSettings = new List<TextureImporterPlatformSettings>();

        public void Init(AsepriteImporter importer, IList<TextureImporterPlatformSettings> platformSettingsNeeded)
        {
            var importerPlatformSettings = importer.GetAllPlatformSettings();
            
            for (var i = 0; i < importerPlatformSettings.Length; ++i)
            {
                var tip = importerPlatformSettings[i];
                var setting = platformSettings.FirstOrDefault(x => x.name == tip.name);
                if (setting == null)
                {
                    TextureImporterUtilities.UpdateWithDefaultSettings(ref tip);
                    platformSettings.Add(tip);
                }
            }
            
            for (var i = 0; i < platformSettingsNeeded.Count; ++i)
            {
                var ps = platformSettingsNeeded[i];
                var setting = platformSettings.FirstOrDefault(x => x.name == ps.name);
                if (setting == null)
                {
                    TextureImporterUtilities.UpdateWithDefaultSettings(ref ps);
                    platformSettings.Add(ps);
                }
            }
        }
    }    
}