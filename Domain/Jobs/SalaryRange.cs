using Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Jobs
{
    public sealed class SalaryRange
    {
        public decimal? MinSalary { get; private set; }
        public decimal? MaxSalary { get; private set; }

        private SalaryRange(decimal? minSalary, decimal? maxSalary)
        {
            MinSalary = minSalary;
            MaxSalary = maxSalary;
        }

        public static SalaryRange Create(decimal? minSalary, decimal? maxSalary)
        {
            if (minSalary.HasValue && minSalary < 0)
                throw new DomainException("Lương tối thiểu không được âm.");

            if (maxSalary.HasValue && maxSalary < 0)
                throw new DomainException("Lương tối đa không được âm.");

            if (minSalary.HasValue && maxSalary.HasValue && minSalary > maxSalary)
                throw new DomainException("Lương tối thiểu không được lớn hơn lương tối đa.");

            return new SalaryRange(minSalary, maxSalary);
        }
    }
}
