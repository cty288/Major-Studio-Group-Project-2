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
    private byte[] outputTexture;

    public byte[] OutputTexture => outputTexture;
    [ES3Serializable]
    private List<BodyInfo> storedBodyInfoId;
    
    [ES3Serializable]
    private int width;

    public int Width => width;
    [ES3Serializable]
    private int height;

    public int Height => height;


    public List<BodyInfo> StoredBodyInfoId => storedBodyInfoId;

    [field: ES3Serializable]
    public string ID { get;}
    public CropInfo(Texture2D outputTexture, List<BodyInfo> storedBodyInfoId) {
        this.outputTexture = outputTexture.GetRawTextureData();
        this.width = outputTexture.width;
        this.height = outputTexture.height;
        this.storedBodyInfoId = storedBodyInfoId;
        ID = Guid.NewGuid().ToString();
    }
    
    public CropInfo() {
        
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
    byte[] bytes;//ͼƬbyte
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
        //������������ʼ��ͼ
        if (Input.GetMouseButtonDown(0))
        {
            drawFlag = true;
            start = Input.mousePosition;
            OnDragMouse?.Invoke();
        }
        //̧��������������ͼ
        if (Input.GetMouseButtonUp(0) && drawFlag)
        {
            drawFlag = false;
            StartCoroutine(CutImage());
        }
        //�������Ҽ�ȡ����ͼ
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
    //���ƿ�ѡ  
    
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
        //if start or end is outside the screen, clamp it
        start.x = Mathf.Clamp(start.x, 0, Screen.width);
        start.y = Mathf.Clamp(start.y, 0, Screen.height);
        end.x = Mathf.Clamp(end.x, 0, Screen.width);
        end.y = Mathf.Clamp(end.y, 0, Screen.height);
        
        
        //ͼƬ��С    
        if (end.x > start.x && start.y > end.y) {
            cutImage = new Texture2D((int) (end.x - start.x), (int) (start.y - end.y), TextureFormat.RGB24, true);
            //�������½�Ϊ(0,0)��
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
                //�������½�Ϊ(0,0)��
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
                    //�������½�Ϊ(0,0)��
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
                        //�������½�Ϊ(0,0)��
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

 
// Use it like this:

    private List<BodyInfo> GetBodyInfosInRect(Rect area) {
        //get all game objects in rect
        List<BodyInfo> bodyInfos = new List<BodyInfo>();
        HashSet<long> existingIDs = new HashSet<long>();

        foreach (GameObject go in FindObjectsOfType(typeof(GameObject)) as GameObject[]) {
            Collider2D collider = go.GetComponent<Collider2D>();
            Bounds bounds = new Bounds();
            bool colliderInBounds = false;
            if (collider) {
                bounds = collider.bounds;
                //convert to screen space
                Vector3 screenPos = Camera.main.WorldToScreenPoint(bounds.center);
                bounds.center = screenPos;
                colliderInBounds = area.Contains(screenPos);
            }
            if (area.Contains(Camera.main.WorldToScreenPoint(go.transform.position))||(collider && colliderInBounds)) {
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
