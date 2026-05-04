using Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Candidates
{
    public sealed class FullName
    {
        public string Value { get; }

        private FullName(string value)
        {
            Value = value;
        }

        public static FullName Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new DomainException("Họ tên không được để trống.");

            value = value.Trim();

            if (value.Length < 2)
                throw new DomainException("Họ tên quá ngắn.");

            if (value.Length > 100)
                throw new DomainException("Họ tên không được vượt quá 100 ký tự.");

            return new FullName(value);
        }

        public override string ToString() => Value;
    }
}
