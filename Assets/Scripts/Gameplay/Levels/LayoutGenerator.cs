using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayoutGenerator
{
    private const int _BubbleCount = 100;
    private const int _MaxRowCount = 10;
    private const int _MaxColumnCount = 10;
    private const float _CellSize = 0.25f * 2f;
    private static readonly Vector2 _Center = new Vector2(0f, 2f);

    public static BubblePlacement[] GenerateBubbleLayout()
    {
        var result = new BubblePlacement[_BubbleCount];

        int row = 0;
        int column = 0;

        float cellRadius = _CellSize * 0.5f;
        float rowHeight = (Quaternion.Euler(0f, 0f, 30f) * new Vector2(0f, _CellSize)).y;

        for (int i= 0; i < result.Length; i++)
        {
            result[i] = new BubblePlacement();

            result[i].Color = (BubbleColor)Random.Range(0, (int)BubbleColor.ColorCount);

            result[i].Position = _Center + new Vector2(
                (-_MaxColumnCount * 0.5f + column) * _CellSize + cellRadius + ((row % 2) * cellRadius - cellRadius * 0.5f),
                (-_MaxRowCount * 0.5f + row) * rowHeight + cellRadius
            );

            column++;

            if (column >= _MaxColumnCount)
            {
                column = 0;
                row++;
            }
        }

        return result;
    }
}
