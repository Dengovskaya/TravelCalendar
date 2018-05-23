using Utils;
using Microsoft.Office.Server;
using Microsoft.Office.Server.UserProfiles;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;
using TravelCalendar.SvcResource;

namespace TravelCalendar
{
    public class MultiModeFormFieldIterator : ListFieldIterator
    {
        static Dictionary<string, SvcCustomFields.CustomFieldDataSet.CustomFieldsRow> customFieldsDict;

        protected override void OnPreRender(EventArgs e)
        {
            
            foreach (BaseFieldControl fieldControl in SPContext.Current.FormContext.FieldControlCollection)
            {
                if (fieldControl.Field != null && SPContext.Current.FormContext.FormMode == SPControlMode.New)
                {
                    if ((fieldControl.Field.InternalName == "User1") || (fieldControl.Field.InternalName == "Title"))
                    {
                        fieldControl.Value = String.Format("{0};#{1}", SPContext.Current.Web.CurrentUser.ID, SPContext.Current.Web.CurrentUser.Name);
                    }
                    fieldControl.Validate();

                    if (fieldControl.IsValid)
                    {
                        fieldControl.UpdateFieldValueInItem();
                    }
                }
            }

            base.OnPreRender(e);
        }

        public class Configuration
        {
            public static string ProjectServerUri
            {
                get { return ConfigurationManager.AppSettings["ProjectServerUri"]; }
            }
        }

        private static void LoadCustomFieldsFromService(SvcCustomFields.CustomFields customFieldService)
        {
            if (customFieldsDict == null)
            {
                customFieldsDict = new Dictionary<string, SvcCustomFields.CustomFieldDataSet.CustomFieldsRow>();

                Guid fieldUid = new Guid("000039b7-8bbe-4ceb-82c4-fa8c0c400284");

                SvcCustomFields.CustomFieldDataSet cfDs = customFieldService.ReadCustomFields(null, false);
                foreach (SvcCustomFields.CustomFieldDataSet.CustomFieldsRow cfRow in cfDs.CustomFields)
                {
                    customFieldsDict[cfRow.MD_PROP_NAME] = cfRow;
                }
            }
        }
    }
}
