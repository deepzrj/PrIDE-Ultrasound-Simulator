using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using KDicom;

public class DicomLoaderForUIImageWithWindowLevelBehaviour : MonoBehaviour {

    [SerializeField]
    string m_Dicom_filename;

    [SerializeField]
    bool m_Auto = false;

    [SerializeField]
    int m_WindowWidth;
    [SerializeField]
    int m_WindowCenter;

    IDicomImage m_DicomImage;
    float m_ElapsedTime = 0.0f;

    Texture2D m_Texture = null;

    // Use this for initialization
    void Start()
    {
        if (!System.IO.File.Exists(m_Dicom_filename))
            return;

        var img = DicomLoader.LoadDicomImage(m_Dicom_filename);
        if (img == null)
            return; // Fail to load

        m_DicomImage = img;
        m_WindowWidth = (int)m_DicomImage.DefaultWindowWidth();
        m_WindowCenter = (int)m_DicomImage.DefaultWindowCenter();

        UpdateImage();
    }

    // Update is called once per frame
    void Update()
    {
        if (m_DicomImage == null)
            return;

        if (m_Auto) {
            int thresh = 250;
            int dwc = (int)m_DicomImage.DefaultWindowCenter();
            int dww = (int)m_DicomImage.DefaultWindowWidth();
            m_ElapsedTime += Time.deltaTime * thresh / 5.0f;
            m_WindowCenter = dwc - (int)(m_ElapsedTime);
            m_WindowWidth = dww - (int)m_ElapsedTime*4;
            if (m_WindowCenter < dwc - thresh) {
                m_WindowCenter = dwc;
                m_WindowWidth = dww;
                m_ElapsedTime = 0.0f;
            }
        }

        UpdateImage();
    }

    void UpdateImage()
    {
        if (m_DicomImage == null)
            return;

        // Set texture to Image
        if (m_Texture == null) {
            m_Texture = m_DicomImage.ToTexture2D(m_WindowWidth, m_WindowCenter);
        }
        else {
            m_Texture.SetPixels32(m_DicomImage.GeneratePixels(m_WindowWidth, m_WindowCenter));
            m_Texture.Apply();
        }
        Sprite texture_sprite = Sprite.Create(m_Texture, new Rect(0, 0, m_Texture.width, m_Texture.height), Vector2.zero);
        this.GetComponent<Image>().sprite = texture_sprite;

        // Change local scale
        if (m_Texture.width < m_Texture.height) {
            this.transform.localScale = new Vector3((float)m_Texture.width / (float)m_Texture.height, 1.0f, 1.0f);
        }
        else {
            this.transform.localScale = new Vector3(1.0f, (float)m_Texture.height / (float)m_Texture.width, 1.0f);
        }
    }
}
