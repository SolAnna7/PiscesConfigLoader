using System;

namespace SnowFlakeGamesAssets.PiscesConfigLoader.Structure
{
    /// <summary>
    /// A container object which may or may not contain a non-null QueryResult. If a value is present, isPresent() will return true and get() will return the QueryResult.
    /// </summary>
    public class MaybeQueryResult
    {
        private readonly QueryResult _queryResult;

        public MaybeQueryResult(object value, ConfigPath path)
        {
            if (value == null)
                IsPresent = false;
            else
            {
                IsPresent = true;
                _queryResult = new QueryResult(value, path);
            }
        }

        /// <summary>
        /// Returns true if the value is not null
        /// </summary>
        public bool IsPresent { get; }

        /// <summary>
        /// Return the QueryResult if the value is not null
        /// </summary>
        public QueryResult Get() => _queryResult;

        /// <summary>
        /// Runs action with QueryResult as input if value is not null
        /// </summary>
        public void IfPresent(Action<QueryResult> actionIfPresent)
        {
            if (IsPresent)
                actionIfPresent(_queryResult);
        }

        /// <summary>
        /// Runs function with QueryResult as input if value is not null
        /// Returns function result or default value
        /// </summary>
        public Tr IfPresentGet<Tr>(Func<QueryResult, Tr> actionIfPresent, Tr defaultValue) => IsPresent ? actionIfPresent(_queryResult) : defaultValue;

        /// <summary>
        /// Runs action with QueryResult as input if value is not null else runs else action
        /// </summary>
        public void IfPresent(Action<QueryResult> actionIfPresent, Action actionIfNotPresent)
        {
            if (IsPresent)
                actionIfPresent(_queryResult);
            else
                actionIfNotPresent();
        }
    }
}