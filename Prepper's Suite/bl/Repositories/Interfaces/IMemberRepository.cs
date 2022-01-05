using bl.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bl.Repositories
{
    public interface IMemberRepository
    {
        Boolean DoesAccountHolderExist_byAccountId(int AccountId);

        Member GetRecord_byAccountId(int AccountId);
        Member GetRecord_byMemberId(int Id);
        IEnumerable<Member> GetList_byAccountId(int AccountId);

        void AddRecord(Member Record);
        void UpdateRecord(Member Record);
        void DeleteRecord(int Id);

        void Save();
    }
}


//void AddNewMember(Member Member);
//void UpdateMember(Member Member);
//IEnumerable<Member> GetMembers();
//void DeleteMember(int id);
//Boolean DoesMemberExist_ByAccountId(int AccountId);