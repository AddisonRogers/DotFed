using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace DotFed;

public class SocialAbstraction
{
    

    public void SpecificConnection(string url)
    {
        
    }
    
    
    
    
    
}

public abstract class Credentials
{
    public string? Username { get; set; }
    public string? Email { get; set; }
    public string Password { get; set; }
}
public class Misc
{
    public int[] SortArray(int[] array, int leftIndex, int rightIndex)
    {
        var i = leftIndex;
        var j = rightIndex;
        var pivot = array[leftIndex];
        while (i <= j)
        {
            while (array[i] < pivot)
            {
                i++;
            }
        
            while (array[j] > pivot)
            {
                j--;
            }
            if (i <= j)
            {
                int temp = array[i];
                array[i] = array[j];
                array[j] = temp;
                i++;
                j--;
            }
        }
    
        if (leftIndex < j)
            SortArray(array, leftIndex, j);
        if (i < rightIndex)
            SortArray(array, i, rightIndex);
        return array;
    }
    // Copied
}
public class Lemmy
{
    public static HttpClient NewConnection(HttpClient client ,string url, string? ip)
    {
        client.BaseAddress = new Uri($"https://{url}/api/v3/");

        /*
        if (credentials != null)
        {
            WebRequest req = WebRequest.Create(url);
            req.Method= "POST";
            req.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(credentials.Username + ":" + credentials.Password)));
            req.Headers.Add("X-Forwarded-For", ip);
            HttpWebResponse resp = req.GetResponse() as HttpWebResponse ?? throw new Exception("Response is null");
        }
        */ // Haha woopsies maybe Ill do this later
        
        return client;
    }

    public static Task<JsonNode?> GetData(HttpClient client, int? limit, int? page, string? community)
    {
        if (limit == null) limit = 25;
        if (page == null) page = 1;
        return client.GetFromJsonAsync<JsonNode>("/community/" + "list?sort=new&limit=" + limit + "&page=" + page);
    }
}
// If I stop coding please shout at me

public class Kbin
{
    // https://docs.kbin.pub/#introduction
}

public class Discuit
{
    // Scrape
}

public class Mastodon
{
    // https://docs.joinmastodon.org/api/
}