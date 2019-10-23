using System.Collections.Generic;
using UnityEngine;
using System;
using Newtonsoft.Json.Linq;
using GLTF.Schema;

namespace SeinJS
{
    public class Sein_testExtensionFactory : SeinExtensionFactory
    {
        public override string GetExtensionName() { return "Sein_test"; }
        public override List<Type> GetBindedComponents() { return new List<Type> { typeof(SeinTest) }; }

        public override void Serialize(ExporterEntry entry, Dictionary<string, Extension> extensions, UnityEngine.Object component = null)
        {
            var extension = new Sein_testExtension();
            var test = component as SeinTest;

            extension.rotateSpeed = test.rotateSpeed;

            AddExtension(extensions, extension);
        }

        public override Extension Deserialize(GLTFRoot root, JProperty extensionToken)
        {
            var extension = new Sein_testExtension();

            if (extensionToken != null)
            {
                extension.rotateSpeed = (float)extensionToken.Value["rotateSpeed"];
            }

            return extension;
        }

        public override void Import(EditorImporter importer, GameObject gameObject, Node gltfNode, Extension extension)
        {
            var test = (Sein_testExtension)extension;
            var testComponent = gameObject.AddComponent<SeinTest>();
            testComponent.rotateSpeed = test.rotateSpeed;
        }
    }
}
