using CQRSWITHES.Infra.EventStore;
using CQRSWITHES.Infra.EventStore;
using CQRSWITHESSQL.Infra.Sql;

namespace CQRSWITHES.Infra
{
    public class OnStart
    {
         public static void Start()
         {
             ReadSaved.SavedEventReader();
             TableReadmodelInterface.CheckForTables();
         }                
    }
}
