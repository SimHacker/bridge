using System;
using System.IO;
using UnityEngine;

namespace UniWebServer
{
    public static class Helper
    {
        public static string GetFolderRoot(string folderPath) {
            Debug.Log("UniWebServer.Helper.GetFolderRoot: folderPath: " + folderPath);
            string folderName = Path.GetDirectoryName(folderPath);

            if (folderName == null) {
                folderName = "/";
            }
            Debug.Log("UniWebServer.Helper.GetFolderRoot: folderName: " + folderName);
            if (folderName.Length <= 2) return folderName;
            var idx = folderName.IndexOf("/", 1, StringComparison.InvariantCulture);
            return idx != -1 ? folderName.Substring(0, idx) : folderName;
        }
    }
}
