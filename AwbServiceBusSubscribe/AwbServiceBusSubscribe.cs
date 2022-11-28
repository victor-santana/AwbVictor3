using Azure.Messaging.ServiceBus;

namespace AwbServiceBusSubscribe
{
    public class AwbServiceBusSunscribe
    {
        
        // o cliente que possui a conexão e pode ser usado para criar remetentes e destinatários
        static ServiceBusClient? client;

        // o processador que lê e processa mensagens da fila
        static ServiceBusProcessor? processor;

        // lidar com mensagens recebidas
        public static async Task MessageHandler(ProcessMessageEventArgs args)
        {
            
            var body = args.Message.Body.ToString();
            Console.WriteLine(body);

             AwbRepository.AwbRepository.InsertBancoDeDados(body);


            // completar a mensagem. mensagens é excluída da fila. 
            await args.CompleteMessageAsync(args.Message);
        }

        // lidar com quaisquer erros ao receber mensagens
        static Task ErrorHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine(args.Exception.ToString());
            return Task.CompletedTask;
        }

        public static async Task AwbServiceBusSubscribeAsync()
        {
            var connectionString = AwbServiceBusPublish.AwbServiceBusPublish.ConfigServiceBusString();
            var queueName = AwbServiceBusPublish.AwbServiceBusPublish.ConfigServiceBusQueue();

            var clientOptions = new ServiceBusClientOptions() { TransportType = ServiceBusTransportType.AmqpWebSockets };
            client = new ServiceBusClient(connectionString, clientOptions);

            // criar um processador que possamos usar para processar as mensagens
            processor = client.CreateProcessor(queueName, new ServiceBusProcessorOptions());

            try
            {
                // adicionar manipulador para processar mensagens
                processor.ProcessMessageAsync += MessageHandler;

                // adicionar manipulador para processar quaisquer erros
                processor.ProcessErrorAsync += ErrorHandler;

                // iniciar o processamento
                await processor.StartProcessingAsync();

                Console.WriteLine("Aguarde um minuto e pressione qualquer tecla para encerrar o processamento");
                Console.ReadKey();

                // parar de processar
                Console.WriteLine("\nParando o receptor...");
                await processor.StopProcessingAsync();
                Console.WriteLine("Parou de receber mensagens");
            }
            finally
            {
                // Chamar DisposeAsync em tipos de cliente é necessário para garantir que a rede
                // recursos e outros objetos não gerenciados são limpos adequadamente.
                await processor.DisposeAsync();
                await client.DisposeAsync();

            }
        }
    }
}
