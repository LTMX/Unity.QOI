﻿using UnityEditor;
using UnityEngine;

public static class ImageSaveUtility
{
    // Opens The Save File Panel and saves the texture to the selected path
    public static void SaveToFile(this Texture2D texture, TextureEncodingFormat format)
    {
        var path = EditorUtility.SaveFilePanel("Save Texture", "", texture.name, format.ToString().ToLower());
        if (!string.IsNullOrEmpty(path)) texture.EncodeTo(format).WriteAllBytes(path);
    }
    
    private static readonly string outputFolder = "QOI_Output";

    /// <summary>
    /// Saves the texture to a file in the project's RenderOutput folder
    /// </summary>
    /// <param name="t"></param>
    /// <param name="format"></param>
    public static void SaveToQOIFile(this Texture2D t, TextureEncodingFormat format)
    {
        var dirPath = Application.dataPath + "/" + outputFolder;
        dirPath.CreateDirectoryIfVoid();

        var fileName = "/QOI_" + t.name + ImageProcessing.FileExtensions.QOI;
        var filePath = dirPath + fileName;

        var bytes = t.EncodeTo(format);
        var byteCount = bytes.WriteAllBytes(filePath);
        
        #if UNITY_EDITOR
        Debug.Log(byteCount / 1024 + "Kb was saved as: " + filePath);
        #endif
        
        // Focuses the file in the project window and highlights it
        Selection.activeObject = AssetImporterExtensions.LoadAtPath<Texture2D>("Assets/" + outputFolder + fileName);
        EditorGUIUtility.PingObject( Selection.activeObject );
        
        // Refresh the project window
        AssetDatabase.SaveAssets();
        #if UNITY_EDITOR
        AssetDatabase.Refresh();
        #endif
    }
    
    
    public static byte[] EncodeTo(this Texture2D t, TextureEncodingFormat format)
    {
        return format switch
        {
            TextureEncodingFormat.QOI => t.EncodeToQOI(),
            TextureEncodingFormat.PNG => t.EncodeToPNG(),
            TextureEncodingFormat.JPG => t.EncodeToJPG(),
            TextureEncodingFormat.EXR => t.EncodeToEXR(),
            TextureEncodingFormat.TGA => t.EncodeToTGA(),
            // BMP => t.EncodeToBMP(),
            // HDR => t.EncodeToHDR(),
            // TIFF => t.EncodeToTIFF(),
            // GIF => t.EncodeToGIF(),
            // HEIF => t.EncodeToHEIF(),
            _ => t.EncodeToPNG()
        };
    }

    public enum TextureEncodingFormat
    {
        QOI, PNG, JPG, EXR, TGA //BMP, HDR, TIFF, GIF, HEIF
    }
}