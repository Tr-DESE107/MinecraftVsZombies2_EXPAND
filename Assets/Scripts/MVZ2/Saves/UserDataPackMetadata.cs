using System;

namespace MVZ2.Saves
{
    public class UserDataPackMetadata
    {

        public UserDataPackMetadata(string username)
        {
            this.username = username;
        }
        public SerializableUserDataPackMetadata ToSerializable()
        {
            return new SerializableUserDataPackMetadata(this);
        }
        public static UserDataPackMetadata FromSerializable(SerializableUserDataPackMetadata seri)
        {
            if (seri == null)
                return null;
            return new UserDataPackMetadata(seri.username);
        }
        public string username;
    }
    [Serializable]
    public class SerializableUserDataPackMetadata
    {

        public SerializableUserDataPackMetadata(UserDataPackMetadata metadata)
        {
            this.username = metadata.username;
        }
        public string username;
    }
}
