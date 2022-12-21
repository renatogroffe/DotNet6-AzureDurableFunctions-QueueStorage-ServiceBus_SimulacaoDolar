using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using Azure.Storage.Queues;
using FunctionAppProcessarMoedas.Models;

namespace FunctionAppProcessarMoedas;

public class FunctionNotificarAzureQueueStorage
{
    [FunctionName("NotificarAzureQueueStorage")]
    public async static Task<bool> NotificarAzureQueueStorage(
        [ActivityTrigger] DadosCotacao dadosCotacao, ILogger log)
    {
        log.LogInformation(
            $"{nameof(NotificarAzureQueueStorage)} - Iniciando a execucao...");
        var jsonCotacao = JsonSerializer.Serialize(dadosCotacao);
        
        try
        {
            var client = new QueueClient(
                Environment.GetEnvironmentVariable("AzureStorageNotifications"),
                Environment.GetEnvironmentVariable("QueueAzureStorageNotifications"));
            await client.SendMessageAsync(jsonCotacao);        
        }
        catch (Exception ex)
        {
            log.LogError($"{nameof(NotificarAzureQueueStorage)} - Erro: " +
                $"{ex.GetType().Name} - {ex.Message}");
            return false;
        }

        log.LogInformation(
            $"{nameof(NotificarAzureQueueStorage)} - Dados procesados: " + jsonCotacao);        
        return true;
    }
}