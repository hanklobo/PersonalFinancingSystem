using System;
using System.Collections.Generic;

namespace PersonalFinancialSystem.Services
{
    public interface IEntryService
    {
        EntryOutput Get(DateTime startDate);
        void Post(IList<Entry> postData);
    }
}