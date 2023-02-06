using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using MikroFramework.Singletons;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using Action = Antlr.Runtime.Misc.Action;

public class CropInfo {
    private Texture2D outputTexture;
    private List<GameObject> allGameObjectsCropped;
    
    public CropInfo(Texture2D outputTexture, List<GameObject> allGameObjectsCropped) {
        this.outputTexture = outputTexture;
        this.allGameObjectsCropped = allGameObjectsCropped;
    }
}
public class ScreenSpaceImageCropper : MonoMikroSingleton<ScreenSpaceImageCropper> {
    [SerializeField]
    private Color rColor = Color.green;
    [SerializeField]
    private Vector3 start = Vector3.zero;
    [SerializeField]
    private Vector3 end = Vector3.zero;
    [SerializeField]
    private Material rMat = null;
    [SerializeField]
    private bool drawFlag = false;
    [SerializeField]
    private Rect rect;
    [SerializeField]
    private Texture2D cutImage;
    byte[] bytes;//图片byte
    private RawImage OutputRawImage;
    [SerializeField]
    private bool isCropping = false;
    
    [SerializeField] 
    private List<Camera> supportedCameras = new List<Camera>();
    
    private Action OnCropCancelled;
    private Action<CropInfo> OnCropFinished;
    private void Awake() {
        RenderPipelineManager.endCameraRendering += RenderPipelineManager_endCameraRendering;
    }
    
    
    private void RenderPipelineManager_endCameraRendering(ScriptableRenderContext arg1, Camera arg2) {
        if (!supportedCameras.Contains(arg2)) {
            return;
        }
        OnPostRender();
    }

    public void StartCrop(Action onCanceled, Action<CropInfo> onFinished) {
        this.OnCropCancelled = onCanceled;
        this.OnCropFinished = onFinished;
        isCropping = true;
    }

    void Update() {
        if (!isCropping) {
            return;
        }
        //按下鼠标左键开始截图
        if (Input.GetMouseButtonDown(0))
        {
            drawFlag = true;
            start = Input.mousePosition;
        }
        //抬起鼠标左键结束截图
        if (Input.GetMouseButtonUp(0) && drawFlag)
        {
            drawFlag = false;
            StartCoroutine(CutImage());
        }
        //点击鼠标右键取消截图
        if (Input.GetMouseButtonDown(1)) {
            drawFlag = false;
            OnCropCancelled?.Invoke();
            ResetEvents();
        }
    }
    
    private void ResetEvents() {
        OnCropCancelled = null;
        OnCropFinished = null;
    }
    //绘制框选  
    void OnPostRender()
    { 
        if (drawFlag) {
            //draw a rectangle, with some transparency
            end = Input.mousePosition;
            GL.PushMatrix();
            rMat.SetPass(0);
            GL.LoadOrtho();
            GL.Begin(GL.QUADS);
            GL.Color(rColor);
            GL.Vertex3(start.x / Screen.width, start.y / Screen.height, 0);
            GL.Vertex3(start.x / Screen.width, end.y / Screen.height, 0);
            GL.Vertex3(end.x / Screen.width, end.y / Screen.height, 0);
            GL.Vertex3(end.x / Screen.width, start.y / Screen.height, 0);
            GL.End();
            GL.PopMatrix();
        }
    }

    IEnumerator CutImage() {

        string date = System.DateTime.Now.ToString("yyyyMMddHHmmss");
        //图片大小    
        if (end.x > start.x && start.y > end.y) {
            cutImage = new Texture2D((int) (end.x - start.x), (int) (start.y - end.y), TextureFormat.RGB24, true);
            //坐标左下角为(0,0)点
            rect = new Rect((int) start.x,
                Screen.height - (int) (Screen.height - end.y),
                (int) (end.x - start.x),
                (int) (start.y - end.y));
            yield return new WaitForEndOfFrame();
            cutImage.ReadPixels(rect, 0, 0, false);
            cutImage.Apply();
            yield return cutImage;
        }
        else {
            if (end.x < start.x && start.y < end.y) {
                cutImage = new Texture2D((int) (start.x - end.x), (int) (end.y - start.y), TextureFormat.RGB24, true);
                //坐标左下角为(0,0)点
                rect = new Rect((int) end.x,
                    Screen.height - (int) (Screen.height - start.y),
                    (int) (start.x - end.x),
                    (int) (end.y - start.y));
                yield return new WaitForEndOfFrame();
                cutImage.ReadPixels(rect, 0, 0, false);
                cutImage.Apply();
                yield return cutImage;
            }
            else {
                if (end.x > start.x && start.y < end.y) {
                    cutImage = new Texture2D((int) (end.x - start.x), (int) (end.y - start.y), TextureFormat.RGB24,
                        true);
                    //坐标左下角为(0,0)点
                    rect = new Rect((int) start.x,
                        Screen.height - (int) (Screen.height - start.y),
                        (int) (end.x - start.x),
                        (int) (end.y - start.y));
                    yield return new WaitForEndOfFrame();
                    cutImage.ReadPixels(rect, 0, 0, false);
                    cutImage.Apply();
                    yield return cutImage;
                }
                else {
                    if (end.x < start.x && start.y > end.y) {
                        cutImage = new Texture2D((int) (start.x - end.x), (int) (start.y - end.y), TextureFormat.RGB24,
                            true);
                        //坐标左下角为(0,0)点
                        rect = new Rect((int) end.x,
                            Screen.height - (int) (Screen.height - end.y),
                            (int) (start.x - end.x),
                            (int) (start.y - end.y));
                        yield return new WaitForEndOfFrame();
                        cutImage.ReadPixels(rect, 0, 0, false);
                        cutImage.Apply();
                        yield return cutImage;
                    }
                }
            }
        }
        
        if (cutImage != null) {
            //render to the raw image
            //OutputRawImage.texture = cutImage;
            //get aspact ratio
            //float aspectRatio = cutImage.width / (float) cutImage.height;
            //set the raw image size
            //OutputRawImage.rectTransform.sizeDelta = new Vector2(300, 300 / aspectRatio);
            
            //get all game objects in rect
            List<GameObject> gameObjects = GetGameObjectsInRect(rect);
            foreach (GameObject o in gameObjects) {
                Debug.Log("Crop: " + o.name);
            }
            
            OnCropFinished?.Invoke(new CropInfo(cutImage, gameObjects));
        }
        else {
            OnCropCancelled?.Invoke();
        }
        ResetEvents();
        
    }
    public static Vector2 GetRelativePosOfWorldPoint(Vector3 worldPoint, Camera camera) {
        Vector3 screenPoint = camera.WorldToScreenPoint(worldPoint);
        return new Vector2(screenPoint.x / camera.pixelWidth, screenPoint.y / camera.pixelHeight);
    }
 
// Use it like this:

    private List<GameObject> GetGameObjectsInRect(Rect area) {
        //get all game objects in rect
        List<GameObject> gameObjects = new List<GameObject>();
        foreach (GameObject go in FindObjectsOfType(typeof(GameObject)) as GameObject[]) {
            if (area.Contains(Camera.main.WorldToScreenPoint(go.transform.position))) {
                gameObjects.Add(go);
            }
        }
        return gameObjects;
    }
}
