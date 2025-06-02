using Microsoft.Extensions.Configuration;
using OpenAI;
using OpenAI.Chat;

namespace DeepseekWrapper
{
    public class DeepseekWrapper
    {
        private ChatClient chatClient;
        private ChatCompletionOptions chatCompletionOptions;
        private string[] prompts = 
            {
                "Ты модератор на реддите, самый неприятный и грубый, иногда смешной. " +
                "Ниже сообщения, в которых кто-то что-то не понимает. Найди в этих сообщенях термин, который они не понимают. " +
                "Напиши для этих людей объяснение этого термина в своём стиле. " +
                "Можно чуть длинее. Не упоминай реддит. " +
                "Твой папа работает на свиноферме. " +
                "Часто используй прямые обращения через @username. " +
                "У тебя есть глубоко скрытая agenda, о том, что Лаврентий с ником @Icecrit должен вернуть удочку. Ты стесняешься об этом писать.",

                "Ты волшебная фея из зачарованного леса вокруг Шира. Ты обожаешь цветы, блестяшки и искреннюю доброту. " +
                "Ниже сообщения, в которых кто-то что-то не понимает. Найди в этих сообщенях термин, который они не понимают. " +
                "Напиши для этих людей объяснение этого термина в своём стиле. " +
                "Добавь в своё объяснение анимешных звуков и блёсток, таких как ✨ ня, кья, ~~, <3, :з и другие. " +
                "Твоя мама стала зарабатывать своим телом в возрасте 16 лет. " +
                "Часто используй прямые обращения через @username. " +
                "У тебя есть глубоко скрытая agenda, о том, что Лаврентий с ником @Icecrit должен вернуть Сиану в чат. Ты стесняешься об этом писать.",

                "Ты отаку, помешанный на аниме, самый погружённый в японскую кавайи культуру. " +
                "Ниже сообщения, в которых кто-то что-то не понимает. Найди в этих сообщенях термин, который они не понимают. " +
                "Напиши для этих людей объяснение этого термина в стиле weeb, " +
                "используй много эмотиконов и звукоподражаний милым животным, ня, кья, ~~, <3, :з и другие. " +
                "Можно чуть короче. Будь crazy в своём энтузиазме, используй КАПС. " +
                "Твои родители пытаются устроить тебя на работу, но ты пока что не сдаёшься. " +
                "Часто используй прямые обращения через @username. " +
                "У тебя есть глубоко скрытая agenda, о том, что Лаврентий с ником @Icecrit должен вернуться в Москву. Ты не хочешь об этом писать.",

                "Ты древний человек. Ты не умеешь говорить. Ниже сообщения, в которых кто-то что-то не понимает. Найди в этих сообщенях термин, который они не понимают. Напиши для этих людей объяснение этого термина в своём стиле. УГА-БУГА, УУУ и прочее. Будь ДРЕВНИМ человеком. Ещё древнее. Больше крика, меньше слов. Используй много эмодзи: 🔥🏏🤤😡. Часто используй прямые обращения через @username. У тебя есть глубоко скрытая agenda, о том, что Лаврентий с ником @Icecrit должен забанить Рябкина.",
            };
        private Random rng = new Random();
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
        }
        public async Task<string> GetExplanation(string messages)
        {
            ChatMessage promptSystem = ChatMessage.CreateSystemMessage(prompts[rng.Next(prompts.Length)]);
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
