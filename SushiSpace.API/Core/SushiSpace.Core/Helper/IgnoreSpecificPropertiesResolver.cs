using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SushiSpace.Core.Helper
{
    public class IgnoreSpecificPropertiesResolver : DefaultContractResolver
    {
        private readonly HashSet<Type> _typesToIgnore = new HashSet<Type>();

        public IgnoreSpecificPropertiesResolver(params Type[] typesToIgnore)
        {
            foreach (var type in typesToIgnore)
            {
                _typesToIgnore.Add(type);
            }
        }

        protected override List<MemberInfo> GetSerializableMembers(Type objectType)
        {
            var members = base.GetSerializableMembers(objectType);
            return members.Where(m => !_typesToIgnore.Contains(m.DeclaringType)).ToList();
        }
    }
}
