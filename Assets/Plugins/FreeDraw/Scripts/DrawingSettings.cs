using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace FreeDraw
{
    // Helper methods used to set drawing settings
    public class DrawingSettings : MonoBehaviour
    {
        public static bool isCursorOverUI = false;
        private byte Transparency = 255;

        // new_width is radius in pixels
        public void SetMarkerWidth(int new_width)
        {
            Drawable.PEN_WIDTH = new_width;
        }

        public void SetMarkerWidth(float new_width)
        {
            SetMarkerWidth((int)new_width);
        }

        public void SetTransparency(byte a)
        {
            Transparency = a;
            Color32 c = Drawable.PEN_COLOR;
            c.a = a;
            Drawable.PEN_COLOR = c;
        }


        // Call these these to change the pen settings
        public void SetMarkerRed()
        {
            Color32 color = Color.red;
            color.a = Transparency;
            Drawable.PEN_COLOR = color;
            Drawable.drawable.SetPenBrush();
        }

        public void SetMarkerGreen()
        {
            Color32 color = Color.green;
            color.a = Transparency;
            Drawable.PEN_COLOR = color;
            Drawable.drawable.SetPenBrush();
        }

        public void SetMarkerBlue()
        {
            Color32 color = Color.blue;
            color.a = Transparency;
            Drawable.PEN_COLOR = color;
            Drawable.drawable.SetPenBrush();
        }

        public void SetEraser()
        {
            Drawable.PEN_COLOR = new Color32(255, 255, 255, 0);
        }

        public void PartialSetEraser()
        {
            Drawable.PEN_COLOR = new Color32(255, 255, 255, 120);
        }
    }
}