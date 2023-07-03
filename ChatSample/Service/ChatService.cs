using ChatSample.Model;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace ChatSample.Service
{
    public class ChatService: IChatService
    {
        public static Dictionary<int, string> userList = new Dictionary<int, string>();
        public static Dictionary<int, GroupChatDto> groupList = new Dictionary<int, GroupChatDto>();
        private static Dictionary<string, List<string>> userConnectionMap = new Dictionary<string, List<string>>();
        private static string userConnectionMapLocker = string.Empty;
        public ChatService()
        {
            groupList.Add(1, new GroupChatDto() { Id = 1, Name = "Group 1" });
            groupList.Add(2, new GroupChatDto() { Id = 2, Name = "Group 2" });
            groupList.Add(3, new GroupChatDto() { Id = 3, Name = "Group 3" });
        }
        public int AddUser(string name)
        {
            int userId = userList.Any()? userList.Max(a => a.Key) + 1:1;
            if (userList.Any())
            {
                if (!userList.Any(a => a.Value == name))
                {
                    userList.Add(userId, name);
                }
                else
                {
                    userId = userList.FirstOrDefault(s => s.Value == name).Key;
                }
            }
            else
            {
                userList.Add(userId, name);
            }
            
            return userId;
        }

        public void RemoveUser(int userId)
        {
            userList.Remove(userId);
        }

        public int AddGroup(string groupName)
        {
            int groupId = groupList.Any() ? groupList.Max(a => a.Key) + 1 : 1;
            if (groupList.Any())
            {
                if (!groupList.Any(a => a.Value.Name == groupName))
                {
                    groupList.Add(groupId, new GroupChatDto() { Id= groupId, Name=groupName});
                }
                else
                {
                    groupId = groupList.FirstOrDefault(s => s.Value.Name == groupName).Key;
                }
            }
            else
            {
                groupList.Add(groupId, new GroupChatDto() { Id = groupId, Name = groupName });
            }

            return groupId;
        }

        public void AddGroupMember(int groupId,int userId)
        {
            GroupChatDto group = groupList.FirstOrDefault(s => s.Key == groupId).Value;
            group.MemberList.Add(userId);
            groupList[groupId] = group;
        }

        public void RemoveGroupMember(int groupId, int userId)
        {
            GroupChatDto group = groupList.FirstOrDefault(s => s.Key == groupId).Value;
            group.MemberList.Remove(userId);
            groupList[groupId] = group;
        }

        public void RemoveGroup(int groupId)
        {
            groupList.Remove(groupId);
        }

        public List<int> GetGroupMember(int groupId)
        {
            List<int> user = groupList.FirstOrDefault(s => s.Key == groupId).Value.MemberList;
            return user;
        }

        public KeyValuePair<int, string> GetUserById(int userId)
        {
            KeyValuePair<int, string> user = userList.FirstOrDefault(s => s.Key == userId);
            return user;
        }

        public List<KeyValuePair<int, string>> GetAllUser()
        {
            List<KeyValuePair<int, string>> user = userList.Select(s => new KeyValuePair<int, string>(s.Key,s.Value)).ToList();
            return user;
        }

        public GroupChatDto GetGroupById(int groupId)
        {
            GroupChatDto user = groupList.FirstOrDefault(s=>s.Key==groupId).Value;
            return user;
        }
        public List<GroupChatDto> GetAllGroup()
        {
            List<GroupChatDto> user = groupList.Select(s => s.Value).ToList();
            return user;
        }

        public Dictionary<string, List<string>> KeepUserConnection(string userId, string connectionId)
        {
            lock (userConnectionMapLocker)
            {
                if (!userConnectionMap.ContainsKey(userId))
                {
                    userConnectionMap[userId] = new List<string>();
                }
                userConnectionMap[userId].Add(connectionId);

            }
            return userConnectionMap;
        }

        public Dictionary<string, List<string>> RemoveUserConnection(string connectionId)
        {
            lock (userConnectionMapLocker)
            {
                foreach (var userId in userConnectionMap.Keys)
                {
                    if (userConnectionMap.ContainsKey(userId))
                    {
                        if (userConnectionMap[userId].Contains(connectionId))
                        {
                            userConnectionMap.Remove(userId);

                            if (!userConnectionMap.ContainsKey(userId))
                            {
                                RemoveUser(int.Parse(userId));
                            }
                        }
                    }
                }
            }

            return userConnectionMap;

        }

        public List<string> GetUserConnections(string userId)
        {
            var conn = new List<string>();
            lock (userConnectionMapLocker)
                if (userConnectionMap.ContainsKey(userId))
                {
                    {
                        conn = userConnectionMap[userId];

                    }
                }
            return conn;
        }

        public List<string> GetConnectedUser(string[] userIds)
        {
            var conndUser = new List<string>();
            lock (userConnectionMapLocker)
            {
                conndUser = userConnectionMap.Where(x => userIds.Contains(x.Key)).Select(x => x.Key).ToList();
            }
            return conndUser;
        }

        public List<string> GetConnectedUserValue(string[] userIds)
        {
            var conn = new List<string>();
            lock (userConnectionMapLocker)
            {
                foreach (var userId in userIds)
                {
                    if (userConnectionMap.ContainsKey(userId))
                    {
                        var connectionId = userConnectionMap[userId][0];
                        conn.Add(connectionId);
                    }
                }
            }
            return conn;
        }

        public Tuple<List<string>, List<string>> GetHubUsers(string[] userIds)
        {
            List<string> conndUser = new List<string>();
            List<string> notConndUser = new List<string>();
            foreach (var id in userIds)
            {
                if (!userConnectionMap.ContainsKey(id))
                {
                    notConndUser.Add(id);
                }
                else
                {
                    conndUser.Add(id);
                }
            }

            return Tuple.Create(conndUser, notConndUser);
        }

        public List<string> GetHubConnectedUsers()
        {
            List<string> conndUser = userConnectionMap.Select(x => x.Key).ToList();
            return conndUser;
        }
        public List<string> GetUserAllConnections(string[] userIds)
        {
            var conn = new List<string>();
            lock (userConnectionMapLocker)
            {
                conn = userConnectionMap.Where(x => userIds.Contains(x.Key)).SelectMany(x => x.Value).ToList();
            }
            return conn;
        }

    }
}
