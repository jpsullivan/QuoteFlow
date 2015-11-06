using System;
using QuoteFlow.Api.Asset.Comparator;
using QuoteFlow.Api.Models;

namespace QuoteFlow.Core.Asset.Search.Searchers.Transformer
{
    /// <summary>
    /// A value entered into a user searcher (e.g. current user, a specific user).
    /// </summary>
    public class UserSearchInput : IComparable<UserSearchInput>
    {
        public enum InputType
        {
            CURRENT_USER,
            EMPTY,
            GROUP,
            USER
        }

        private object @object;
        private readonly InputType type;
        private readonly string value;

        private UserSearchInput(InputType type, string value)
        {
            this.type = type;
            this.value = value;
        }

        /// <returns> An instance representing the "currentUser()" value. </returns>
        public static UserSearchInput currentUser()
        {
            return new UserSearchInput(InputType.CURRENT_USER, null);
        }

        /// <returns> An instance representing the "empty" value (e.g. unassigned). </returns>
        public static UserSearchInput empty()
        {
            return new UserSearchInput(InputType.EMPTY, null);
        }

        /// <param name="name"> The name of the group. </param>
        /// <returns> An instance representing a particular group. </returns>
        public static UserSearchInput group(string name)
        {
            return new UserSearchInput(InputType.GROUP, name);
        }

        /// <param name="name"> The user's username. </param>
        /// <returns> An instance representing a particular user. </returns>
        public static UserSearchInput user(string name)
        {
            return new UserSearchInput(InputType.USER, name);
        }

        public virtual int CompareTo(UserSearchInput other)
        {
            // Groups and users should be shown together.
            bool bothGroupsOrUsers = (type == InputType.GROUP || type == InputType.USER) && (other.type == InputType.GROUP || other.type == InputType.USER);

            if (!bothGroupsOrUsers)
            {
                return type.CompareTo(other.type);
            }

            return (new NullComparator()).Compare(CompareValue, other.CompareValue);
        }

        /// <returns> The "comparison" value to be used in {@code compareTo}. </returns>
        private string CompareValue
        {
            get
            {
                if (@object != null)
                {
//                    if (Group)
//                    {
//                        return ((Group)@object).Name.ToLower();
//                    }
                    if (User)
                    {
                        return ((User) @object).Username.ToLower();
                    }
                }

                return null;
            }
        }

        /// <returns> The actual object that corresponds to the user's input (i.e. the
        ///     group/user object with the given name/username). </returns>
        public virtual object Object
        {
            get { return @object; }
            set { this.@object = value; }
        }

        /// <returns> The type of the input value (i.e. current user, empty, etc.). </returns>
        public virtual InputType Type
        {
            get { return type; }
        }

        /// <returns> The input value (i.e. the name of the group/user). </returns>
        public virtual string Value
        {
            get { return value; }
        }

        /// <returns> Whether the instance represents the "currentUser()" value. </returns>
        public virtual bool CurrentUser
        {
            get { return type == InputType.CURRENT_USER; }
        }

        /// <returns> Whether the instance represents the "empty" value. </returns>
        public virtual bool Empty
        {
            get { return type == InputType.EMPTY; }
        }

        /// <returns> Whether the instance represents a particular group. </returns>
        public virtual bool Group
        {
            get { return type == InputType.GROUP; }
        }

        /// <returns> Whether the instance represents a particular user. </returns>
        public virtual bool User
        {
            get { return type == InputType.USER; }
        }

    }
}