﻿/**
 * @File   : Sein_imageBasedLightingExtension.cs
 * @Author : dtysky (dtysky@outlook.com)
 * @Link   : dtysky.moe
 * @Date   : 2019/10/13 0:00:00PM
 */
using GLTF.Math;
using GLTF.Schema;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace SeinJS
{
    public class Sein_imageBasedLightingExtension : Extension
    {
        public struct Light
        {
            public float[][] shCoefficients;
            public float diffuseIntensity;
            public int brdfLUT;
            public int[] specMapFaces;
            public float specIntensity;
        }

        public bool isGlobal = false;

        // global
        public List<Light> lights = new List<Light>();

        // each
        public int iblIndex = -1;
        public int iblType = 2;

        public JProperty Serialize()
        {
            var value = new JObject();

            if (isGlobal)
            {
                var eLights = new JArray();
                value.Add("lights", eLights);
                foreach (var light in lights)
                {
                    var l = new JObject();
                    var coefficients = new JArray();
                    l.Add("diffuse", new JObject(
                        new JProperty("type", "SH"),
                        new JProperty("intensity", light.diffuseIntensity),
                        new JProperty("coefficients", coefficients)
                    ));

                    for (int i = 0; i < light.shCoefficients.Length; i += 1)
                    {
                        var cs = light.shCoefficients[i];
                        coefficients.Add(new JArray { cs[0], cs[1], cs[2] });
                    }

                    l.Add("specular", new JObject(
                        new JProperty("type", "CUBE"),
                        new JProperty("intensity", light.specIntensity),
                        new JProperty("brdfLUT", light.brdfLUT),
                        new JProperty("faces", JArray.FromObject(light.specMapFaces))
                    ));

                    eLights.Add(l);
                }
            }
            else
            {
                value.Add("light", iblIndex);
                value.Add("type", iblType == 1 ? "SPECULAR" : "ALL");
            }

            return new JProperty(ExtensionManager.GetExtensionName(typeof(Sein_imageBasedLightingExtensionFactory)), value);
        }
    }
}
