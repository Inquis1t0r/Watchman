using System;
using System.Collections.Generic;
using System.Linq;
using Watchman.Common.Models;

namespace Watchman.DomainModel.Commons.Calculators.Statistics.Splitters
{
    public class HourSplitter : ISplitter
    {
        public IEnumerable<KeyValuePair<TimeRange, IEnumerable<T>>> Split<T>(IEnumerable<T> collection)
            where T : ISplittable
        {
            var groupedByDay = collection.GroupBy(x => x.GetSplittable().Date.Day);
            foreach (var day in groupedByDay)
            {
                var groupedbyHours = collection.GroupBy(x => x.GetSplittable().Date.Hour);
                foreach (var hour in groupedbyHours)
                {
                    //var timeRange = TimeRange.Create(hour.Key, hour.Key);
                    var timeRange = TimeRange.Create(new DateTime(hour.Key), new DateTime(hour.Key + 24));
                    yield return new KeyValuePair<TimeRange, IEnumerable<T>>(timeRange, hour);
                }
            }
        }
    }
}
