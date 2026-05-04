using Domain.Users;
using Ports.Outbound.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Common
{
    /// <summary>
    /// Helper kiểm tra đăng nhập và phân quyền.
    /// 
    /// Đặt ở Application vì việc kiểm tra current user thuộc luồng use case,
    /// không thuộc Domain thuần.
    /// </summary>
    public static class AuthorizationGuard
    {
        /// <summary>
        /// Đảm bảo request hiện tại đã đăng nhập.
        /// </summary>
        public static Guid RequireAuthenticatedUser(ICurrentUserService currentUser)
        {
            if (!currentUser.IsAuthenticated || currentUser.UserId is null)
                throw new UseCaseException("Bạn cần đăng nhập để thực hiện thao tác này.");

            return currentUser.UserId.Value;
        }

        /// <summary>
        /// Đảm bảo user hiện tại có đúng role yêu cầu.
        /// </summary>
        public static Guid RequireRole(
            ICurrentUserService currentUser,
            UserRole requiredRole
        )
        {
            var userId = RequireAuthenticatedUser(currentUser);

            if (currentUser.Role != requiredRole)
                throw new UseCaseException($"Bạn cần quyền {requiredRole} để thực hiện thao tác này.");

            return userId;
        }

        /// <summary>
        /// Đảm bảo user hiện tại là Admin.
        /// </summary>
        public static Guid RequireAdmin(ICurrentUserService currentUser)
        {
            return RequireRole(currentUser, UserRole.Admin);
        }

        /// <summary>
        /// Đảm bảo user hiện tại là Employer.
        /// </summary>
        public static Guid RequireEmployer(ICurrentUserService currentUser)
        {
            return RequireRole(currentUser, UserRole.Employer);
        }

        /// <summary>
        /// Đảm bảo user hiện tại là Candidate.
        /// </summary>
        public static Guid RequireCandidate(ICurrentUserService currentUser)
        {
            return RequireRole(currentUser, UserRole.Candidate);
        }
    }
}
