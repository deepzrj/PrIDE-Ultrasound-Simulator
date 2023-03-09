using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KDicom;

public class DisplayDicomImage3DPlaneBehaviour : MonoBehaviour
{

    Texture2D m_Texture;
    float m_ElapsedTime = 0.0f;
    bool m_Reverse = false;

    [SerializeField]
    Material m_Material;

    [SerializeField]
    MPRType m_MPR_Type = MPRType.Axial;
    [SerializeField]
    bool m_AutoIndex = false;

    [SerializeField]
    int m_Index;

    [SerializeField]
    int m_WindowWidth = 1000;
    [SerializeField]
    int m_WindowCenter = 500;

    IDicomVolume m_DicomVolume;

    public enum MPRType
    {
        Axial,
        Coronal,
        Sagittal,
    }

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (m_DicomVolume == null)
        {
            FindDicomVolume();
            return;
        }

        if (m_AutoIndex)
        {
            int max = 0;
            switch (m_MPR_Type)
            {
                case MPRType.Axial:
                    max = m_DicomVolume.Depth;
                    break;
                case MPRType.Coronal:
                    max = m_DicomVolume.Height;
                    break;
                case MPRType.Sagittal:
                    max = m_DicomVolume.Width;
                    break;
            }

            if (m_Index < 0)
                m_Index = 0;
            if (m_Index > max)
                m_Index = max;

            m_ElapsedTime += Time.deltaTime * max / 5.0f;
            var index = (int)m_ElapsedTime;
            if (m_Reverse)
            {
                //                if (index != 0)
                {
                    m_Index -= index;
                    m_ElapsedTime -= index;
                    if (m_Index < 0)
                    {
                        m_Index = 0;
                        m_Reverse = false;

                        System.GC.Collect();
                        Resources.UnloadUnusedAssets();
                    }
                    UpdateImage();
                }
            }
            else
            {
                //                if (index != 0)
                {
                    m_Index += index;
                    m_ElapsedTime -= index;
                    if (m_Index >= max)
                    {
                        m_Index = max;

                        m_Reverse = true;

                        System.GC.Collect();
                        Resources.UnloadUnusedAssets();
                    }
                    UpdateImage();
                }
            }
        }

    }


    void OnGUI()
    {
        if (m_DicomVolume == null)
            return;

        // Display DICOM information
        GUI.Label(new Rect(60, 180, 600, 20), "Index : " + m_Index);
        GUI.Label(new Rect(60, 200, 600, 20), "Size : (" + m_DicomVolume.Width + ", " + m_DicomVolume.Height + ", " + m_DicomVolume.Depth + ")");
        GUI.Label(new Rect(60, 220, 600, 20), "PixelSpacing : (" + m_DicomVolume.XPixelSpacing().ToString("F5") + ", " + m_DicomVolume.YPixelSpacing().ToString("F5") + ")");
        GUI.Label(new Rect(60, 240, 600, 20), "SliceThickness : " + m_DicomVolume.ZPixelSpacing().ToString("F5"));
        GUI.Label(new Rect(60, 260, 600, 20), "WindowWidth : " + m_DicomVolume.DefaultWindowWidth());
        GUI.Label(new Rect(60, 280, 600, 20), "WindowCenter : " + m_DicomVolume.DefaultWindowCenter());
        string modality;
        if (!m_DicomVolume.GetTagInfo(new DicomTag(Tags.Modality), out modality))
        {
            modality = "empty";
        }
        GUI.Label(new Rect(60, 300, 600, 20), "Modality : " + modality);
    }

    void OnValidate()
    {
        if (m_DicomVolume == null)
            return;

        UpdateImage();
    }


    void UpdateImage()
    {

        if (m_DicomVolume == null)
            return;


        // Get image
        IDicomImage image = null;
        switch (m_MPR_Type)
        {
            case MPRType.Axial:
                if (m_Index < 0 || m_DicomVolume.Depth <= m_Index)
                    return;

                image = m_DicomVolume.ToDicomImageAxial(m_Index);
                break;
            case MPRType.Coronal:
                if (m_Index < 0 || m_DicomVolume.Height <= m_Index)
                    return;

                image = m_DicomVolume.ToDicomImagelCoronal(m_Index);
                break;
            case MPRType.Sagittal:
                if (m_Index < 0 || m_DicomVolume.Width <= m_Index)
                    return;

                image = m_DicomVolume.ToDicomImagelSagittal(m_Index);
                break;
        }

        // Generate image display plane mesh in Unity coordinate
        GeneratePlane((float)(image.Width * image.PixelSpacing[0] * 0.001), (float)(image.Height * image.PixelSpacing[1] * 0.001), m_MPR_Type);

        // Translate image position in Unity coordinate
        transform.position = image.ImageCenterPosition * 0.001f;

        // Update texture
        {
            m_Texture.Resize(image.Width, image.Height);
            m_Texture.SetPixels32(image.GeneratePixels(m_WindowWidth, m_WindowCenter));
            //m_Texture.SetPixels32(image.GeneratePixels()); // Use default WindowWidth, WindowCenter
            m_Texture.Apply();
            // Set image to the material
            GetComponent<Renderer>().material.SetTexture("_MainTex", m_Texture);
        }
    }

    void GeneratePlane(float image_width_range, float image_height_range, MPRType type)
    {

        // Get(Add) mesh component
        var meshFilter = gameObject.GetComponent<MeshFilter>();
        var meshRenderer = gameObject.GetComponent<MeshRenderer>();
        if (meshFilter == null)
        {

            meshFilter = gameObject.AddComponent<MeshFilter>();
            meshRenderer = gameObject.AddComponent<MeshRenderer>();

        }

        // Generate mesh
        Mesh m = new Mesh();
        {
            m.name = name;

            int[] triangles = new int[]
            {
                0, 1, 2,
                2, 3, 0
            };
            Vector2[] uv = new Vector2[]
            {
                new Vector2(0.0f, 0.0f),
                new Vector2(1.0f, 0.0f),
                new Vector2(1.0f, 1.0f),
                new Vector2(0.0f, 1.0f)
            };

            Vector3[] vertices;
            switch (type)
            {
                default:
                case MPRType.Axial:
                    vertices = new Vector3[]
                    {
                            new Vector3(-image_width_range * 0.5f, -image_height_range * 0.5f, 0.0f),
                            new Vector3( image_width_range * 0.5f, -image_height_range * 0.5f, 0.0f),
                            new Vector3( image_width_range * 0.5f,  image_height_range * 0.5f, 0.0f),
                            new Vector3(-image_width_range * 0.5f,  image_height_range * 0.5f, 0.0f)
                    };

                    break;
                case MPRType.Coronal:
                    vertices = new Vector3[]
                    {
                            new Vector3(-image_width_range * 0.5f, 0.0f, -image_height_range * 0.5f),
                            new Vector3( image_width_range * 0.5f, 0.0f, -image_height_range * 0.5f),
                            new Vector3( image_width_range * 0.5f, 0.0f,  image_height_range * 0.5f),
                            new Vector3(-image_width_range * 0.5f, 0.0f,  image_height_range * 0.5f)
                    };

                    break;
                case MPRType.Sagittal:
                    vertices = new Vector3[]
                    {
                            new Vector3(0.0f, -image_width_range * 0.5f, -image_height_range * 0.5f),
                            new Vector3(0.0f, image_width_range * 0.5f, -image_height_range * 0.5f),
                            new Vector3(0.0f, image_width_range * 0.5f,  image_height_range * 0.5f),
                            new Vector3(0.0f, -image_width_range * 0.5f,  image_height_range * 0.5f)
                    };

                    break;
            }

            m.vertices = vertices;
            m.triangles = triangles;
            m.uv = uv;
            m.RecalculateNormals();
            m.RecalculateBounds();
        }

        Destroy(meshFilter.mesh);

        meshFilter.mesh = m;
        meshRenderer.material = m_Material;
    }

    void FindDicomVolume()
    {
        var dicomloader = gameObject.GetComponent<DicomVolumeLoaderBehaviour>();
        if (dicomloader == null)
        {
            dicomloader = gameObject.GetAncestorComponent<DicomVolumeLoaderBehaviour>();
            if (dicomloader == null)
                return;
        }

        m_DicomVolume = dicomloader.DicomVolume;

        if (m_DicomVolume == null)
            return;

        m_Texture = m_DicomVolume.ToTexture2D(0);
        m_WindowWidth = (int)m_DicomVolume.DefaultWindowWidth();
        m_WindowCenter = (int)m_DicomVolume.DefaultWindowCenter();

        UpdateImage();
    }

}
