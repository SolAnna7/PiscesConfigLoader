using System;
using System.Collections.Generic;
using SnowFlakeGamesAssets.PiscesConfigLoader.Utils.Expression;

namespace SnowFlakeGamesAssets.PiscesConfigLoader.Structure
{
    /// <summary>
    /// A wrapper around a third party Expression evaluator
    /// </summary>
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

        /// <summary>
        /// Sets a named param of the expression
        /// </summary>
        public ConfigExpressionBuilder SetParam(string paramName, double value)
        {
            _params[paramName] = value;
            return this;
        }

        /// <summary>
        /// Enables the expression, to get its parameters from a config node
        /// </summary>
        public ConfigExpressionBuilder WithConfig(ConfigNode configNode)
        {
            // used this way so the Dictionary can be changed in iteration
            foreach (var paramName in _expression.Parameters.Keys)
            {
                configNode.TryQuery(paramName).IfPresent(queryResult => { _params[paramName] = queryResult.AsDouble(); });
            }

            return this;
        }

        /// <summary>
        /// Returns the evaluated value of the expression
        /// </summary>
        /// <exception cref="Exception">If some parameters are not set</exception>
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