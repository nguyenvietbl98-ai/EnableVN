using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Jobs
{
    public enum JobStatus
    {
        Draft = 1,
        Published = 2,
        Closed = 3,
        Deleted = 4
    }
}
