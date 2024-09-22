using System.Reflection;

namespace RpcServer.Framework
{
    public static class RpcHelper
    {
        public static T Map<T>(Dictionary<string, object> inDict) where T : class, new()
        {
            var outObj = new T();

            var outObjType = typeof(T);
            foreach (var eachPair in inDict)
            {
                var eachProp = outObjType.GetProperty(eachPair.Key, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (eachProp == null)
                {
                    continue;
                }

                var eachPropType = eachProp.PropertyType;
                var eachValueType = eachPair.Value.GetType();
                if (eachPropType.IsAssignableFrom(eachValueType)) // 프로퍼티 타입과 값 타입이 동일한 경우
                {
                    eachProp.SetValue(outObj, eachPair.Value);
                }
                else // 프로퍼티 타입과 값 타입이 다른 경우
                {
                    var eachValueStr = eachPair.Value.ToString(); // CHECKME: 퍼포먼스 
                    var eachConvertedValue = Convert.ChangeType(eachValueStr, eachPropType);
                    eachProp.SetValue(outObj, eachConvertedValue);
                }
            }
            return outObj;
        }

        public static T TouchCollection<T>(ref Dictionary<string, object> refDict, string inKey) where T : class, new()
        {
            if (!refDict.TryGetValue(inKey, out var oldObj))
            {
                var newObj = new T();
                refDict.Add(inKey, newObj);
                return newObj;
            }
            return (T)oldObj;
        }
    }
}
