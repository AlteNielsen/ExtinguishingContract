using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Ray.Culc
{
    public class RayCulcManager<T> where T : RayAbstractCulculator, new()
    {
        private T culculator;

        private Dictionary<string, int> index = new();

        private float[] masterBuffer;
        private int inputLength;
        private int relayLength;
        private int outputLength;

        private RayCulcMethod[] methods;

        public RayCulcManager()
        {
            culculator = new T();
            inputLength = InputSetup(0);
            relayLength = RelaySetup(inputLength);
            outputLength = OutputSetup(inputLength + relayLength);
            masterBuffer = new float[inputLength + relayLength +  outputLength];
            MethodSetup();
            index = null;
        }

        public void Culculate(float[] input, float[] output)
        {
            Array.Copy(input, masterBuffer, inputLength);
            Execute();
            Array.Copy(masterBuffer, inputLength + relayLength, output, 0, outputLength);
        }

        private void Execute()
        {
            for (int i = 0; i < methods.Length; i++)
            {
                RayCulcMethod method = methods[i];
                int inputLength = method.inputIndexes.Length;
                switch (inputLength)
                {
                    case 0:
                        masterBuffer[method.outputIndex] = ((Func<float>)method.action)();
                        break;
                    case 1:
                        masterBuffer[method.outputIndex] = ((Func<float, float>)method.action)(masterBuffer[method.inputIndexes[0]]);
                        break;
                    case 2:
                        masterBuffer[method.outputIndex] = ((Func<float, float, float>)method.action)(masterBuffer[method.inputIndexes[0]], masterBuffer[method.inputIndexes[1]]);
                        break;
                    case 3:
                        masterBuffer[method.outputIndex] = ((Func<float, float, float, float>)method.action)(masterBuffer[method.inputIndexes[0]], masterBuffer[method.inputIndexes[1]], masterBuffer[method.inputIndexes[2]]);
                        break;
                }
            }
        }

        private int InputSetup(int offset)
        {
            FieldInfo[] fields = culculator.GetType().GetFields()
                .Where(m => m.GetCustomAttribute<RayInputAttribute>() != null)
                .OrderBy(f => f.GetCustomAttribute<RayInputAttribute>().index)
                .ToArray();
            for (int i = 0; i < fields.Length; i++)
            {
                RayInputAttribute attr = fields[i].GetCustomAttribute<RayInputAttribute>();
                index[attr.fieldName] = i + offset;
            }
            return fields.Length;
        }

        private int RelaySetup(int offset)
        {
            FieldInfo[] fields = culculator.GetType().GetFields()
                .Where(m => m.GetCustomAttribute<RayRelayAttribute>() != null)
                .OrderBy(f => f.GetCustomAttribute<RayRelayAttribute>().index)
                .ToArray();
            for (int i = 0; i < fields.Length; i++)
            {
                RayRelayAttribute attr = fields[i].GetCustomAttribute<RayRelayAttribute>();
                index[attr.fieldName] = i + offset;
            }
            return fields.Length;
        }

        private int OutputSetup(int offset)
        {
            FieldInfo[] fields = culculator.GetType().GetFields()
                .Where(m => m.GetCustomAttribute<RayOutputAttribute>() != null)
                .OrderBy(f => f.GetCustomAttribute<RayOutputAttribute>().index)
                .ToArray();
            for (int i = 0; i < fields.Length; i++)
            {
                RayOutputAttribute attr = fields[i].GetCustomAttribute<RayOutputAttribute>();
                index[attr.fieldName] = i + offset;
            }
            return fields.Length;
        }
        private void MethodSetup()
        {
            MethodInfo[] methodInfo = culculator.GetType().GetMethods()
                .Where(m => m.GetCustomAttribute<RayCulcAttribute>() != null)
                .ToArray(); ;
            methods = new RayCulcMethod[methodInfo.Length];
            for (int i = 0; i < methodInfo.Length; i++)
            {
                RayCulcAttribute attr = methodInfo[i].GetCustomAttribute<RayCulcAttribute>();
                Type delegateType = Expression.GetDelegateType(methodInfo[i].GetParameters().Select(p => p.ParameterType).Append(methodInfo[i].ReturnType).ToArray());
                methods[i].action = Delegate.CreateDelegate(delegateType, culculator, methodInfo[i]);
                methods[i].inputIndexes = new int[attr.input.Length];
                for (int j = 0; j < attr.input.Length; j++)
                {
                    methods[i].inputIndexes[j] = index[attr.input[j]];
                }
                methods[i].outputIndex = index[attr.output];
            }
            TopologicalSort();
        }

        private void TopologicalSort()
        {
            List<int> outputs = new List<int>();
            for (int i = 0; i < methods.Length; i++)
            {
                outputs.Add(methods[i].outputIndex);
            }
            int[] o = outputs.Distinct().ToArray();
            if (o.Length != outputs.Count)
            {
                throw new Exception("一つのデータに書き込むメソッドは一つにしてください");
            }

            List<RayCulcMethod> sortedList = new List<RayCulcMethod>();
            List<int> sortedIndex = new List<int>();

            while (sortedList.Count < methods.Length)
            {
                int[] queue = new int[methods.Length];
                for (int i = 0; i < queue.Length; i++)
                {
                    for (int j = 0; j < methods[i].inputIndexes.Length; j++)
                    {
                        for (int k = 0; k < outputs.Count; k++)
                        {
                            if (methods[i].inputIndexes[j] == outputs[k])
                            {
                                queue[i]++;
                            }
                        }
                    }
                }

                bool isLoop = true;
                for (int i = 0; i < queue.Length; i++)
                {
                    if (queue[i] == 0 && !sortedIndex.Contains(i))
                    {
                        sortedList.Add(methods[i]);
                        outputs.Remove(methods[i].outputIndex);
                        sortedIndex.Add(i);
                        isLoop = false;
                    }
                }

                if (isLoop)
                {
                    throw new Exception("循環参照をやめましょう");
                }
            }

            methods = sortedList.ToArray();
        }

        private struct RayCulcMethod
        {
            public Delegate action;
            public int[] inputIndexes;
            public int outputIndex;
        }
    }

    public abstract class RayAbstractCulculator
    {
        
    }
}
