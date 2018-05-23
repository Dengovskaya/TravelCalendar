using Utils;
using System;
using System.Data;
using System.Net;
using TravelCalendar.SvcLookupTable;
using TravelCalendar.SvcResource;
using Microsoft.SharePoint;

namespace TravelCalendar
{
    public class PsiExporter
    {
        public static string GetSubdivision(SPList list, string email)
        {
            Resource ResourceSvc;
            LookupTable LookupTable;
            ListConf cfg = new ListConf(list);
            CreateServices(cfg, out ResourceSvc, out LookupTable);
            var usersData = ResourceSvc.ReadResources(string.Empty, false);

            Guid userUid = Guid.Empty;
            foreach (DataRow row in usersData.Resources.Rows)
            {
                if (Convert.ToString(row["WRES_EMAIL"]) == email)
                    userUid = new Guid(Convert.ToString(row["RES_UID"]));
            }

            if (userUid == Guid.Empty)
                return string.Empty;

            Guid customFieldGuid = new Guid("000039b7-8bbe-4ceb-82c4-fa8c0c400284");
            Guid valueUid = Guid.Empty;
            foreach (DataRow row in usersData.ResourceCustomFields.Rows)
            {
                if ((Guid)row["MD_PROP_UID"] == customFieldGuid &&
                    (Guid)row["RES_UID"] == userUid)
                {
                    valueUid = (Guid)row["CODE_VALUE"];
                    break;
                }
            }

            if (valueUid == Guid.Empty)
                return string.Empty;
            
            var tables = LookupTable.ReadLookupTablesByUids(new Guid[] { new Guid("00008e67-65a3-4898-baaa-d82d995bbb02") }, false, 1049);
            foreach (DataRow row in tables.LookupTableTrees.Rows)
            {
                if ((Guid)row["LT_STRUCT_UID"] == valueUid)
                {
                    return Convert.ToString(row["LT_VALUE_TEXT"]);
                }
            }

            return string.Empty;
        }

        private static void CreateServices(ListConf cfg, out Resource ResourceSvc, out LookupTable LookupTable)
        {
            string projectServerUri = cfg.UrlServer;
            const string lookupTablePath = "/_vti_bin/PSI/LookupTable.asmx";
            const string ServicePath = "/_vti_bin/PSI/Resource.asmx";

            ResourceSvc = new Resource();
            ResourceSvc.Url = projectServerUri + ServicePath;
            SPLog.Write("ProjectService.Url = {0}", ResourceSvc.Url);
            ResourceSvc.Credentials = new NetworkCredential("users", "password", "domen");

            LookupTable = new SvcLookupTable.LookupTable();
            LookupTable.Url = projectServerUri + lookupTablePath;
            LookupTable.Credentials = new NetworkCredential("users", "password", "domen");
            SPLog.Write("QueueService.Url = {0}", LookupTable.Url);
        }
    }
}
