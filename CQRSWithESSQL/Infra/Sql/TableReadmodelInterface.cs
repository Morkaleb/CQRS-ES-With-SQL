using System;
using System.Linq;
using System.Reflection;
using System.Data.SqlClient;

namespace CQRSWITHESSQL.Infra.Sql
{
    public class TableReadmodelInterface
    {
        static SqlConnection myConnection = new SqlConnection("Data Source=DESKTOP-OD2D63H;Initial Catalog=WSMgrSchedules;Integrated Security=True");

        public static void CheckForTables()
        {
            string nspace = "CQRSWITHES.src.ReadModels";
            var q = from t in Assembly.GetExecutingAssembly().GetTypes()
                    where t.IsClass && t.Namespace == nspace
                    select t;
            
          
            SqlCommand command = myConnection.CreateCommand();
            foreach (var readModel in q) {                
                string[] key = readModel.Name.ToString().Split("Read");
                if (key.Length == 1)
                {
                    myConnection.Open();
                    string methodName = readModel.Name.ToString();
                    string nameSpace = readModel.Namespace.ToString();
                    string fullClassName = nameSpace + "." + methodName;
                    object classToInvoke = Activator.CreateInstance(Type.GetType(fullClassName));
                    string dboParams = "";
                    foreach (PropertyInfo propertyInfo in classToInvoke.GetType().GetProperties()) {
                        if (propertyInfo.Name.ToString() != "Id")
                        {
                            dboParams = dboParams + propertyInfo.Name.ToString() + " varchar(50), ";
                        }
                    }
                    string commandtext = "IF NOT EXISTS(SELECT * FROM sysobjects WHERE id = object_id(N'[dbo].[" + key[0] + "]'))"
                        + Environment.NewLine +
                        " CREATE TABLE " + key[0] + "(" +
                        " Id varchar(50) PRIMARY KEY, " +
                        dboParams +
                        ");";
                    command.CommandText = commandtext;
                    var test = command.ExecuteReader();
                    myConnection.Close();
                }
            }
        }

        public static void UpdateTable(dynamic readModelData, string table)
        {
            string commandText = "";
            myConnection.Open();
        }
            
    }
}
