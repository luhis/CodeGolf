using EnsureThat;

namespace CodeGolf.Domain
{
    public class User
    {
        public User(int userId, string loginName, string realName, string avatarUri)
        {
            this.UserId = EnsureArg.IsNotDefault(userId, nameof(userId));
            this.LoginName = EnsureArg.IsNotNullOrWhiteSpace(loginName, nameof(loginName));
            this.RealName = EnsureArg.IsNotNull(realName, nameof(realName));
            this.AvatarUri = EnsureArg.IsNotNullOrWhiteSpace(avatarUri, nameof(avatarUri));
        }

        public int UserId { get; private set; }

        public string LoginName { get; private set; }

        public string RealName { get; private set; }

        public string AvatarUri { get; private set; }
    }
}