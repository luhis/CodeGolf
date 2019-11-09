namespace CodeGolf.Domain
{
    using System;
    using EnsureThat;

    public class User
    {
        public User(int userId, string loginName, string realName, Uri avatarUri)
        {
            this.UserId = EnsureArg.IsNotDefault(userId, nameof(userId));
            this.LoginName = EnsureArg.IsNotNullOrWhiteSpace(loginName, nameof(loginName));
            this.RealName = EnsureArg.IsNotNull(realName, nameof(realName));
            this.AvatarUri = EnsureArg.IsNotNull(avatarUri, nameof(avatarUri));
        }

        public int UserId { get; private set; }

        public string LoginName { get; private set; }

        public string RealName { get; private set; }

        public Uri AvatarUri { get; private set; }
    }
}
