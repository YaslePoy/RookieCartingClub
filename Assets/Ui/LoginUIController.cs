using System.Net.Http;
using System.Text;
using DefaultNamespace;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class LoginUIController : MonoBehaviour
{
    public static User User { get; private set; }
    private UIDocument _ui;
    private TextField _email;
    public bool Authed = false;
    private TextField _password;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _ui = GetComponent<UIDocument>();
        _email = _ui.rootVisualElement.Q<TextField>("email");
        _password = _ui.rootVisualElement.Q<TextField>("password");
        _ui.rootVisualElement.Q<Button>("auth").clicked += () =>
        {
            if (string.IsNullOrWhiteSpace(_email.value) || string.IsNullOrWhiteSpace(_password.value))
                return;

            var client = new HttpClient();
            var auth = new AuthRequest { email = _email.value, password = _password.value };
            client.PostAsync("http://localhost:5110/api/auth",
                new StringContent(JsonUtility.ToJson(auth), Encoding.UTF8, "application/json")).ContinueWith(i =>
            {
                var response = i.Result;
                response.Content.ReadAsStringAsync().ContinueWith(s =>
                {
                    var user = s.Result;
                    User = JsonUtility.FromJson<User>(user);
                    SessionSetup.Nickname = User.username;
                    SessionSetup.Id = User.id;
                    Authed = true;
                });
            });
        };
        _ui.rootVisualElement.Q<Button>("reg").clicked += () =>
        {
            _ui.rootVisualElement.Q<VisualElement>("AuthForm").style.visibility = Visibility.Hidden;
            _ui.rootVisualElement.Q<VisualElement>("RegForm").style.visibility = Visibility.Visible;
        };

        _ui.rootVisualElement.Q<Button>("GoAuth").clicked += () =>
        {
            _ui.rootVisualElement.Q<VisualElement>("AuthForm").style.visibility = Visibility.Visible;
            _ui.rootVisualElement.Q<VisualElement>("RegForm").style.visibility = Visibility.Hidden;
        };
    }

    // Update is called once per frame
    void Update()
    {
        if (!Authed) return;
        
        Authed = false;
        SceneManager.LoadScene("Scenes/SelectorScene");
    }
}

public class AuthRequest
{
    public string email;
    public string password;
}

public class RegisterReqeust
{
    public string Username;
    public string Email;
    public string Password;
    public string About;
    public string Region;
}

public class User
{
    public int id;
    public string username;
    public string email;
    public string password;
    public string about;
    public string region;
    public int rating;
}