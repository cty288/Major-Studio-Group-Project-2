using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class TestCode : MonoBehaviour
{
    
    
    private bool isMouseDown = false;
    private Vector2 min = Vector2.positiveInfinity;
    private Vector2 max = Vector2.negativeInfinity;
    public Camera camera;
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isMouseDown = true;
          
            min = Vector2.positiveInfinity;
            max = Vector2.negativeInfinity;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isMouseDown = false;
        }
 
        if (isMouseDown)
        {
            Vector2 mousePosition = Input.mousePosition;
            Vector2 worldPosition = camera.ScreenToWorldPoint(mousePosition);
          
           
           
 
            min.x = Mathf.Min(min.x, worldPosition.x);
            min.y = Mathf.Min(min.y, worldPosition.y);
            max.x = Mathf.Max(max.x, worldPosition.x);
            max.y = Mathf.Max(max.y, worldPosition.y);
        }

        if (Input.GetKeyDown(KeyCode.Space)) {
            SaveScreen();
        }
    }
 
    public void SaveScreen()
    {
        int width = (int)(max.x - min.x);
        int height = (int)(max.y - min.y);
        
        Texture2D texture = new Texture2D(width, height, TextureFormat.RGB24, false);
        RenderTexture rt = new RenderTexture(width, height, 24);
        
        
        Color[] colors = new Color[width * height];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Vector2 point = new Vector2(x + min.x, y + min.y);
                colors[y * width + x] = GetPixel(camera.targetTexture, point);
            }
        }
        texture.SetPixels(colors);
        texture.Apply();
        
        byte[] bytes = texture.EncodeToPNG();
        File.WriteAllBytes(Application.dataPath + "/screenshot.png", bytes);
    }
 
    private Color GetPixel(RenderTexture rt, Vector2 pos)
    {
        RenderTexture.active = rt;
         Texture2D tex = new Texture2D(rt.width, rt.height);
        tex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        tex.Apply();
        return tex.GetPixel((int)pos.x, (int)pos.y);
    }
    
    private bool IsPointInPolygon(Vector2 point, Vector2[] polygon)
    {
        int intersections = 0;
        for (int i = 0; i < polygon.Length - 1; i++)
        {
            Vector2 a = polygon[i];
            Vector2 b = polygon[i + 1];
            if (RayIntersectsSegment(point, a, b))
            {
                intersections++;
            }
        }
        return (intersections % 2 == 1);
    }
 
    private bool RayIntersectsSegment(Vector2 point, Vector2 a, Vector2 b)
    {
        Vector2 p = point;
        Vector2 p1 = a;
        Vector2 p2 = b;
 
        float dx = p2.x - p1.x;
        float dy = p2.y - p1.y;
        float dpx = p.x - p1.x;
        float dpy = p.y - p1.y;
 
        float T2 = dpx * dx + dpy * dy;
 
        if (T2 < 0)
        {
            return false;
        }
 
        float T1 = dx * dx + dy * dy;
 
        if (T2 > T1)
        {
            return false;
        }
 
        if (T1 == 0)
        {
            return false;
        }
 
        float R = T2 / T1;
        float Py = p1.y + R * (p2.y - p1.y);
        float Px = p1.x + R * (p2.x - p1.x);
 
        if ((Py < Mathf.Min(p1.y, p2.y) || Py > Mathf.Max(p1.y, p2.y)) || (Px < Mathf.Min(p1.x, p2.x) || Px > Mathf.Max(p1.x, p2.x)))
        {
            return false;
        }
 
        return true;
    }

}
