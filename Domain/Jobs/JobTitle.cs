using Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Jobs
{
    public sealed class JobTitle
    {
        public string Value { get; }

        private JobTitle(string value)
        {
            Value = value;
        }

        public static JobTitle Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new DomainException("Tiêu đề công việc không được để trống.");

            value = value.Trim();

            if (value.Length < 5)
                throw new DomainException("Tiêu đề công việc quá ngắn.");

            if (value.Length > 200)
                throw new DomainException("Tiêu đề công việc không được vượt quá 200 ký tự.");

            return new JobTitle(value);
        }

        public override string ToString() => Value;
    }
}
