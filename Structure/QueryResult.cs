using System;
using System.Collections.Generic;
using SnowFlakeGamesAssets.PiscesConfigLoader.Utils;
using SnowFlakeGamesAssets.PiscesConfigLoader.Utils.Expression;
using UnityEngine;
using Random = System.Random;

namespace SnowFlakeGamesAssets.PiscesConfigLoader.Structure
{
    /// <summary>
    /// The result of querying a config node value
    /// Can return different complex or simple types as result
    /// </summary>
    public class QueryResult
    {
        /// <summary>
        /// The pathe where this result is in the config tree
        /// </summary>
        public ConfigPath Path { get; }

        private object _value;

        private static readonly ExpressionParser expressionParser = new ExpressionParser();
        private static readonly Random random = new Random(0);

        static QueryResult()
        {
            expressionParser.AddFunc("rand", inputs =>
            {
                if (inputs == null)
                    throw new ArgumentNullException(nameof(inputs));
                if (inputs.Length != 2)
                    throw new ArgumentException("Function random should have only two input parameter.", nameof(inputs));

                float min = (float) inputs[0];
                float max = (float) inputs[1];

                if (min > max)
                    throw new ArgumentException("The first parameter should be less then the second", nameof(inputs));

                return random.NextDouble() * (max - min) + min;
            });
        }

        public QueryResult(object value, ConfigPath path)
        {
            _value = value;
            Path = path;
            if (value == null)
                throw new Exception("Result has no value!");
        }

        /// <summary>
        /// Returns value parsed to integer
        /// </summary>
        public int AsInt() => int.Parse(AsString());

        /// <summary>
        /// Returns value parsed to long
        /// </summary>
        public long AsLong() => long.Parse(AsString());

        /// <summary>
        /// Returns value parsed to float
        /// </summary>
        public float AsFloat() => float.Parse(AsString());

        /// <summary>
        /// Returns value parsed to double
        /// </summary>
        public double AsDouble() => double.Parse(AsString());

        /// <summary>
        /// Returns value parsed to Vector3 from "x y z" string format
        /// </summary>
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

        /// <summary>
        /// Returns the raw string value
        /// </summary>
        public string AsString()
        {
            return _value as string ?? _value.ToString();
        }

        /// <summary>
        /// Returns the value parsed to ConfigPath using the "pathStep1.pathStep2.pathStep3..." string format
        /// </summary>
        /// <exception cref="ArgumentException">If the value is null or the path contains spaces</exception>
        public ConfigPath AsPath()
        {
            var pathStr = AsString();

            if (pathStr == null)
                throw new ArgumentException($"Path string should not be null");
            if (pathStr.Contains(" "))
                throw new ArgumentException($"Path string [{pathStr}] should not contain spaces");

            return new ConfigPath(pathStr.Split('.'));
        }

        /// <summary>
        /// Returns the value as a ConfigNode
        /// </summary>
        public ConfigNode AsNode() => new ConfigNode(_value as Dictionary<object, object>, Path);

        /// <summary>
        /// Returns the value as a QueryResult List
        /// </summary>
        public List<QueryResult> AsList()
        {
            if (_value is IList<object> objects)
            {
                List<QueryResult> result = new List<QueryResult>();
                for (var index = 0; index < objects.Count; index++)
                {
                    var x = objects[index];
                    result.Add(new QueryResult(x, Path.Add(index.ToString())));
                }

                return result;
            }

            throw new Exception("Query result is a leaf, not a node!");
        }

        /// <summary>
        /// Returns value parsed to ConfigExpressionBuilder
        /// </summary>
        public ConfigExpressionBuilder AsExpression()
        {
            var asString = AsString();

            var expression = expressionParser.EvaluateExpression(asString);

            return new ConfigExpressionBuilder(expression);
        }

        /// <summary>
        /// Returns value parsed to RangeI from string format "min_max" or from a single number
        /// </summary>
        /// <exception cref="Exception"> if unable to parse</exception>
        public RangeI AsRangeI()
        {
            var asString = AsString();

            var nums = asString.Split('_');
            if (nums.Length == 2)
            {
                return new RangeI(int.Parse(nums[0]), int.Parse(nums[1]));
            }

            if (nums.Length == 1)
            {
                return new RangeI(int.Parse(nums[0]), int.Parse(nums[0]));
            }

            throw new Exception($"Unable to parse RangeI: {asString}");
        }

        /// <summary>
        /// Returns value parsed to Unity color from html color code
        /// </summary>
        public Color AsColor()
        {
            var asString = AsString();
            return ConfigColorUtils.ParseColorFromHtml(asString);
        }
    }
}