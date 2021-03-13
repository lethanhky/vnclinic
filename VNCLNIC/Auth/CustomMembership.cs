using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using VNCLNIC.Data;
using VNCLNIC.Models;

namespace VNCLNIC.Auth
{
    public class CustomMembership
    {
        private readonly DataContext context;

        public CustomMembership(DataContext context)
        {
            this.context = context;
        }

        public bool EnablePasswordRetrieval => throw new NotImplementedException();

        public bool EnablePasswordReset => throw new NotImplementedException();

        public bool RequiresQuestionAndAnswer => throw new NotImplementedException();

        public string ApplicationName { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public int MaxInvalidPasswordAttempts => throw new NotImplementedException();

        public int PasswordAttemptWindow => throw new NotImplementedException();

        public bool RequiresUniqueEmail => throw new NotImplementedException();

        public MembershipPasswordFormat PasswordFormat => throw new NotImplementedException();

        public int MinRequiredPasswordLength => throw new NotImplementedException();

        public int MinRequiredNonAlphanumericCharacters => throw new NotImplementedException();

        public string PasswordStrengthRegularExpression => throw new NotImplementedException();

        public bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            throw new NotImplementedException();
        }

        public bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer)
        {
            throw new NotImplementedException();
        }

        public MembershipUser CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey, out MembershipCreateStatus status)
        {
            throw new NotImplementedException();
        }

        public bool DeleteUser(string username, bool deleteAllRelatedData)
        {
            throw new NotImplementedException();
        }

        public MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public int GetNumberOfUsersOnline()
        {
            throw new NotImplementedException();
        }

        public string GetPassword(string username, string answer)
        {
            throw new NotImplementedException();
        }

        public User GetUser(object providerUserKey, bool userIsOnline)
        {
            throw new NotImplementedException();
        }

        public User GetUser(string username, bool userIsOnline)
        {
            var user = (from us in context.Users
                        where string.Compare(username, us.Username, StringComparison.OrdinalIgnoreCase) == 0
                        select us).FirstOrDefault();
            if (user == null)
            {
                return null;
            }

            // var selectedUser = new CustomMembershipUser(user);
            return user;
        }

        public string GetUserNameByEmail(string email)
        {
            string username = (from u in context.Users
                               where string.Compare(email, u.Email) == 0
                               select u).FirstOrDefault().Username ?? "";
            return !string.IsNullOrEmpty(username) ? username : String.Empty;
        }

        public string ResetPassword(string username)
        {
            var user = (from u in context.Users
                        where string.Compare(username, u.Username, StringComparison.OrdinalIgnoreCase) == 0
                        select u).FirstOrDefault();
            var passwordReset = Membership.GeneratePassword(8, 1);
            //user.Password = Utilities.Encrypt(passwordReset);
            context.Users.Attach(user);
            context.Entry(user).State = System.Data.Entity.EntityState.Modified;
            context.SaveChanges();
            return passwordReset;
        }

        public bool UnlockUser(string userName)
        {
            throw new NotImplementedException();
        }

        public void UpdateUser(MembershipUser user)
        {
            throw new NotImplementedException();
        }

        public bool ValidateUser(string username, string password, out bool isActive)
        {
            isActive = false;
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                return false;
            }

            var hashPassword = Utilities.Encrypt(password);

            var user = (from us in context.Users
                        where string.Compare(username, us.Username, StringComparison.OrdinalIgnoreCase) == 0
                        && string.Compare(hashPassword, us.Password, StringComparison.OrdinalIgnoreCase) == 0
                        select us).FirstOrDefault();
            if (user == null)
                return false;
            isActive = user.Roles.Any(r => r.Permissions.Any(p => p.IsActive == true));
            return true;
        }
    }
}