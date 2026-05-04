using System;
using System.Collections.Generic;
using System.Text;
using Domain.Common;
using System.Text.RegularExpressions;

namespace Domain.Users
{
    public sealed class Email
    {
        public string Value { get; }

        private Email(string value)
        {
            Value = value;
        }

        public static Email Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new DomainException("Email không được để trống.");

            value = value.Trim().ToLower();

            var isValid = Regex.IsMatch(
                value,
                @"^[^@\s]+@[^@\s]+\.[^@\s]+$"
            );

            if (!isValid)
                throw new DomainException("Email không hợp lệ.");

            return new Email(value);
        }

        public override string ToString() => Value;
    }
}
