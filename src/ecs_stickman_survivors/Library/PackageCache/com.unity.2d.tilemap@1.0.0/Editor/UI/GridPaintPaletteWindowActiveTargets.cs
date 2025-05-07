using UnityEngine.UIElements;

namespace UnityEditor.Tilemaps
{
    internal class GridPaintPaletteWindowActiveTargets : VisualElement
    {
        const string kUssClassName = "unity-tilepalette-activetargets";

        public GridPaintPaletteWindowActiveTargets()
        {
            AddToClassList(kUssClassName);
            TilePaletteOverlayUtility.SetStyleSheet(this);

            name = "activeTargetsTilePalette";
            Add(new TilePaletteActiveTargetsPopupIcon());
            Add(new TilePaletteActiveTargetsPopup());
        }
    }
}
