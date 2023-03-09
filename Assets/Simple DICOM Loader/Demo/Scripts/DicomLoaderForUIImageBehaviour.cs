using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using KDicom;

public class DicomLoaderForUIImageBehaviour : MonoBehaviour {

    [SerializeField]
    string m_Dicom_filename;

    string m_Modality;

    // Use this for initialization
    void Start () {
        UpdateImage();
    }

    // Update is called once per frame
    void Update () {
    }

    void UpdateImage()
    {
        if (!System.IO.File.Exists(m_Dicom_filename))
            return;

        // Load DICOM in UI Image coordinate
        var img = DicomLoader.LoadDicomImage(m_Dicom_filename);

        // Failed to load
        if (img == null) {
            Debug.Log("failed to load " + m_Dicom_filename);
            return;
        }

        img.GetTagInfo(new DicomTag(Tags.Modality), out m_Modality);

        // Set texture to UI Image
        var tex = img.ToTexture2D();
        Sprite texture_sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.zero);
        this.GetComponent<Image>().sprite = texture_sprite;

        // Change local scale
        if (tex.width < tex.height) {
            this.transform.localScale = new Vector3((float)tex.width / (float)tex.height, 1.0f, 1.0f);
        }
        else {
            this.transform.localScale = new Vector3(1.0f, (float)tex.height / (float)tex.width, 1.0f);
        }
    }
}
