//----------------------------------------------------
// Simple DICOM Loader
// Copyright (c) 2018 Kompath, Inc.
// Version 1.0.0
//----------------------------------------------------

Simple DICOM Loader(SDL) is an all-in-one asset, it can easily load and display DICOM images and files.

**** Features ****
- This asset makes it possible to easily develop a DICOM-related software.
- This asset includes several sample scenes, scripts, and gimics which are useful for your development life.
- Furthermore, the tool can create Axial, Coronal and Sagittal MPR images from CT, MR or DICOM files which you select.

**** Main class ****
- DicomLoader: class for loading DICOM
- IDicomImage: class for DICOM image and Tag data
- IDicomVolume: class for DICOM volume and Tag data
- DicomTag: class for DICOM tag to get tag value from group and element number

**** How to use and some code sample ****

[A] Using DICOM image as 2D image for UI Image
(1) Prepare a DICOM image.
(2) Load DICOM image using DicomLoader class:
 
	<Code Sample> 
     var dicom_image = DicomLoader.LoadDicomImage((string)filename);

(3) Get Texture2D from "dicom_image"
 
	<Code Sample> 
     var texture = dicom_image.ToTexture2D();

[B] Using DICOM images as 3D volume in world coordinate
(1) Prepare DICOM images.
(2) Load DICOM volumes using DicomLoader class:
 
 	<Code Samples>
     var dicom_volumes = DicomLoader.LoadDicomVolumes((string) directory_path);
     (or)
     var dicom_volumes = DicomLoader.LoadDicomVolumes((string[]) filenames);

(3) Choose a volume you want to get:

	<Code Sample>
     var dicom_volume = dicom_volumes[0];

(4) Get the Multi Planar Reconstruction(MPR) image. Sample codes to create Axial, Coronal, and Sagittal image are shown below.

	<Code Samples>
     var axial_image = dicom_volume.ToDicomImageAxial((int)index);
     var coronal_image = dicom_volume.ToDicomImageCoronal((int)index);
     var sagittal_image = dicom_volume.ToDicomImageSagittal((int)index);

(5) Get Texture2D from MPR images.

	<Code Samples>
     var axial_texture = axial_image.ToTexture2D();
     var coronal_texture = coronal_image.ToTexture2D();
     var sagittal_texture = sagittal_image.ToTexture2D();

(6) Get Texture2D from volume directly as an Axial image.

	<Code Sample>
     var axial_texture = dicom_volume.ToTexture2D((int)index);

[C] Common functions in image and volume
(1) Get DICOM tag elements, we show code sample to extract "Modality" tag as below.

	<Code Samples>
     string modality;
     dicom_image.GetTagInfo(new DicomTag(Tags.Modality), out modality)
     (or)
     dicom_volume.GetTagInfo(new DicomTag(Tags.Modality), out modality)

(2) Get an image with different contrast and brightness value at getting Texture2D.

	<Code Samples> 
     var texture = dicom_image.ToTexture2D((int)window_width, (int)window_center);
     var texture = dicom_volume.ToTexture2D((int)index, (int)window_width, (int)window_center);

Version:
1.0: First release

Target platforms:
 Windows (64bit)





