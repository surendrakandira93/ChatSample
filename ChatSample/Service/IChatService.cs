using ChatSample.Model;
using System;
using System.Collections.Generic;

namespace ChatSample.Service
{
    public interface IChatService
    {
        int AddUser(string name);

        void RemoveUser(int userId);

        int AddGroup(string groupName);

        List<int> GetGroupMember(int groupId);

        KeyValuePair<int, string> GetUserById(int userId);

        List<KeyValuePair<int, string>> GetAllUser();

        List<GroupChatDto> GetAllGroup();

        GroupChatDto GetGroupById(int groupId);

        void AddGroupMember(int groupId, int userId);

        void RemoveGroupMember(int groupId, int userId);

        Dictionary<string, List<string>> KeepUserConnection(string userId, string connectionId);

        Dictionary<string, List<string>> RemoveUserConnection(string connectionId);

        List<string> GetUserConnections(string userId);

        List<string> GetConnectedUser(string[] userIds);
        List<string> GetConnectedUserValue(string[] userIds);

        Tuple<List<string>, List<string>> GetHubUsers(string[] userIds);
        List<string> GetHubConnectedUsers();

        List<string> GetUserAllConnections(string[] userIds);
    }
}
