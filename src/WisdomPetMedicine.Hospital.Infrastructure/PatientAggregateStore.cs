using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using WisdomPetMedicine.Common;
using WisdomPetMedicine.Hospital.Domain.Entities;
using WisdomPetMedicine.Hospital.Domain.Repositories;
using WisdomPetMedicine.Hospital.Domain.ValueObjects;

namespace WisdomPetMedicine.Hospital.Infrastructure
{
    public class PatientAggregateStore : IPatientAggregateStore
    {
        private readonly CosmosClient cosmosClient;
        private readonly Container container;
        public PatientAggregateStore(IConfiguration config)
        {
            var Connection = config["CosmosDb:ConnectionString"];
            var database = config["CosmosDb:DatabaseId"];
            var containerId = config["CosmosDb:ContainerId"];

            var clientOptions = new CosmosClientOptions()
            {
                SerializerOptions = new CosmosSerializationOptions()
                {
                    PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase,
                }
            };
            cosmosClient = new CosmosClient(Connection, clientOptions);
            container = cosmosClient.GetContainer(database, containerId);
        }
        public async Task<Patient> LoadAsync(PatientId patientId)
        {
            // carga los eventos pasados
            if(patientId == null)
            {
                throw new ArgumentNullException(nameof(patientId));
            }
            // var aggregateid = $"Patient-{patientId.Value}";
            var aggregateid = "Patient-35e56a82-4e9c-421e-a3f5-e4f541efd332";
            var sql = $"select * from c where c.aggregateId = '{aggregateid}'";
            var queryDefinition = new QueryDefinition(sql);
            var queryResult=container.GetItemQueryIterator<CosmosEventData>(queryDefinition);
            var allEvents= new List<CosmosEventData>();
            while(queryResult.HasMoreResults)
            {
                var current=await queryResult.ReadNextAsync();
                foreach(var item in current)
                {
                    allEvents.Add(item);
                }
            }

            var domainEvents=allEvents.Select(e =>
            {
                var asembly = JsonConvert.DeserializeObject<string>(e.AssemblyQualifiedName);
                var eventType= Type.GetType(asembly);
                var data = JsonConvert.DeserializeObject(e.Data, eventType);
                return data as IDomainEvent;
            });

            var aggregate = new Patient();
            aggregate.Load(domainEvents);
            return aggregate;

        }

        public async Task SaveAsync(Patient patient)
        {
            //guarda el nueco evento
            if(patient == null)
            {
                throw new ArgumentNullException(nameof(patient));
            }
            var changes = patient.GetChanges()// serie de events de dominio que han ocurrido e el objeto, cuando lo recibe del service bus 
                .Select(o => new CosmosEventData(Guid.NewGuid(),
                $"Patient-{patient.Id}",
                o.GetType().Name,
                JsonConvert.SerializeObject(o),
                JsonConvert.SerializeObject(o.GetType().AssemblyQualifiedName)))
                .AsEnumerable();
            if(!changes.Any())
            {
                return;
            }

            foreach(var item in changes)
            {
                await container.CreateItemAsync(item);
            }
            patient.ClearChanges();
        }
    }
}