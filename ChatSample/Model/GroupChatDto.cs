using System.Collections.Generic;

namespace ChatSample.Model
{
    public class GroupChatDto
    {
        public GroupChatDto()
        {
            this.MemberList = new List<int>();
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public List<int> MemberList { get; set; }
    }
}
