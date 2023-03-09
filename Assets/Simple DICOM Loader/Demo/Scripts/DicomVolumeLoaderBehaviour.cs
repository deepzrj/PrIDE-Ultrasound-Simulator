using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KDicom;
using System.Linq;

public class DicomVolumeLoaderBehaviour : MonoBehaviour
{
    [SerializeField]
    string m_DicomDirectoryPath;

    IDicomVolume m_DicomVolume;
    public IDicomVolume DicomVolume { get { return m_DicomVolume; } }

    // Use this for initialization
    void Start()
    {

        // Load Dicom files
        var files = System.IO.Directory.GetFiles(m_DicomDirectoryPath).Where(n => n.Substring(n.Length - 4) != "meta").ToArray();
        var volumes = DicomLoader.LoadDicomVolumes(files);
        int depthmax = 0;
        foreach (var vol in volumes)
        {
            if (vol.Depth > depthmax)
            {
                depthmax = vol.Depth;
                m_DicomVolume = vol;
            }
        }
        // Dispose unused volumes
        foreach (var vol in volumes)
        {
            if (vol != m_DicomVolume)
            {
                vol.Dispose();
            }
        }

        if (m_DicomVolume == null)
            return;

        // Initialize WindowWidth, WindowCenter
        {
            var ww = (int)m_DicomVolume.DefaultWindowWidth();
            if (ww == 0 && files.Length > 0)
            {
                var filename = files[files.Length / 2];
                var slice = DicomLoader.LoadDicomImage(filename);
                ww = (int)slice.DefaultWindowWidth();
                var wc = (int)slice.DefaultWindowCenter();
                if (ww != 0)
                {
                    m_DicomVolume.SetTagInfo(new DicomTag(Tags.WindowCenter), wc.ToString());
                    m_DicomVolume.SetTagInfo(new DicomTag(Tags.WindowWidth), ww.ToString());
                }
            }
        }



        // Initialize MainCamera position
        {
            // Culclate volume center position in Unity coordinate
            var volume_centerpos = m_DicomVolume.ToDicomImageAxial(m_DicomVolume.Depth / 2).ImageCenterPosition * 0.001f;
            Camera.main.transform.rotation = Quaternion.Euler(-65.0f, -80.0f, 80.0f);
            Camera.main.transform.position = volume_centerpos + new Vector3(0.08f, -0.35f, -0.05f);
            Camera.main.GetComponent<CameraOperationBehaviour>().CenterPos = volume_centerpos;
        }
    }

    private void OnDestroy()
    {
        if (m_DicomVolume != null)
        {
            m_DicomVolume.Dispose();
            System.GC.Collect();
            Resources.UnloadUnusedAssets();
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
