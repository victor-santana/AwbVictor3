//using AwbServiceBus.Reader;
//using AwbServiceBus.Writer;
//using Azure.Messaging.ServiceBus;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("Escreve no ServiceBus");
        await Task.Run(AwbServiceBusPublish.AwbServiceBusPublish.AwbServiceBusPublishAsync);
        Console.WriteLine("Le no ServiceBus");
        await Task.Run(AwbServiceBusSubscribe.AwbServiceBusSunscribe.AwbServiceBusSubscribeAsync);
        
    }
}