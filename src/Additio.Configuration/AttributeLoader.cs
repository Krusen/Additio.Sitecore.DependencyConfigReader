using System.Collections.Generic;
using System.Xml.Linq;

namespace Additio.Configuration
{
    public interface IAttributeLoader
    {
        IEnumerable<XAttribute> GetAttributesOnRootElement(string file);
    }

    public class AttributeLoader : IAttributeLoader
    {
        public IEnumerable<XAttribute> GetAttributesOnRootElement(string file)
        {
            return XElement.Load(file).Attributes();
        }
    }
}
