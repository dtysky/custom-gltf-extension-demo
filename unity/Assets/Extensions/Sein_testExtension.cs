using GLTF.Schema;
using Newtonsoft.Json.Linq;

namespace SeinJS
{
    public class Sein_testExtension : Extension
    {
        public float rotateSpeed;

        public JProperty Serialize()
        {
            return new JProperty(ExtensionManager.GetExtensionName(typeof(Sein_testExtensionFactory)), new JObject(
                new JProperty("rotateSpeed", rotateSpeed)
            ));
        }
    }
}
