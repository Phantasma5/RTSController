using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class References : MonoBehaviour
{
    public static References instance;
    public static UserInterface UserInterface
    {
        get
        {
            return instance.userInterface;
        }
        set
        {
            instance.userInterface = value;
        }
    }
    private UserInterface userInterface;
    public static ClientNetwork ClientNet
    {
        get
        {
            return instance.clientNet;
        }
        set
        {
            instance.clientNet = value;
        }
    }
    private ClientNetwork clientNet;
    public static GameObject SelectionIndicator
    {
        get
        {
            return instance.selectionIndicator;
        }
        set
        {
            instance.selectionIndicator = value;
        }
    }
    [SerializeField] private GameObject selectionIndicator;
    public static AbilitySystem SendingAbilitySystem
    {
        get
        {
            return instance.sendingAbilitySystem;
        }
        set
        {
            instance.sendingAbilitySystem = value;
        }
    }
    [SerializeField] private AbilitySystem sendingAbilitySystem;
    public static StatSystem ClientStatSystem
    {
        get
        {
            return instance.clientStatSystem;
        }
        set
        {
            instance.clientStatSystem = value;
        }
    }
    private StatSystem clientStatSystem;

    private void Awake()
    {
        if (null != instance)
        {
            Destroy(this.gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(this.gameObject);
        FindReferences();
    }
    private void FindReferences()
    {
        clientNet = GetComponent<ClientNetwork>();
        userInterface = FindObjectOfType<UserInterface>();
        clientStatSystem = GetComponent<StatSystem>();
    }
}
