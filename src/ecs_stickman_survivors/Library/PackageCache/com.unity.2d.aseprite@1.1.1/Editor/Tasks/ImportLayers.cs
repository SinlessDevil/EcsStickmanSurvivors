using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

namespace UnityEditor.U2D.Aseprite
{
    internal static class ImportLayers
    {
        public static void Import(List<Layer> layers, out List<NativeArray<Color32>> cellBuffers, out List<int> cellWidth, out List<int> cellHeight)
        {
            cellBuffers = new List<NativeArray<Color32>>();
            cellWidth = new List<int>();
            cellHeight = new List<int>();

            for (var i = 0; i < layers.Count; ++i)
            {
                var cells = layers[i].cells;
                for (var m = 0; m < cells.Count; ++m)
                {
                    var width = cells[m].cellRect.width;
                    var height = cells[m].cellRect.height;
                    if (width == 0 || height == 0)
                        continue;

                    cellBuffers.Add(cells[m].image);
                    cellWidth.Add(width);
                    cellHeight.Add(height);
                }
            }

            for (var i = 0; i < cellBuffers.Count; ++i)
            {
                var buffer = cellBuffers[i];
                TextureTasks.FlipTextureY(ref buffer, cellWidth[i], cellHeight[i]);
                cellBuffers[i] = buffer;
            }            
        }
    }
}