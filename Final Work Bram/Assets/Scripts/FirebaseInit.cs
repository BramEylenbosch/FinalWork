using Firebase;
using Firebase.Auth;
using UnityEngine;
using System.Threading.Tasks;

public class FirebaseInit : MonoBehaviour
{
    public static FirebaseAuth auth;

    public async Task InitializeFirebase()
    {
        var dep = await FirebaseApp.CheckAndFixDependenciesAsync();
        if (dep == DependencyStatus.Available)
        {
            auth = FirebaseAuth.DefaultInstance;
            Debug.Log("[FirebaseInit] Firebase ready!");
        }
        else
        {
            Debug.LogError("[FirebaseInit] Firebase dependencies niet beschikbaar: " + dep);
        }
    }

    private void Awake()
    {
        // Start async initialisatie
        InitializeFirebase().ContinueWith(task =>
        {
            if (task.IsCompleted)
                Debug.Log("[FirebaseInit] Init completed");
        });
    }
}
