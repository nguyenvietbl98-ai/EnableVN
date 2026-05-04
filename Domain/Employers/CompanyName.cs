using Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Employers
{
    public sealed class CompanyName
    {
        public string Value { get; }

        private CompanyName(string value)
        {
            Value = value;
        }

        public static CompanyName Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new DomainException("Tên doanh nghiệp không được để trống.");

            value = value.Trim();

            if (value.Length < 2)
                throw new DomainException("Tên doanh nghiệp quá ngắn.");

            if (value.Length > 200)
                throw new DomainException("Tên doanh nghiệp không được vượt quá 200 ký tự.");

            return new CompanyName(value);
        }

        public override string ToString() => Value;
    }
}
