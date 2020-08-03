using System;
using System.Collections.Generic;
namespace HabitTracker
{
    public class Badge
    {
        private Guid _id;
        private string _name;
        private string _description;
        private Guid _usersID;
        private DateTime _createdAt;

        public Guid ID
        {
            get
            {
                return _id;
            }
        }

        public string name
        {
            get
            {
                return _name;
            }
        }

        public string description
        {
            get
            {
                return _description;
            }
        }

        public Guid users
        {
            get
            {
                return _usersID;
            }
        }

        public DateTime created_at
        {
            get
            {
                return _createdAt;
            }
        }

        public Badge(Guid id ,string name, string description, Guid userID, DateTime created_at)
        {
            if (name == null) throw new Exception("Name cannot be null");
            if (name.Length < 2 || name.Length > 100) throw new Exception("Name must between 2 char and 100 char");
            if (description == null) throw new Exception("Description must be filled");
            this._id = id;
            this._name = name;
            this._description = description;
            this._usersID = userID;
            this._createdAt = created_at;
        }

        public static Badge NewBadge(Guid id, string name, string description, Guid userID, DateTime created_at)
        {
            return new Badge(id, name, description, userID,created_at);
        }


        public override bool Equals(object obj)
        {
            var badge = obj as Badge;
            if (badge == null) return false;

            return true;
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