using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace dio_explorer
{
    class Program
    {
        static async Task Main(string[] args)
        {

            string authToken = "Token de autorização";
            int maxPages = 1206;
            int usersWithAllProjectsCount = 0;
            int usersWithoutAllProjectsCount = 0;

            List<string> userData = new List<string>();


            for (int page = 1; page <= maxPages; page++)
            {
                Console.Clear(); // Limpar o console a cada ciclo
                Console.WriteLine($"Processando a página {page}...");

                string url = $"https://endpoint.com/ficticio/dio/page={page}";

                using (HttpClient client = new HttpClient())
                {
                    try
                    {
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

                        HttpResponseMessage response = await client.GetAsync(url);

                        if (response.IsSuccessStatusCode)
                        {
                            string usersResponseBody = await response.Content.ReadAsStringAsync();
                            var responseObject = JsonConvert.DeserializeAnonymousType(usersResponseBody, new { results = new User[0] });

                            foreach (var user in responseObject.results)
                            {
                                Console.WriteLine($"Testando usuário de id {user.id}...");

                                HttpResponseMessage projectsResponse = await client.GetAsync($"https://endpoint.com/ficticio/dio/users/{user.id}/projects/");

                                if (projectsResponse.IsSuccessStatusCode)
                                {
                                    string projectsResponseBody = await projectsResponse.Content.ReadAsStringAsync();
                                    var projects = JsonConvert.DeserializeObject<Project[]>(projectsResponseBody);

                                    bool hasAllProjects = true;
                                    foreach (var desiredProject in new[] { "Construindo um Sistema para um Estacionamento com C#", "Criando um Sistema e Abstraindo um Celular com POO em C#", "Implementando Validações de Testes Unitários com C#" })
                                    {
                                        bool hasProject = false;
                                        foreach (var project in projects)
                                        {
                                            if (project.project == desiredProject)
                                            {
                                                hasProject = true;
                                                break;
                                            }
                                        }

                                        if (!hasProject)
                                        {
                                            hasAllProjects = false;
                                            break;
                                        }
                                    }

                                    if (hasAllProjects)
                                    {
                                        usersWithAllProjectsCount++;
                                        userData.Add($"{user.name}, {user.linkedin}");

                                    }
                                    else
                                    {
                                        usersWithoutAllProjectsCount++;
                                    }
                                }
                                else
                                {
                                    Console.WriteLine($"Erro na solicitação dos projetos do usuário {user.id}: {projectsResponse.StatusCode}");
                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine($"Erro na solicitação: {response.StatusCode}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Ocorreu um erro durante a solicitação HTTP: {ex.Message}");
                    }
                }


                //  Salva o Nome e Linkedin de quem entregou os três projetos do DecolaTech 4. 

                string filePath = "C:\\Users\\André Magalhães\\Desktop\\PROJETOS\\PROJETOS C# (.NET)\\dio-explorer\\concluintes_decolatech4.txt";
                try
                {
                    using (StreamWriter writer = new StreamWriter(filePath))
                    {
                        foreach (string data in userData)
                        {
                            await writer.WriteLineAsync(data);
                        }
                    }
                    Console.WriteLine($"Dados dos usuários concluintes foram salvos em {filePath}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro ao salvar os dados dos usuários em {filePath}: {ex.Message}");
                }


                // Exibir o número de usuários que possuem e não possuem os três projetos desejados
                Console.WriteLine($"Número de usuários que possuem os três projetos desejados: {usersWithAllProjectsCount}");
                Console.WriteLine($"Número de usuários que não possuem os três projetos desejados: {usersWithoutAllProjectsCount}");
                await Task.Delay(1000); // Aguardar 2 segundos antes de limpar o console. Isso que deixou o processo mais lento. Teria sido MUITO mais rápido, mas queria que fosse possível acompanhar
            }
        }

        public class User
        {
            public string id { get; set; }
            public string name { get; set; }
            public string linkedin { get; set; }
        }

        public class Project
        {
            public string project { get; set; }
        }
    }
}

// Ferramenta que percorre todas as páginas e identifica quais IDs estão repetidos.

//using System;
//using System.Collections.Generic;
//using System.Net.Http;
//using System.Net.Http.Headers;
//using System.Threading.Tasks;
//using Newtonsoft.Json;

//namespace dio_explorer
//{
//    class Program
//    {
//        static async Task Main(string[] args)
//        {
//            string authToken = "Token de autorização";
//            HashSet<string> ids = new HashSet<string>();
//            HashSet<string> repeatedIds = new HashSet<string>();
//            int repeatedCount = 0;

//            for (int page = 1; page <= 1206; page++)
//            {
//                Console.Clear(); // Limpar o console a cada ciclo
//                Console.WriteLine($"Processando a página {page}...");

//                string url = $"https://endpoint.com/ficticio/dio/page={page}";

//                using (HttpClient client = new HttpClient())
//                {
//                    try
//                    {
//                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

//                        HttpResponseMessage response = await client.GetAsync(url);

//                        if (response.IsSuccessStatusCode)
//                        {
//                            string usersResponseBody = await response.Content.ReadAsStringAsync();
//                            var responseObject = JsonConvert.DeserializeAnonymousType(usersResponseBody, new { results = new User[0] });

//                            foreach (var user in responseObject.results)
//                            {
//                                if (!ids.Add(user.id))
//                                {
//                                    if (repeatedIds.Add(user.id))
//                                    {
//                                        repeatedCount++;
//                                    }
//                                }
//                            }
//                        }
//                        else
//                        {
//                            Console.WriteLine($"Erro na solicitação: {response.StatusCode}");
//                        }
//                    }
//                    catch (Exception ex)
//                    {
//                        Console.WriteLine($"Ocorreu um erro durante a solicitação HTTP: {ex.Message}");
//                    }
//                }

//                Console.WriteLine($"Quantidade de IDs repetidos até o momento: {repeatedCount}");
//                await Task.Delay(100); // Acelerei para o negócio andar mais rápido... 
//            }
//        }

//        public class User
//        {
//            public string id { get; set; }
//            public string name { get; set; }
//            public string linkedin { get; set; }
//        }
//    }
//}

// Sistema que conta possíveis repetições no arquivo .txt salvo

//using System;
//using System.Collections.Generic;
//using System.IO;

//class Program
//{
//    static void Main(string[] args)
//    {
//        // Caminho do arquivo de usuários concluintes
//        string filePath = "C:\\Users\\André Magalhães\\Desktop\\PROJETOS\\PROJETOS C# (.NET)\\dio-explorer\\concluintes_decolatech4.txt";

//        // Dicionário para armazenar links do LinkedIn e os usuários correspondentes. Lembrem-se da estrutura de dados DICIONÁRIO.
//        Dictionary<string, List<string>> linkedInUsers = new Dictionary<string, List<string>>();

//        // Verificar se o arquivo existe
//        if (File.Exists(filePath))
//        {
//            // Ler todas as linhas do arquivo
//            string[] lines = File.ReadAllLines(filePath);

//            // Iterar sobre as linhas do arquivo
//            foreach (string line in lines)
//            {
//                // Dividir a linha em partes usando a vírgula como separador
//                string[] parts = line.Split(',');
//                if (parts.Length == 2)
//                {
//                    string userName = parts[0].Trim();
//                    string linkedInLink = parts[1].Trim();

//                    // Verificar se o link do LinkedIn já existe no dicionário
//                    if (!linkedInUsers.ContainsKey(linkedInLink))
//                    {
//                        // Se não existir, criar uma nova entrada no dicionário
//                        linkedInUsers[linkedInLink] = new List<string>();
//                    }

//                    // Adicionar o nome do usuário à lista correspondente ao link do LinkedIn
//                    linkedInUsers[linkedInLink].Add(userName);
//                }
//            }

//            int totalParticipants = 0;

//            // Exibir ou armazenar os usuários repetidos
//            Console.WriteLine("Usuários com links do LinkedIn repetidos:");
//            foreach (var usuarioRepetido in linkedInUsers)
//            {
//                if (usuarioRepetido.Value.Count > 1)
//                {
//                    Console.WriteLine($"LinkedIn Link: {usuarioRepetido.Key}");
//                    Console.WriteLine($"Usuários: {string.Join(", ", usuarioRepetido.Value)}");
//                    Console.WriteLine($"Repetições: {usuarioRepetido.Value.Count - 1}");
//                    Console.WriteLine();
//                }
//                else
//                {
//                    totalParticipants++;
//                }
//            }

//            Console.WriteLine($"Número total de participantes que concluíram o DecolaTech4: {totalParticipants}");
//        }
//        else
//        {
//            Console.WriteLine("O arquivo de usuários concluintes não existe.");
//        }
//    }
//}
