using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using KDicom;

public class DicomLoaderForUIImageWithDirectorypathBehaviour : MonoBehaviour {

    [SerializeField]
    string m_Dicom_directorypath;
    [SerializeField]
    int m_ImageIndex;

    [SerializeField]
    bool m_Auto = false;
    [SerializeField]
    float m_WaitTime = 2.0f;


    string[] m_Dicom_filenames;
    float m_ElapsedTime = 0.0f;

    Texture2D m_Texture = null;

    // Use this for initialization
    void Start()
    {
        // Check exist
        if (!System.IO.Directory.Exists(m_Dicom_directorypath))
            return;

        var files = System.IO.Directory.GetFiles(m_Dicom_directorypath);
        m_Dicom_filenames = files;

        UpdateImage();
    }

    // Update is called once per frame
    void Update()
    {
        if (m_Auto) {
            m_ElapsedTime += Time.deltaTime * m_Dicom_filenames.Length / 5.0f;
            m_ImageIndex = (int)m_ElapsedTime;
            if (m_ImageIndex >= m_Dicom_filenames.Length) {
                m_ImageIndex = 0;
                m_ElapsedTime = 0.0f;
            }
        }

        UpdateImage();
    }

    void UpdateImage()
    {
        if (m_ImageIndex < 0 || m_ImageIndex >= m_Dicom_filenames.Length)
            return;

        if (!System.IO.File.Exists(m_Dicom_filenames[m_ImageIndex]))
            return;

        var img = DicomLoader.LoadDicomImage(m_Dicom_filenames[m_ImageIndex]);

        // Failed to load
        if (img == null) {
            return;
        }

        // Set texture to Image
        if( m_Texture == null ) {
            m_Texture = img.ToTexture2D();
        }
        else {
            m_Texture.Resize(img.Width, img.Height);
            m_Texture.SetPixels32(img.GeneratePixels());
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
