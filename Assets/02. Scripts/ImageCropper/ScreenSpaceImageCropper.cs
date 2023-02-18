using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using MikroFramework.Architecture;
using MikroFramework.Singletons;
using NHibernate.Mapping;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using Action = Antlr.Runtime.Misc.Action;

[ES3Serializable]
public class CropInfo {
    [ES3Serializable]
    private Texture2D outputTexture;

    public Texture2D OutputTexture => outputTexture;
    [ES3Serializable]
    private List<BodyInfo> storedBodyInfoId;

    public List<BodyInfo> StoredBodyInfoId => storedBodyInfoId;

    public CropInfo(Texture2D outputTexture, List<BodyInfo> storedBodyInfoId) {
        this.outputTexture = outputTexture;
        this.storedBodyInfoId = storedBodyInfoId;
    }
}
public class ScreenSpaceImageCropper : MonoMikroSingleton<ScreenSpaceImageCropper>, IController {
    [SerializeField]
    private Color rColor = Color.green;
    
    [SerializeField]
    private Color unavailableColor = Color.red;
    
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

    public bool IsCropping => isCropping;

    [SerializeField] 
    private List<Camera> supportedCameras = new List<Camera>();
    
    
    private Action OnCropCancelled;
    private Action<CropInfo> OnCropFinished;
    public Action OnStartCrop;
    public Action OnDragMouse;
    public Action OnEndCrop;
    
    protected PlayerControlModel playerControlModel;
    private void Awake() {
        RenderPipelineManager.endCameraRendering += RenderPipelineManager_endCameraRendering;
        playerControlModel = this.GetModel<PlayerControlModel>();
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
        playerControlModel.ControlType.Value = PlayerControlType.Screenshot;
        OnStartCrop?.Invoke();
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
            OnDragMouse?.Invoke();
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
            OnEndCrop?.Invoke();
            OnCropCancelled?.Invoke();
            ResetEvents();
        }
    }
    
    private void ResetEvents() {
        OnCropCancelled = null;
        OnCropFinished = null;
        isCropping = false;
        playerControlModel.ControlType.Value = PlayerControlType.Normal;
    }
    //绘制框选  
    void OnPostRender()
    { 
        if (drawFlag) {
            //draw a rectangle, with some transparency
            end = Input.mousePosition;
            
            //make sure it is a square
            float width = Mathf.Abs(start.x - end.x);
            float height = Mathf.Abs(start.y - end.y);
            float size = Mathf.Min(width, height);
            //change end to be the correct size
            if (start.x < end.x) {
                end.x = start.x + size;
            }
            else {
                end.x = start.x - size;
            }
            if (start.y < end.y) {
                end.y = start.y + size;
            }
            else {
                end.y = start.y - size;
            }
            
            
            
            GL.PushMatrix();
            rMat.SetPass(0);
            GL.LoadOrtho();
            GL.Begin(GL.QUADS);
            GL.Color(rColor);
            
            //if the rectangle is too small or the ratio is too large, draw it in red
            if (Mathf.Abs(end.x - start.x) < 100 || Mathf.Abs(end.y - start.y) < 100 || Mathf.Abs((end.x - start.x) / (end.y - start.y)) > 3) {
               
                GL.Color(unavailableColor);
            }
            
            GL.Vertex3(start.x / Screen.width, start.y / Screen.height, 0);
            GL.Vertex3(start.x / Screen.width, end.y / Screen.height, 0);
            GL.Vertex3(end.x / Screen.width, end.y / Screen.height, 0);
            GL.Vertex3(end.x / Screen.width, start.y / Screen.height, 0);
            GL.End();
            GL.PopMatrix();
        }
    }

    IEnumerator CutImage() {
        if (Mathf.Abs(end.x - start.x) < 100 || Mathf.Abs(end.y - start.y) < 100 || Mathf.Abs((end.x - start.x) / (end.y - start.y)) > 3) {
               
            OnCropCancelled?.Invoke();
            OnEndCrop?.Invoke();
            ResetEvents();
        }

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
            List<BodyInfo> gameObjects = GetBodyInfosInRect(rect);
            foreach (BodyInfo o in gameObjects) {
                Debug.Log("Crop: " + o.ID);
            }
            
            OnCropFinished?.Invoke(new CropInfo(cutImage, gameObjects));
            OnEndCrop?.Invoke();
        }
        else {
            OnCropCancelled?.Invoke();
            OnEndCrop?.Invoke();
        }
        ResetEvents();
        
    }
    public static Vector2 GetRelativePosOfWorldPoint(Vector3 worldPoint, Camera camera) {
        Vector3 screenPoint = camera.WorldToScreenPoint(worldPoint);
        return new Vector2(screenPoint.x / camera.pixelWidth, screenPoint.y / camera.pixelHeight);
    }
 
// Use it like this:

    private List<BodyInfo> GetBodyInfosInRect(Rect area) {
        //get all game objects in rect
        List<BodyInfo> bodyInfos = new List<BodyInfo>();
        HashSet<long> existingIDs = new HashSet<long>();

        foreach (GameObject go in FindObjectsOfType(typeof(GameObject)) as GameObject[]) {
            if (area.Contains(Camera.main.WorldToScreenPoint(go.transform.position))) {
                if(go.TryGetComponent<IHaveBodyInfo>(out var bodyInfo)) {
                    foreach (BodyInfo info in bodyInfo.BodyInfos) {
                        if (!existingIDs.Contains(info.ID)) {
                            bodyInfos.Add(info);
                            existingIDs.Add(info.ID);
                        }
                    }
                }
            }
        }
        return bodyInfos;
    }

    public IArchitecture GetArchitecture() {
        return MainGame.Interface;
    }
}
