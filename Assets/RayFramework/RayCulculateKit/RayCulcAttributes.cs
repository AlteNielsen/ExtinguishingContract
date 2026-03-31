using System;

namespace Ray.Culc
{
    [AttributeUsage(AttributeTargets.Field)]
    public class RayInputAttribute : Attribute
    {
        public string fieldName { get; }
        public int index { get; }

        public RayInputAttribute(int x, string name)
        {
            fieldName = name;
            index = x;
        }
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class RayRelayAttribute : Attribute
    {
        public string fieldName { get; }
        public int index { get; }

        public RayRelayAttribute(int x, string name)
        {
            fieldName = name;
            index = x;
        }
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class RayOutputAttribute : Attribute
    {
        public string fieldName { get; }
        public int index { get; }

        public RayOutputAttribute(int x, string name)
        {
            fieldName = name;
            index = x;
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class RayCulcAttribute : Attribute
    {
        public string[] input { get; }
        public string output { get; }

        public RayCulcAttribute(string _output)
        {
            output = _output;
        }

        public RayCulcAttribute(string _output, string _input1)
        {
            output = _output;
            input = new string[] { _input1 };
        }

        public RayCulcAttribute(string _output, string _input1, string _input2)
        {
            output = _output;
            input = new string[] { _input1, _input2 };
        }

        public RayCulcAttribute(string _output, string _input1, string _input2, string _input3)
        {
            output = _output;
            input = new string[] { _input1, _input2, _input3 };
        }
    }
}
