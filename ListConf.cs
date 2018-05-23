using Microsoft.SharePoint;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Utils;

namespace TravelCalendar
{
    public class ListConf
    {
        static string PropertyKeyPrefix = "ListConfig_";

        [DefaultValue("http://server/pwa")]
        public string UrlServer;

        public ListConf() { }

        public ListConf(SPList list)
        {
            SPFolder folder = list.RootFolder;

            IEnumerable<FieldInfo> fields = GetCfgFields();
            foreach (FieldInfo field in fields)
            {
                object value = folder.GetProperty(PropertyKeyPrefix + field.Name);
                if (value != null)
                    field.SetValue(this, Convert.ChangeType(value, field.FieldType));
            }
        }

        public void SaveToListProperty(SPList list)
        {
            SPFolder folder = list.RootFolder;

            IEnumerable<FieldInfo> fields = GetCfgFields();
            foreach (FieldInfo field in fields)
            {
                folder.SetProperty(PropertyKeyPrefix + field.Name, Convert.ToString(field.GetValue(this)));
            }

            bool allowUnsafeUpdates = list.ParentWeb.AllowUnsafeUpdates;
            list.ParentWeb.AllowUnsafeUpdates = true;
            folder.Update();
            list.ParentWeb.AllowUnsafeUpdates = allowUnsafeUpdates;
        }

        public IEnumerable<FieldInfo> GetCfgFields()
        {
            IEnumerable<FieldInfo> fields = this.GetType().GetFields().Where(x => !x.IsLiteral && !x.IsStatic);
            return fields;

        }

        public void SetDefaultValues()
        {
            IEnumerable<FieldInfo> fields = GetCfgFields();
            foreach (FieldInfo field in fields)
            {
                DefaultValueAttribute dvAttribute = field.GetCustomAttributes(typeof(DefaultValueAttribute), false).FirstOrDefault() as DefaultValueAttribute;
                if (dvAttribute != null)
                    field.SetValue(this, Convert.ChangeType(dvAttribute.Value, field.FieldType));
            }
        }

        public void ToSPLog()
        {
            IEnumerable<FieldInfo> fields = GetCfgFields();
            foreach (FieldInfo field in fields)
            {
                SPLog.Write("Конфигурация. {0} = <{1}>", field.Name, field.GetValue(this));
            }
        }
    }
}

