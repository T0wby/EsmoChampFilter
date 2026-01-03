using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace EsmoChamps.Utility
{
    public static class ImageManager
    {
        public static readonly string UserImagesFolder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            "EsmoChamps",
            "ChampionImages"
        );

        private static readonly string AssetImagesFolder = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "Assets",
            "ChampionImages"
        );

        /// <summary>
        /// Initializes the user images folder and copies default images from Assets
        /// </summary>
        public static void InitializeImageFolder()
        {
            if (!Directory.Exists(UserImagesFolder))
            {
                Directory.CreateDirectory(UserImagesFolder);
            }

            if (Directory.Exists(AssetImagesFolder))
            {
                CopyDefaultImages();
            }

            CreateReadmeFile();
        }

        private static void CopyDefaultImages()
        {
            try
            {
                var assetImages = Directory.GetFiles(AssetImagesFolder, "*.*", SearchOption.TopDirectoryOnly)
                    .Where(file => IsImageFile(file));

                foreach (var sourceFile in assetImages)
                {
                    string fileName = Path.GetFileName(sourceFile);
                    string targetFile = Path.Combine(UserImagesFolder, fileName);

                    // Only copy if the file doesn't exist in user folder
                    // This allows users to customize their images without them being overwritten
                    if (!File.Exists(targetFile))
                    {
                        File.Copy(sourceFile, targetFile, false);
                        System.Diagnostics.Debug.WriteLine($"Copied default image: {fileName}");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error copying default images: {ex.Message}");
            }
        }

        private static bool IsImageFile(string filePath)
        {
            string extension = Path.GetExtension(filePath).ToLower();
            return extension == ".png" || extension == ".jpg" || extension == ".jpeg" || extension == ".bmp";
        }

        private static void CreateReadmeFile()
        {
            string readmePath = Path.Combine(UserImagesFolder, "README.txt");

            if (!File.Exists(readmePath))
            {
                string readmeContent = @"Champion Images Folder
=====================

This folder contains images for your champions.

HOW TO ADD/CHANGE IMAGES:
1. Name your image file exactly as the champion name (e.g., 'Pembroke.png', 'Warper.jpg')
2. Place the image in this folder
3. Supported formats: .png, .jpg, .jpeg, .bmp
4. Recommended size: 512x512 pixels or larger (square images work best)

NOTES:
- Default images were copied from the application's Assets folder on first run
- You can replace any image by simply overwriting the file
- Images are loaded automatically when viewing champions
- If an image is missing, the champion's first letter will be shown instead

Location: " + UserImagesFolder;

                try
                {
                    File.WriteAllText(readmePath, readmeContent);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error creating README: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Gets the full path to a champion image
        /// </summary>
        public static string GetImagePath(string imageName)
        {
            if (string.IsNullOrEmpty(imageName))
                return null;

            string fullPath = Path.Combine(UserImagesFolder, imageName);
            return File.Exists(fullPath) ? fullPath : null;
        }

        /// <summary>
        /// Copies an external image into the user images folder
        /// </summary>
        public static bool CopyImageToFolder(string sourceFilePath, out string fileName)
        {
            fileName = null;

            try
            {
                if (!File.Exists(sourceFilePath))
                    return false;

                fileName = Path.GetFileName(sourceFilePath);
                string targetPath = Path.Combine(UserImagesFolder, fileName);

                File.Copy(sourceFilePath, targetPath, true);
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error copying image: {ex.Message}");
                return false;
            }
        }
    }
}
