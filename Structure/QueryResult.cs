using System;
using System.Collections.Generic;
using SnowFlakeGamesAssets.PiscesConfigLoader.Utils;
using SnowFlakeGamesAssets.PiscesConfigLoader.Utils.Expression;
using SnowFlakeGamesAssets.TaurusDungeonGenerator.Utils;
using UnityEngine;

namespace SnowFlakeGamesAssets.PiscesConfigLoader.Structure
{
    public class QueryResult
    {
        public ConfigPath Path { get; }
        public bool HasValue { get; } = true;

        private object _value;

        public QueryResult(object value, ConfigPath path)
        {
            _value = value;
            Path = path;
            if (value == null)
                HasValue = false;
        }

        public int AsInt() => int.Parse(AsString());
        public long AsLong() => long.Parse(AsString());
        public float AsFloat() => float.Parse(AsString());
        public double AsDouble() => double.Parse(AsString());

        public Vector3 AsVector3()
        {
            var str = AsString();
            var coordinateStrs = str.Split(' ');

            if (coordinateStrs.Length == 3)
                return new Vector3(
                    float.Parse(coordinateStrs[0]),
                    float.Parse(coordinateStrs[1]),
                    float.Parse(coordinateStrs[2]));
            if (coordinateStrs.Length == 2)
                return new Vector3(
                    float.Parse(coordinateStrs[0]),
                    float.Parse(coordinateStrs[1]));
            throw new Exception($"Unable to parse Vector3 from string: {str}");
        }

        public string AsString()
        {
            return _value as string ?? _value.ToString();
        }

        public ConfigPath AsPath()
        {
            var pathStr = AsString();

            if (pathStr == null)
                throw new ArgumentException($"Path string should not be null");
            if (pathStr.Contains(" "))
                throw new ArgumentException($"Path string [{pathStr}] should not contain spaces");

            return new ConfigPath(pathStr.Split('.'));
        }


        public ConfigNode AsNode() => new ConfigNode(_value, Path);

        public List<ConfigNode> AsNodeList()
        {
            var objects = _value as IList<object>;
            if (objects != null)
            {
                List<ConfigNode> result = new List<ConfigNode>();
                for (var index = 0; index < ((IList<object>) _value).Count; index++)
                {
                    var x = ((IList<object>) _value)[index];
                    result.Add(new ConfigNode(x, Path.Add(index.ToString())));
                }

                return result;
            }

            throw new Exception("Query result is a leaf, not a node!");
        }

        public ConfigExpressionBuilder AsExpression()
        {
            var asString = AsString();

            var expression = GameConfig.ExpressionParser.EvaluateExpression(asString);

            return new ConfigExpressionBuilder(expression);
        }

        public RangeI AsRangeI()
        {
            var asString = AsString();

            var nums = asString.Split('_');
            if (nums.Length == 2)
            {
                return new RangeI(int.Parse(nums[0]), int.Parse(nums[1]));
            }
            // a sima számokat is beparsoljuk
            if (nums.Length == 1)
            {
                return new RangeI(int.Parse(nums[0]), int.Parse(nums[0]));
            }
            
            throw new Exception($"Unable to parse RangeI: {asString}");

        }

        public Color AsColor()
        {
            var asString = AsString();
            return CollectionColorUtils.ParseColorFromHtml(asString);
        }


        public class ConfigExpressionBuilder
        {
            internal ConfigExpressionBuilder(Expression expression)
            {
                _expression = expression ?? throw new ArgumentNullException(nameof(expression));
                foreach (var key in expression.Parameters.Keys)
                {
                    _params.Add(key, null);
                }
            }

            private readonly Expression _expression;
            private readonly Dictionary<string, double?> _params = new Dictionary<string, double?>();

            public ConfigExpressionBuilder SetParam(string paramName, double value)
            {
                _params[paramName] = value;
                return this;
            }

            public ConfigExpressionBuilder WithConfig(ConfigNode configNode)
            {
                //azért csinálom ilyen furán, hogy módosítani lehessen a dictionary-t
                foreach (var paramName in _expression.Parameters.Keys)
                {
                    configNode.TryQuery(paramName).IfPresent(queryResult => { _params[paramName] = queryResult.AsDouble(); });
                }

                return this;
            }

            public double Evaluate()
            {
                foreach (var param in _params)
                {
                    if (param.Value == null)
                        throw new Exception($"parameter {param.Key} not set!");
                    _expression.Parameters[param.Key].Value = param.Value.Value;
                }

                return _expression.Value;
            }
        }
    }
}