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

        private object _object;
        private readonly InputType _type;

        private UserSearchInput(InputType type, string value)
        {
            _type = type;
            Value = value;
        }

        /// <summary>
        /// An instance representing the "currentUser()" value.
        /// </summary>
        /// <returns></returns>
        public static UserSearchInput CurrentUser()
        {
            return new UserSearchInput(InputType.CURRENT_USER, null);
        }

        /// <summary>
        /// An instance representing the "empty" value (e.g. unassigned).
        /// </summary>
        /// <returns></returns>
        public static UserSearchInput Empty()
        {
            return new UserSearchInput(InputType.EMPTY, null);
        }


        /// <summary>
        /// An instance representing a particular group.
        /// </summary>
        /// <param name="name">The name of the group</param>
        /// <returns></returns>
        public static UserSearchInput Group(string name)
        {
            return new UserSearchInput(InputType.GROUP, name);
        }

        /// <summary>
        /// An instance representing a particular user.
        /// </summary>
        /// <param name="name">The user's username.</param>
        /// <returns></returns>
        public static UserSearchInput User(string name)
        {
            return new UserSearchInput(InputType.USER, name);
        }

        public virtual int CompareTo(UserSearchInput other)
        {
            // Groups and users should be shown together.
            bool bothGroupsOrUsers = (_type == InputType.GROUP || _type == InputType.USER) &&
                                     (other._type == InputType.GROUP || other._type == InputType.USER);
            if (!bothGroupsOrUsers)
            {
                return _type.CompareTo(other._type);
            }

            return new NullComparator().Compare(CompareValue, other.CompareValue);
        }

        /// <summary>
        /// The "comparison" value to be used in CompareTo().
        /// </summary>
        private string CompareValue
        {
            get
            {
                if (_object != null)
                {
//                    if (IsGroup)
//                    {
//                        return ((IsGroup)@_object).Name.ToLower();
//                    }
                    if (IsUser)
                    {
                        return ((User) _object).Username.ToLower();
                    }
                }

                return null;
            }
        }

        /// <summary>
        /// The actual _object that corresponds to the user's input (i.e. the user 
        /// _object with the given name/username).
        /// </summary>
        public virtual object Object
        {
            get { return _object; }
            set { _object = value; }
        }

        /// <summary>
        /// The _type of the input value (i.e. current user, empty, etc.).
        /// </summary>
        public virtual InputType Type => _type;

        /// <summary>
        /// The input value (i.e. the name of the group/user).
        /// </summary>
        public virtual string Value { get; }

        /// <summary>
        /// Whether the instance represents the "currentUser()" value.
        /// </summary>
        public virtual bool IsCurrentUser => _type == InputType.CURRENT_USER;

        /// <summary>
        /// Whether the instance represents the "empty" value.
        /// </summary>
        public virtual bool IsEmpty => _type == InputType.EMPTY;

        /// <summary>
        /// Whether the instance represents a particular group.
        /// </summary>
        public virtual bool IsGroup => _type == InputType.GROUP;

        /// <summary>
        /// Whether the instance represents a particular user.
        /// </summary>
        public virtual bool IsUser => _type == InputType.USER;
    }
}