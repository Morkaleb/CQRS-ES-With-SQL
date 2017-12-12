using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Data.SqlClient;

namespace CQRSWITHES.Infra
{
    public static class EventDistributor
    {
        public static void Publish(EventModel anEvent)
        {
            dynamic readmodelData;
            var data = EventDictionary.Streams;
            string nspace = "CQRSWITHES.src.ReadModels";
            var q = from t in Assembly.GetExecutingAssembly().GetTypes()
                    where t.IsClass && t.Namespace == nspace
                    select t;
            foreach (var readModel in q)
            {
                try
                {
                    MethodInfo theMethod = typeof(ReadModels.ReadModel).GetMethod("EventPublish", new[] { typeof(EventModel) });
                    string methodName = readModel.Name.ToString();
                    string nameSpace = readModel.Namespace.ToString();
                    string key = methodName.Split("Read")[0];
                    if (Book.book.ContainsKey(key))
                    {
                        string fullClassName = nameSpace + "." + methodName;
                        object readModelToInvoke = Activator.CreateInstance(Type.GetType(fullClassName));
                        readmodelData = theMethod.Invoke(readModelToInvoke, new EventModel[] { anEvent });
                        PublishToSQL(readmodelData, key);
                    }
                    else
                    {
                        Book.book.Add(key, new List<ReadModelData>());
                        string fullClassName = nameSpace + "." + methodName;
                        object readModelToInvoke = Activator.CreateInstance(Type.GetType(fullClassName));
                        readmodelData = theMethod.Invoke(readModelToInvoke, new EventModel[] { anEvent });
                    }
                }
                catch (Exception e)
                {
                    Console.Write(e);
                }

            }

        }       

        private static void PublishToSQL(object readModelData, string key)
        {
            SqlConnection myConnection = new SqlConnection("Data Source=DESKTOP-OD2D63H;Initial Catalog=WSMgrSchedules;Integrated Security=True");
            try
            {
                myConnection.Open();
            }
            catch(Exception e)
            {
                Console.Write(e);
            }
            if(myConnection.State == System.Data.ConnectionState.Open)
            {
                string db = myConnection.Database;
                
            }
        }
    }
}
