using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data;
using FlopManager.Domain;


namespace Fbm.FamilyAppService
{
    public static class FamilyMemberAdapter
    {
        public static IList<FamilyMember> Convert(DataTable table)
        {
            if (table == null) throw new ArgumentNullException("table");
            if (table.Rows.Count == 0) throw new InvalidOperationException("No data to convert");
            List<FamilyMember> list = new List<FamilyMember>();
            foreach (DataRow row    in table.Rows)
            {
                var shareHolder = Convert(row);
                list.Add(shareHolder);
            }
            return list;
        }

        private static FamilyMember Convert(DataRow row)
        {

            FamilyMember s = new FamilyMember();
               s. Code = row.Field<int>("Code");
               s. Parent = row.Field<int>("Parent");
               s. FirstName = row.Field<string>("Name");
               s. FullName = row.Field<string>("FullName");
               s. MotherCode = row.Field<int>("MotherCode");
               s. Independent = row.Field<byte>("Independent");
               s. IndependentDate = row.Field<string>("IndependentDate");
               s. Alive = row.Field<byte>("Alive");
               s. HasChildren = row.Field<byte>("HasChildren");
               s. Shares = row.Field<int>("Shares");
               s. XShares = row.Field<int?>("XShares") ?? 0;
               s. Buffer = row.Field<int?>("Buffer") ?? 0;
               s.ShareHolderLevel = row.Field<byte>("Level");
               s.Sex = ConvertSex(row.Field<byte>("Sex"));
               return s;
           
        }

        private static Sex ConvertSex(byte p)
        {
            if (p == 1)
            {
                return Sex.Male;
            }
            return Sex.Female;
        }
    }
}
