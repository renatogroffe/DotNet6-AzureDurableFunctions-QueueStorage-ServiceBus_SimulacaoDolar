using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using Azure.Messaging.ServiceBus;
using FunctionAppProcessarMoedas.Models;

namespace FunctionAppProcessarMoedas;

public class FunctionNotificarAzureServiceBus
{
    [FunctionName("NotificarAzureServiceBus")]
    public async static Task<bool> NotificarAzureServiceBus(
        [ActivityTrigger] DadosCotacao dadosCotacao, ILogger log)
    {
        log.LogInformation(
            $"{nameof(FunctionNotificarAzureServiceBus)} - Iniciando a execucao...");
        var jsonCotacao = JsonSerializer.Serialize(dadosCotacao);
        
        try
        {
            var clientOptions = new ServiceBusClientOptions() { TransportType = ServiceBusTransportType.AmqpWebSockets };
            var client = new ServiceBusClient(
                Environment.GetEnvironmentVariable("AzureServiceBusNotifications"),
                clientOptions);
            var sender = client.CreateSender(
                Environment.GetEnvironmentVariable("QueueAzureServiceBusNotifications"));
            
            using ServiceBusMessageBatch messageBatch = await sender.CreateMessageBatchAsync();
            messageBatch.TryAddMessage(new ServiceBusMessage(jsonCotacao));
            await sender.SendMessagesAsync(messageBatch);
        }
        catch (Exception ex)
        {
            log.LogError($"{nameof(NotificarAzureServiceBus)} - Erro: " +
                $"{ex.GetType().Name} - {ex.Message}");
            return false;
        }
        
        log.LogInformation(
            $"{nameof(NotificarAzureServiceBus)} - Dados procesados: " + jsonCotacao);        
        return true;
    }
}