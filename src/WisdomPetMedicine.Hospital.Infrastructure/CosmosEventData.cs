using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WisdomPetMedicine.Hospital.Infrastructure
{
    public record CosmosEventData(Guid id,string AggregateId,string EventName,string Data,string AssemblyQualifiedName);
    
}
