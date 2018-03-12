using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data;
using FlopManager.Domain;


namespace Fbm.FamilyAppService
{
    public class Repository
    {
        readonly string _dbAddress;
        public Repository(string dbAddress)
        {
            this._dbAddress = dbAddress;
        }
        public IList<FamilyMember> GetShareHolders()
        { 
            Database db = new Database(_dbAddress);
            DataTable table = db.GetTable("Id");
            return FamilyMemberAdapter.Convert(table);
        }
    }
}
