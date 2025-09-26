using UnityEngine;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using TMPro; // TextMeshPro

public class aulaPokemonAPI : MonoBehaviour
{
    private readonly HttpClient httpClient = new HttpClient();
    private const string baseUrl = "https://pokeapi.co/api/v2/pokemon/";

    public Image pikachuImg;
    public TextMeshProUGUI pokemonInfoText; // Campo de texto para mostrar as informações

    async void Start()
    {
        Pokemon pikachu = await GetDadosPokemon("sylveon");

        if (pikachu != null)
        {
            string info = $"Nome: {pikachu.name}\n";
            info += $"ID: {pikachu.id}\n";
            info += $"Altura: {pikachu.height}\n";
            info += $"Peso: {pikachu.weight}\n";
            info += $"Experiência Base: {pikachu.base_experience}\n";

            if (pikachu.types != null && pikachu.types.Length > 0)
            {
                info += $"Tipo Principal: {pikachu.types[0].type.name}\n";
            }

            pokemonInfoText.text = info; // Atualiza o TMP com as informações

            // Carrega a imagem do Pokémon
            StartCoroutine(LoadPokemonSprite(pikachu.sprites.front_default));
        }
        else
        {
            pokemonInfoText.text = "Erro ao carregar dados do Pokémon.";
        }
    }

    public async Task<Pokemon> GetDadosPokemon(string nome)
    {
        string url = baseUrl + nome.ToLower();

        HttpResponseMessage response = await httpClient.GetAsync(url);
        string jsonResponse = await response.Content.ReadAsStringAsync();
        Pokemon pokemon = JsonUtility.FromJson<Pokemon>(jsonResponse);
        return pokemon;
    }

    IEnumerator LoadPokemonSprite(string spriteUrl)
    {
        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(spriteUrl))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Texture2D texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f);
                pikachuImg.sprite = sprite;
                Debug.Log("Imagem do Pokémon carregada com sucesso!");
            }
            else
            {
                Debug.LogError($"Erro ao carregar imagem: {request.error}");
            }
        }
    }

    void OnDestroy()
    {
        httpClient?.Dispose();
    }
}

