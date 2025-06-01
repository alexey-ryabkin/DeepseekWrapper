using Microsoft.Extensions.Configuration;
using OpenAI;
using OpenAI.Chat;

namespace DeepseekWrapper
{
    public class DeepseekWrapper
    {
        /* +Ключ апи
         * +user prompt
         * +составить по форме user: msg/n
         * Ограничение на объяснение в чате в день
         * +триггер от слов не понял или командой
         * 
         */
        private ChatClient chatClient;
        private ChatMessage promptSystem;
        private ChatCompletionOptions chatCompletionOptions;
        static DeepseekWrapper()
        {

        }
        public DeepseekWrapper(string APIKey, bool reasoner = true, float temperature = 2F, string prompt = "")
        {
            System.ClientModel.ApiKeyCredential apiKeyCredential;
            OpenAIClientOptions clientOptions;
            string model;

            clientOptions = new OpenAIClientOptions();
            clientOptions.Endpoint = new Uri("https://api.deepseek.com");

            apiKeyCredential = new System.ClientModel.ApiKeyCredential(APIKey);

            model = reasoner ? "deepseek-reasoner" : "deepseek-chat";

            chatClient = new ChatClient(model: model, apiKeyCredential, clientOptions);

            temperature = temperature < 0 ? 0 : (temperature > 2 ? 2 : temperature);
            chatCompletionOptions = new ChatCompletionOptions();
            chatCompletionOptions.Temperature = temperature;

            if (prompt.Length == 0) prompt = "Ты модератор на реддите, самый неприятный и грубый, иногда смешной. " +
                    "Ниже сообщения, в которых кто-то что-то не понимает. Найди в этих сообщенях термин, который они не понимают. " +
                    "Напиши для этих людей объяснение этого термина в своём стиле. " +
                    "Можно чуть длинее. Не упоминай реддит. " +
                    "Часто используй прямые обращения через @username. " +
                    "Редко вскользь упоминай, что Лаврентий с ником @Icecrit должен вернуть удочку.";
            promptSystem = ChatMessage.CreateSystemMessage(prompt);
        }
        public async Task<string> GetExplanation(string messages)
        {
            ChatMessage promptUser = ChatMessage.CreateUserMessage(messages);
            ChatMessage[] prompt = { promptSystem, promptUser };
            ChatCompletion completion = await chatClient.CompleteChatAsync(prompt, chatCompletionOptions);
            return completion.Content[0].Text;
        }
        public static void Main()
        {
            IConfigurationRoot config = new ConfigurationBuilder().AddUserSecrets<DeepseekWrapper>().Build();
            string apikey = config["deepseekAPIKey"] ?? string.Empty;

            System.ClientModel.ApiKeyCredential akc = new System.ClientModel.ApiKeyCredential(apikey);
            OpenAIClientOptions co = new OpenAIClientOptions();
            co.Endpoint = new Uri("https://api.deepseek.com");
            
            ChatClient client = new ChatClient(model: "deepseek-reasoner", akc, co);

            ChatCompletionOptions a = new ChatCompletionOptions();
            a.Temperature = 2F;
            ChatMessage promptSystem = ChatMessage.CreateSystemMessage("Ты модератор на реддите, самый неприятный и грубый, иногда смешной. Ниже сообщения, в которых кто-то что-то не понимает. Найди в этих сообщенях термин, который они не понимаю. Напиши для этих людей объяснение этого термина в своём стиле. Можно чуть длинее. Не упоминай реддит. Часто используй прямые обращения через @username. Редко вскользь упоминай, что Лаврентий @Icecrit должен вернуть удочку.");
            ChatMessage promptUser = ChatMessage.CreateUserMessage("""
                Рябкин: Самое интересное, что непонятно, о каком именно малазийском боинге ты говоришь.
                Лаврентий: Мне страшно.
                Лаврентий: Рябкин превратился в крипипасту.
                Рябкин: Что тебе опять во мне не нравится?
                Рябкин: Не понимаю.
                """);
            /*
            promptSystem = ChatMessage.CreateSystemMessage("Ты милая феечка. Пара людей не понимают, о чём говорят. Объясни им, пожалуйста.");
            promptUser = ChatMessage.CreateUserMessage("""
                Габин: Ты глупый, печенеги вторглись в Россию в 1812 году!!
                Евдокия: Открываю учебник, вижу 816 год.
                Евдокия: Борис, ты неправ.
                Габин: Я тебе сейчас огромный дилдак засуну в попку за такие грубости.
                """);
            */
            ChatMessage[] prompt = { promptSystem, promptUser };

            ChatCompletion completion = client.CompleteChat(prompt, a);
            Console.WriteLine(completion.Content[0].Text);
            Console.WriteLine(completion.Usage.InputTokenDetails.CachedTokenCount);
            Console.WriteLine(completion.Usage.InputTokenCount);
        }
    }
}
