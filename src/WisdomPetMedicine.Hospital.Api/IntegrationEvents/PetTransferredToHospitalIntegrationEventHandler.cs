using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;
using WisdomPetMedicine.Hospital.Domain.Entities;
using WisdomPetMedicine.Hospital.Domain.Repositories;
using WisdomPetMedicine.Hospital.Domain.ValueObjects;

namespace WisdomPetMedicine.Hospital.Api.IntegrationEvents
{
    public class PetTransferredToHospitalIntegrationEventHandler : BackgroundService
    {
        private readonly IPatientAggregateStore patientAggregateStore;

        public PetTransferredToHospitalIntegrationEventHandler(IPatientAggregateStore patientAggregateStore)
        {
            this.patientAggregateStore = patientAggregateStore;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // cuando se procesa un evento
            var patientid = PatientId.Create(Guid.NewGuid());
            var patient = new Patient(patientid);
            await patientAggregateStore.SaveAsync(patient);
        }
    }
}
