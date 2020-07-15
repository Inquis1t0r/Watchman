using System;
using System.Collections.Generic;
using System.Text;
using Watchman.Common.Models;

namespace Watchman.DomainModel.Commons.Calculators.Statistics.Splitters
{
    public class QuarterSplitter : ISplitter
    {
        public IEnumerable<KeyValuePair<TimeRange, IEnumerable<T>>> Split<T>(IEnumerable<T> collection) where T : ISplittable
        {
            throw new NotImplementedException();
        }
    }
}
