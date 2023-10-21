using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Utility.Runtime
{
    public abstract class EnumBaseType<T> where T : EnumBaseType<T>
    {
        protected static List<T> _enumTypes = new List<T>();

        public int key;
        public string value;
        
        protected EnumBaseType(int key, string value)
        {
            this.key = key;
            this.value = value;
            _enumTypes.Add((T)this);
        }

        public static ReadOnlyCollection<T> GetAllEnumTypes()
        {
            return _enumTypes.AsReadOnly();
        }

        public static T GetEnumType(int key)
        {
            foreach (T enumType in _enumTypes)
            {
                if (enumType.key == key)
                    return enumType;
            }

            return null;
        }

        public override string ToString()
        {
            return value;
        }
    }
}