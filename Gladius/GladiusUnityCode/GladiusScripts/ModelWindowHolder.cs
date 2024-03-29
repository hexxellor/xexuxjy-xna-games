using UnityEngine;
using System.Collections;
using Gladius;

public class ModelWindowHolder : MonoBehaviour
{
    private GameObject m_gameObjectInstance;
    private GameObject m_gameObjectPrefab;
    
    public  GameObject m_modelCamera;
    private dfTextureSprite m_parentSprite;
    GUITexture gtexture = new GUITexture();
    public float RotationSpeed = 30f;
    private Vector3 m_localCenter;

    private float m_angle;

    public void Awake()
    {
        //m_modelCamera = GameObject.Find("ModelCamera");
        //m_modelCamera.GetComponent<SnapshotCamera>().ScreenReadyEvent += new SnapshotCamera.ScreenReadyEventDelegate(ModelWindowHolder_ScreenReadyEvent);
        m_parentSprite = GetComponent<dfTextureSprite>();
    }

    void ModelWindowHolder_ScreenReadyEvent()
    {
        //m_parentSprite.Texture = m_modelCamera.GetComponent<SnapshotCamera>().screenshot;
        //gtexture.texture = m_modelCamera.GetComponent<SnapshotCamera>().screenshot;
        //m_parentPanel. = gtexture;
    }

    public void AttachedModelPrefabToWindow(GameObject prefab)
    {
        if (prefab != null)
        {
            // if we have a different prefab
            if (m_gameObjectPrefab != prefab)
            {
                m_gameObjectPrefab = prefab;
                if (m_gameObjectInstance != null)
                {
                    Destroy(m_gameObjectInstance);
                    m_gameObjectInstance = null;
                }
                m_angle = 0f;
                //dfPanel parentPanel = GetComponent<dfPanel>();
                //Bounds bb = parentPanel.GetBounds();
                //Vector3 extents = bb.extents;
                //extents.y *=-1f;
                //extents.z = 0.2f;

                m_gameObjectInstance = Instantiate(m_gameObjectPrefab) as GameObject;
                
                int uiLayerId = LayerMask.NameToLayer("UI");
                GladiusGlobals.MoveToLayer(m_gameObjectInstance.transform, uiLayerId);

                m_gameObjectInstance.transform.parent = m_modelCamera.transform;
                m_gameObjectInstance.transform.localPosition = new Vector3(0, 0, 0);
                m_gameObjectInstance.transform.localRotation = Quaternion.Euler(new Vector3(90, 90, 0));
                m_gameObjectInstance.transform.localScale = new Vector3(50, 50, 50);

                
                MeshFilter mf = m_gameObjectInstance.GetComponentInChildren<MeshFilter>();
                MeshRenderer mr = m_gameObjectInstance.GetComponentInChildren<MeshRenderer>();
                SkinnedMeshRenderer smr = m_gameObjectInstance.GetComponentInChildren<SkinnedMeshRenderer>();
                if (mf != null)
                {
                    Bounds objectBounds = mf.mesh.bounds;
                    Vector3 extents = objectBounds.extents;
                    //extents.Scale(m_gameObjectInstance.transform.localScale);
                    //extents.Scale(m_gameObjectInstance.transform.localScale);
                    //Debug.Log("Bounds : " + objectBounds.extents);
                    Camera objectCamera = GameObject.Find("/ItemCamHolder/Camera").GetComponent<Camera>();
                    //http://docs.unity3d.com/Manual/FrustumSizeAtDistance.html

                    float distance = 10f;
                    float recomendedHeight = 2.0f * Mathf.Tan(0.5f * objectCamera.fieldOfView * Mathf.Deg2Rad) * distance;
                    float frustumHeight = 2.0f * distance * Mathf.Tan(objectCamera.fieldOfView * 0.5f * Mathf.Deg2Rad);

                    float scaleFactor = frustumHeight / extents.y;
                    scaleFactor /= 4f;

                    m_localCenter = objectBounds.center;
                    m_localCenter *= scaleFactor;

                    m_gameObjectInstance.transform.localPosition = (objectCamera.transform.forward * distance) - m_localCenter;
                    m_gameObjectInstance.transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);
                }
            }
        }
    }


    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        m_angle += RotationSpeed * Time.deltaTime;

        if (m_gameObjectInstance != null)
        {
            //m_gameObjectInstance.transform.RotateAround(m_localCenter, Vector3.up, m_angle);
            Quaternion q = Quaternion.Euler(new Vector3(m_angle, m_angle, 0));
            m_gameObjectInstance.transform.localRotation = q;

        }
    }
}
