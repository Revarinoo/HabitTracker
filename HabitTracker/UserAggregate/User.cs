using System;
namespace HabitTracker
{
    public class User
    {
        private Guid _id;
        private string _name;
        

        public Guid ID
        {
            get
            {
                return _id;
            }
        }
        public string Name
        {
            get
            {
                return _name;
            }
        }

        

        // public User(Guid id, string name) : this(id, name) { }

        public User(Guid id, string name)
        {
            this._id = id;
            this._name = name;
        }

        public static User NewUser(string name)
        {
            return new User(Guid.NewGuid(), name);
        }

        public override bool Equals(object obj)
        {
            var user = obj as User;
            if (user == null) return false;

            return this._id == user._id;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + _id.GetHashCode();
                return hash;
            }
        }
    }
}