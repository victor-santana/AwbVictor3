using AwbRepository;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Reflection;

namespace AwbServiceBusPublish
{
    public class AwbServiceBusPublish
    {
        
        // o cliente que possui a conexão e pode ser usado para criar remetentes e destinatários
        static ServiceBusClient? client;

        //  remetente usado para publicar mensagens na fila
        static ServiceBusSender? sender;

        public static string ConfigServiceBusString()
        {
            var config = new ConfigurationBuilder()
           .SetBasePath(Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location))
           .AddJsonFile("C://Users//victor.santana//source//repos//AwbVictor3//AwbVictor3//appsettings.json").Build();
            var stringConection = config.GetConnectionString("ServiceBus");

            return stringConection;            
        }
        public static string ConfigServiceBusQueue()
        {
            var config = new ConfigurationBuilder()
           .SetBasePath(Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location))
           .AddJsonFile("C://Users//victor.santana//source//repos//AwbVictor3//AwbVictor3//appsettings.json").Build();            
            var queueName = config.GetConnectionString("QueueName");

            return queueName;
        }
        public static async Task AwbServiceBusPublishAsync()
        {
            var connectionString = ConfigServiceBusString();
            var queueName = ConfigServiceBusQueue(); 
            string entities = AwbRepository.AwbRepository.SelectBancoDeDados();
            var awbEntity = JsonConvert.DeserializeObject<AwbEntity>(entities);
            var teste = awbEntity.Table;

            List<AwbMessage> messagens = new List<AwbMessage>();

            foreach (Table itemTabela in teste)
            {
                AwbMessage messagem = new AwbMessage();

                messagem.Id = itemTabela.Id;
                messagem.PackingShippingNumber = itemTabela.PackingShippingNumber;
                messagem.CratedDate = itemTabela.CratedDate;
                messagem.ReceivedDate = itemTabela.ReceivedDate;
                messagem.PackedDate = itemTabela.PackedDate;
                messagem.ShippedDate = itemTabela.ShippedDate;
                messagem.Timestamp = itemTabela.Timestamp;

                messagens.Add(messagem);
            }

            var clientOptions = new ServiceBusClientOptions() { TransportType = ServiceBusTransportType.AmqpWebSockets };
            client = new ServiceBusClient(connectionString, clientOptions);
            sender = client.CreateSender(queueName);

            // criar um lote
            using ServiceBusMessageBatch? messageBatch = await sender.CreateMessageBatchAsync();


            //Publicar no ServiceBus
            foreach (var elemento in messagens)
            {
                messageBatch.TryAddMessage(new ServiceBusMessage(JsonConvert.SerializeObject(elemento)));
            }
            try
            {
                // use o cliente produtor para enviar o lote de mensagens para a fila do Barramento de Serviço
                await sender.SendMessagesAsync(messageBatch);
                Console.WriteLine($"Um lote de mensagens foi publicado na fila.");
            }
            finally
            {
                // Chamar DisposeAsync em tipos de cliente é necessário para garantir que os recursos de rede e outros objetos não gerenciados sejam limpos adequadamente.
                await sender.DisposeAsync();
                await client.DisposeAsync();
            }
            Console.WriteLine("Pressione qualquer tecla para encerrar o aplicativo...");
            Console.ReadKey();
        }
    }
}
