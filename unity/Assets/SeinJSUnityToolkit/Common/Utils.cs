﻿/**
 * @File   : Utils.cs
 * @Author : dtysky (dtysky@outlook.com)
 * @Link   : dtysky.moe
 * @Date   : 2019/09/09 0:00:00PM
 */
using System;
using UnityEditor;
using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using GLTF;
using GLTF.Schema;

namespace SeinJS {
    public class Utils: Editor
    {
        public static bool inited = false;
        static System.Version newVersion = null;

        static string checlUrl = "https://api.github.com/repos/SeinJS/SeinUnityToolkit/git/refs/tags";
        static string url = "https://github.com/SeinJS/SeinUnityToolkit/tree/master/bin";

        static IEnumerator coroutine = null;

        public static IEnumerator CheckForUpdate()
        {
            using (UnityWebRequest www = UnityWebRequest.Get(checlUrl))
            {
                yield return www.SendWebRequest();

                while (!www.isDone)
                {
                    yield return 1;
                }

                if (www.isNetworkError || www.isHttpError)
                {
                    EditorUtility.DisplayDialog(
                        "Error occured while checking for update!",
                        www.error,
                        "OK"
                    );
                    coroutine = null;
                    yield break;
                }

                string response = www.downloadHandler.text;

                if (response == "")
                {
                    coroutine = null;
                    yield break;
                }

                var json = JsonConvert.DeserializeObject<JArray>(response);

                if (json.Count < 1)
                {
                    coroutine = null;
                    yield break;
                }

                var tags = ((string)json[json.Count - 1]["ref"]).Split('/');
                string tag = tags[tags.Length - 1].Replace("v", "");
                newVersion = new System.Version(tag);

                if (newVersion <= Config.Version)
                {
                    coroutine = null;
                    yield break;
                }

                if (EditorUtility.DisplayDialog(
                        "A new version 'v" + newVersion + "' is available",
                        "Download the last version to esure the best experience",
                        "Download",
                        "Cancel"
                    ))
                {
                    coroutine = null;
                    Application.OpenURL(url);
                }
            }
        }

        [MenuItem("SeinJS/Check for update", priority = 2)]
        public static void Init()
        {
            if (!inited)
            {
                EditorApplication.update += EditorUpdate;
            }

            if (coroutine == null)
            {
                coroutine = CheckForUpdate();
            }

            inited = true;
        }

        [MenuItem("SeinJS/Help", priority = 3)]
        public static void Help()
        {
            Application.OpenURL("http://seinjs.com/cn/guide/unity");
        }

        static void EditorUpdate()
        {
            if (coroutine != null)
            {
                coroutine.MoveNext();
            }
        }

        public static string MakeRelativePath(string fromPath, string toPath)
        {
            if (string.IsNullOrEmpty(fromPath)) throw new ArgumentNullException(nameof(fromPath));
            if (string.IsNullOrEmpty(toPath)) throw new ArgumentNullException(nameof(toPath));

            Uri fromUri = new Uri(fromPath + "/a.txt");
            Uri toUri = new Uri(toPath);

            if (fromUri.Scheme != toUri.Scheme) { return toPath; } // path can't be made relative.

            Uri relativeUri = fromUri.MakeRelativeUri(toUri);
            string relativePath = Uri.UnescapeDataString(relativeUri.ToString()).Replace("/a.txt", "");

            if (toUri.Scheme.Equals("file", StringComparison.InvariantCultureIgnoreCase))
            {
                relativePath = relativePath.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
            }

            return relativePath;
        }

        public static GLTF.Math.Vector3 ConvertVector3LeftToRightHandedness(Vector3 v)
        {
            return new GLTF.Math.Vector3(v.x, v.y, -v.z);
        }

        public static GLTF.Math.Vector4 ConvertVector4LeftToRightHandedness(Vector4 v)
        {
            return new GLTF.Math.Vector4(v.x, v.y, -v.z, -v.w);
        }

        public static GLTF.Math.Quaternion ConvertQuatLeftToRightHandedness(Quaternion q)
        {
            return new GLTF.Math.Quaternion(q.x, q.y, -q.z, -q.w);
        }

        public static Vector3 ConvertVector3LeftToRightHandedness(ref Vector3 v)
        {
            v.z = -v.z;

            return v;
        }

        public static Vector4 ConvertVector4LeftToRightHandedness(ref Vector4 v)
        {
            v.z = -v.z;
            v.w = -v.w;

            return v;
        }

        public static Quaternion ConvertQuatLeftToRightHandedness(ref Quaternion q)
        {
            q.z = -q.z;
            q.w = -q.w;

            return q;
        }

        public static GLTF.Math.Matrix4x4 ConvertMat4LeftToRightHandedness(Matrix4x4 mat)
        {
            ConvertMat4LeftToRightHandedness(ref mat);

            return new GLTF.Math.Matrix4x4 (
                mat.m00, mat.m01, mat.m02, mat.m03,
                mat.m10, mat.m11, mat.m12, mat.m13,
                mat.m20, mat.m21, mat.m22, mat.m23,
                mat.m30, mat.m31, mat.m32, mat.m33
            );
        }

        public static Matrix4x4 ConvertMat4LeftToRightHandedness(ref Matrix4x4 mat)
        {
            Vector3 position = mat.GetColumn(3);
            ConvertVector3LeftToRightHandedness(ref position);
            Quaternion rotation = Quaternion.LookRotation(mat.GetColumn(2), mat.GetColumn(1));
            ConvertQuatLeftToRightHandedness(ref rotation);

            Vector3 scale = new Vector3(mat.GetColumn(0).magnitude, mat.GetColumn(1).magnitude, mat.GetColumn(2).magnitude);
            float epsilon = 0.00001f;

            // Some issues can occurs with non uniform scales
            if (Mathf.Abs(scale.x - scale.y) > epsilon || Mathf.Abs(scale.y - scale.z) > epsilon || Mathf.Abs(scale.x - scale.z) > epsilon)
            {
                Debug.LogWarning("A matrix with non uniform scale is being converted from left to right handed system. This code is not working correctly in this case");
            }

            // Handle negative scale component in matrix decomposition
            if (Matrix4x4.Determinant(mat) < 0)
            {
                Quaternion rot = Quaternion.LookRotation(mat.GetColumn(2), mat.GetColumn(1));
                Matrix4x4 corr = Matrix4x4.TRS(mat.GetColumn(3), rot, Vector3.one).inverse;
                Matrix4x4 extractedScale = corr * mat;
                scale = new Vector3(extractedScale.m00, extractedScale.m11, extractedScale.m22);
            }

            // convert transform values from left handed to right handed
            mat.SetTRS(position, rotation, scale);

            return mat;
        }
    }
}
