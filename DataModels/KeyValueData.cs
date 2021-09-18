using System.Collections.Generic;

namespace Average.Server.DataModels
{
    public class KeyValueData
    {
        public class KeyValue
        {
            public string Key { get; set; }
            public dynamic Value { get; set; }
        }

        public class ObjectArgument
        {
            private readonly object _value;

            public ObjectArgument(object value)
            {
                _value = value;
            }

            public static implicit operator byte(ObjectArgument value) => value;
            public static implicit operator sbyte(ObjectArgument value) => value;
            public static implicit operator int(ObjectArgument value) => value;
            public static implicit operator uint(ObjectArgument value) => value;
            public static implicit operator short(ObjectArgument value) => value;
            public static implicit operator ushort(ObjectArgument value) => value;
            public static implicit operator long(ObjectArgument value) => value;
            public static implicit operator ulong(ObjectArgument value) => value;
            public static implicit operator float(ObjectArgument value) => value;
            public static implicit operator double(ObjectArgument value) => value;
            public static implicit operator char(ObjectArgument value) => value;
            public static implicit operator bool(ObjectArgument value) => value;
            public static implicit operator string(ObjectArgument value) => value;
            public static implicit operator decimal(ObjectArgument value) => value;

            public static explicit operator ObjectArgument(byte value) => new ObjectArgument(value);
            public static explicit operator ObjectArgument(sbyte value) => new ObjectArgument(value);
            public static explicit operator ObjectArgument(int value) => new ObjectArgument(value);
            public static explicit operator ObjectArgument(uint value) => new ObjectArgument(value);
            public static explicit operator ObjectArgument(short value) => new ObjectArgument(value);
            public static explicit operator ObjectArgument(ushort value) => new ObjectArgument(value);
            public static explicit operator ObjectArgument(long value) => new ObjectArgument(value);
            public static explicit operator ObjectArgument(ulong value) => new ObjectArgument(value);
            public static explicit operator ObjectArgument(float value) => new ObjectArgument(value);
            public static explicit operator ObjectArgument(double value) => new ObjectArgument(value);
            public static explicit operator ObjectArgument(char value) => new ObjectArgument(value);
            public static explicit operator ObjectArgument(bool value) => new ObjectArgument(value);
            public static explicit operator ObjectArgument(string value) => new ObjectArgument(value);
            public static explicit operator ObjectArgument(decimal value) => new ObjectArgument(value);
        }

        public long CharacterId { get; set; }
        public CharacterData Character { get; set; }

        public Dictionary<string, ObjectArgument> Values { get; set; }
    }
}
