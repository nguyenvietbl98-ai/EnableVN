using InfrastructureSqlite.PersistenceModels;
using System;
using System.Collections.Generic;
using System.Text;
using Domain.Users;

namespace InfrastructureSqlite.Mappers
{
    public static class UserPersistenceMapper
    {
        public static UserRecord ToRecord(User user)
        {
            return new UserRecord
            {
                Id = user.Id,
                Email = user.Email.Value,
                PasswordHash = user.PasswordHash,
                Role = user.Role.ToString(),
                Status = user.Status.ToString()
            };
        }

        public static User ToDomain(UserRecord record)
        {
            var role = Enum.Parse<UserRole>(record.Role);
            var status = Enum.Parse<UserStatus>(record.Status);

            return User.Restore(
                record.Id,
                record.Email,
                record.PasswordHash,
                role,
                status
            );
        }

        public static void UpdateRecord(UserRecord record, User user)
        {
            record.Email = user.Email.Value;
            record.PasswordHash = user.PasswordHash;
            record.Role = user.Role.ToString();
            record.Status = user.Status.ToString();
        }
    }
}
