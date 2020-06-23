using System;

namespace SnowFlakeGamesAssets.PiscesConfigLoader.Structure
{
    public class MaybeQueryResult
    {
        private readonly QueryResult _queryResult;

        public MaybeQueryResult(QueryResult queryResult)
        {
            _queryResult = queryResult;
        }

        public bool IsPresent => _queryResult.HasValue;

        public QueryResult Get() => _queryResult;

        public void IfPresent(Action<QueryResult> actionIfPresent)
        {
            if (IsPresent)
                actionIfPresent(_queryResult);
        }

        public void IfPresent(Action<QueryResult> actionIfPresent, Action actionIfNotPresent)
        {
            if (IsPresent)
                actionIfPresent(_queryResult);
            else
                actionIfNotPresent();
        }
    }
}